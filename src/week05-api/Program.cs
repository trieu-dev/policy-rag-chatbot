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

// 3. API Infrastructure
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

app.Run();

// Make the implicit Program class public so test projects can access it
public partial class Program { }
