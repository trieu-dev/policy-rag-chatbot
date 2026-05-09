using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.AI.Ollama;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text.Json;
using Week05;

var builder = WebApplication.CreateBuilder(args);

// Load configuration
Config.Load();

string chatModel = Config.GetEnv("ANTHROPIC_MODEL", "llama3");
string ollamaEndpoint = Config.GetEnv("OLLAMA_ENDPOINT", "http://localhost:11434");
string embedModel = Config.GetEnv("OPENAI_MODEL", "nomic-embed-text");
string qdrantUrl = Config.GetEnv("QDRANT_URL", "http://localhost:6333");

// 1. Setup Kernel Memory with Ollama
var memoryBuilder = new KernelMemoryBuilder()
    .WithOllamaTextGeneration(new OllamaConfig
    {
        Endpoint = ollamaEndpoint,
        TextModel = new OllamaModelConfig(chatModel)
    })
    .WithOllamaTextEmbeddingGeneration(new OllamaConfig
    {
        Endpoint = ollamaEndpoint,
        EmbeddingModel = new OllamaModelConfig(embedModel)
    })
    .WithQdrantMemoryDb(qdrantUrl)
    .WithSimpleFileStorage("data");

var memory = memoryBuilder.Build<MemoryServerless>();
builder.Services.AddSingleton<IKernelMemory>(memory);

// 2. Setup Custom Services
builder.Services.AddSingleton<IIngestionQueue, IngestionQueue>();
builder.Services.AddSingleton<IHistoryStore, HistoryStore>();
builder.Services.AddHostedService<IngestionWorker>();

// 3. Setup Semantic Kernel with Ollama
var skBuilder = Kernel.CreateBuilder();
skBuilder.AddOllamaChatCompletion(
    modelId: chatModel,
    endpoint: new Uri(ollamaEndpoint)
);

var kernel = skBuilder.Build();
builder.Services.AddSingleton<Kernel>(kernel);

// 4. API Infrastructure
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// --- Endpoints ---

// GET /health
app.MapGet("/health", () => TypedResults.Ok(new { status = "Healthy", provider = "Ollama", model = chatModel, timestamp = DateTime.UtcNow }))
   .WithName("GetHealth")
   .WithOpenApi(op => { op.Summary = "Check API health"; return op; });

// POST /ingest
app.MapPost("/ingest", async (IFormFile file, IIngestionQueue queue) =>
{
    if (file == null || file.Length == 0) return Results.BadRequest("No file uploaded.");
    
    var dataPath = Path.Combine(Directory.GetCurrentDirectory(), "data", "uploads");
    if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);

    var filePath = Path.Combine(dataPath, file.FileName);
    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }

    await queue.QueueBackgroundWorkItemAsync(filePath);
    return Results.Accepted(value: new { message = "File uploaded and queued for ingestion", fileName = file.FileName });
})
.WithName("IngestFile")
.DisableAntiforgery() // Simplification for dev
.WithOpenApi(op => { op.Summary = "Upload a PDF for background ingestion"; return op; });

// POST /chat (SSE Streaming)
app.MapPost("/chat", async (HttpContext context, [FromBody] ChatRequest request, Kernel kernel, IKernelMemory memory, IHistoryStore historyStore) =>
{
    context.Response.ContentType = "text/event-stream";
    
    var searchResult = await memory.AskAsync(request.Query);
    string ragContext = searchResult.Result;
    string citations = string.Join("\n", searchResult.RelevantSources.Select(s => $"Source: {s.SourceName} ({s.Link})"));

    var history = historyStore.GetHistory(request.ChatId);
    history.AddUserMessage(request.Query);

    var chatService = kernel.GetRequiredService<IChatCompletionService>();
    string systemPrompt = $@"Use the following context to answer the user's question. 
Context:
{ragContext}

If the answer is not in the context, say so. Always cite sources if possible.";

    var turnHistory = new ChatHistory(systemPrompt);
    foreach (var msg in history) turnHistory.Add(msg);

    var responseStream = chatService.GetStreamingChatMessageContentsAsync(turnHistory, kernel: kernel);
    string fullResponse = "";

    await foreach (var chunk in responseStream)
    {
        if (chunk.Content != null)
        {
            fullResponse += chunk.Content;
            await context.Response.WriteAsync($"data: {JsonSerializer.Serialize(new { content = chunk.Content })}\n\n");
            await context.Response.Body.FlushAsync();
        }
    }

    if (!string.IsNullOrEmpty(citations))
    {
        await context.Response.WriteAsync($"data: {JsonSerializer.Serialize(new { content = "\n\n" + citations })}\n\n");
        await context.Response.Body.FlushAsync();
        fullResponse += "\n\n" + citations;
    }

    history.AddAssistantMessage(fullResponse);
    historyStore.SaveHistory(request.ChatId, history);

    await context.Response.WriteAsync("data: [DONE]\n\n");
    await context.Response.Body.FlushAsync();
})
.WithName("ChatStream")
.WithOpenApi(op => { op.Summary = "Ask a question and get a streamed RAG response (SSE)"; return op; });

// GET /chat/history
app.MapGet("/chat/history/{chatId}", (string chatId, IHistoryStore historyStore) =>
{
    var history = historyStore.GetHistory(chatId);
    return Results.Ok(history.Select(m => new { role = m.Role.ToString(), content = m.Content }));
})
.WithName("GetChatHistory")
.WithOpenApi(op => { 
    op.Summary = "Retrieve conversation history"; 
    op.Description = "Returns the full list of messages for a specific chat session.";
    return op; 
});

// DELETE /chat/{chatId}
app.MapDelete("/chat/{chatId}", (string chatId, IHistoryStore historyStore) =>
{
    historyStore.DeleteHistory(chatId);
    return Results.NoContent();
})
.WithName("DeleteChatHistory")
.WithOpenApi(op => { 
    op.Summary = "Delete chat history"; 
    op.Description = "Removes all stored messages for a specific chat session.";
    return op; 
});

app.Run();

// Request Model
public record ChatRequest(string Query, string ChatId = "default");

// Make the implicit Program class public so test projects can access it
public partial class Program { }
