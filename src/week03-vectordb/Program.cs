using DotNetEnv;
using Week03;

// ---------------------------------------------------------------
// Week 3 Deliverable: PDF → Chunks → Embeddings → Qdrant
// Usage: dotnet run <path-to-pdf> [search query]
// ---------------------------------------------------------------

// Load environment variables from infra/.env (walk up from CWD)
var envPath = FindEnvFile();
if (envPath is not null)
    Env.Load(envPath);

// --- Config ---
var qdrantUrl = Env.GetString("QDRANT_URL", "http://localhost:6333");
var openAiEndpoint = Env.GetString("OPENAI_ENDPOINT", "http://localhost:11434/v1");
var openAiKey = Env.GetString("OPENAI_API_KEY", "dummy");
var embeddingModel = Env.GetString("OPENAI_MODEL", "nomic-embed-text");
const string CollectionName = "policy-chunks";

// --- Args ---
if (args.Length == 0)
{
    Console.WriteLine("Usage: dotnet run <path-to-pdf> [\"optional search query\"]");
    return;
}

var pdfPath = args[0];
var searchQuery = args.Length > 1 ? args[1] : null;

if (!File.Exists(pdfPath))
{
    Console.WriteLine($"Error: File not found — {pdfPath}");
    return;
}

Console.WriteLine("=== Week 3 — Vector DB Pipeline ===\n");

// ── Step 1: Extract chunks from PDF ──────────────────────────────
Console.WriteLine("Step 1: Extracting chunks from PDF...");
var extractor = new PdfExtractor();
var chunks = extractor.ExtractChunks(pdfPath);
Console.WriteLine($"  → {chunks.Count} chunks extracted.\n");

// ── Step 2: Embed chunks ──────────────────────────────────────────
Console.WriteLine($"Step 2: Generating embeddings ({embeddingModel})...");
var embedder = new EmbeddingService(openAiEndpoint, openAiKey, embeddingModel);
var texts = chunks.Select(c => c.Text).ToArray();
var embeddings = await embedder.EmbedBatchAsync(texts);
Console.WriteLine($"  → {embeddings.Length} vectors generated (dim={embedder.Dimensions}).\n");

// ── Step 3: Upsert into Qdrant ───────────────────────────────────
Console.WriteLine($"Step 3: Upserting into Qdrant ({qdrantUrl})...");
var repo = new QdrantRepository(qdrantUrl, CollectionName, (uint)embedder.Dimensions);
await repo.EnsureCollectionAsync();
await repo.UpsertChunksAsync(chunks, embeddings);
Console.WriteLine($"\n  ✅ Upsert complete!");
Console.WriteLine($"  → Open dashboard: {qdrantUrl}/dashboard\n");

// ── Step 4 (optional): Search ────────────────────────────────────
if (searchQuery is not null)
{
    Console.WriteLine($"Step 4: Searching for \"{searchQuery}\"...");
    var queryVector = await embedder.EmbedAsync(searchQuery);
    var results = await repo.SearchAsync(queryVector, topK: 3);

    Console.WriteLine($"\nTop {results.Count} results:");
    foreach (var (r, i) in results.Select((r, i) => (r, i + 1)))
    {
        Console.WriteLine($"\n  [{i}] Score: {r.Score:F4} | {r.SourceFile} p.{r.PageNumber}");
        Console.WriteLine($"      {r.Text.Replace("\n", " ").Substring(0, Math.Min(200, r.Text.Length))}...");
    }
}

Console.WriteLine("\n--- Week 3 done. Week 4: Add Semantic Kernel orchestration! ---");

// ── Helpers ───────────────────────────────────────────────────────
static string? FindEnvFile()
{
    var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
    while (dir is not null)
    {
        var candidate = Path.Combine(dir.FullName, "infra", ".env");
        if (File.Exists(candidate)) return candidate;
        dir = dir.Parent;
    }
    return null;
}
