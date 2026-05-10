using Microsoft.Extensions.Configuration;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.AI.Ollama;
using Microsoft.KernelMemory.MemoryDb.Qdrant;
using Microsoft.SemanticKernel;

Console.WriteLine("=== Week 06: Hybrid Search & Reranking ===");

// 1. Load Configuration
var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var ollamaConfig = config.GetSection("Ollama");
var qdrantConfig = config.GetSection("Qdrant");

// 2. Setup Ollama Services (Local LLM & Embeddings)
var ollamaEndpoint = ollamaConfig["Endpoint"]!;
var textModel = ollamaConfig["TextModel"]!;
var embeddingModel = ollamaConfig["EmbeddingModel"]!;

// 3. Setup Kernel Memory with Qdrant
var memory = new KernelMemoryBuilder()
    .WithOllamaTextGeneration(new OllamaConfig
    {
        Endpoint = ollamaEndpoint,
        TextModel = new OllamaModelConfig(textModel)
    })
    .WithOllamaTextEmbeddingGeneration(new OllamaConfig
    {
        Endpoint = ollamaEndpoint,
        EmbeddingModel = new OllamaModelConfig(embeddingModel)
    })
    .WithQdrantMemoryDb(qdrantConfig["Endpoint"]!)
    .Build<MemoryServerless>();

Console.WriteLine($"Initialized with Ollama ({textModel}) and Qdrant.");

// 4. Step 1 - Enable Sparse Vectors in Qdrant
var collectionName = config["KernelMemory:IndexName"] ?? "policy-rag-hybrid";
await Week06.Search.QdrantSparseHelper.EnsureSparseVectorEnabledAsync(qdrantConfig["Endpoint"]!, collectionName);

// TODO: Step 2 - Implement BM25 Tokenizer
// TODO: Step 3 - Implement Hybrid Search Logic

Console.WriteLine("Ready for Hybrid Search setup.");
