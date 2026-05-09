using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.AI.Ollama;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Week05;

var builder = WebApplication.CreateBuilder(args);

// Load configuration
Config.Load();

string chatModel = Config.GetEnv("ANTHROPIC_MODEL", "llama3"); // Using llama3 by default via Ollama
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

// 2. Setup Semantic Kernel with Ollama
var skBuilder = Kernel.CreateBuilder();
skBuilder.AddOllamaChatCompletion(
    modelId: chatModel,
    endpoint: new Uri(ollamaEndpoint)
);

var kernel = skBuilder.Build();
builder.Services.AddSingleton<Kernel>(kernel);

// Add standard web services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Health endpoint
app.MapGet("/health", () => TypedResults.Ok(new { 
    status = "Healthy", 
    provider = "Ollama",
    model = chatModel,
    timestamp = DateTime.UtcNow 
}))
.WithName("GetHealth")
.WithOpenApi();

app.Run();
