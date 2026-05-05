using Week01;

// ---------------------------------------------------------------
// Week 1 Deliverable: first RAG demo — no vector DB, no framework
// Pure fundamentals: embed → similarity → grounded Claude answer
// ---------------------------------------------------------------

var anthropicKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY") ?? "dummy";
var openAiKey    = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "dummy";

var anthropicEndpoint = Environment.GetEnvironmentVariable("ANTHROPIC_ENDPOINT");
var openaiEndpoint    = Environment.GetEnvironmentVariable("OPENAI_ENDPOINT");
var anthropicModel    = Environment.GetEnvironmentVariable("ANTHROPIC_MODEL");
var openaiModel       = Environment.GetEnvironmentVariable("OPENAI_MODEL");

var http          = new HttpClient();
var claudeClient  = new ClaudeClient(http, anthropicKey, anthropicEndpoint, anthropicModel);
var embedClient   = new EmbeddingClient(http, openAiKey, openaiEndpoint, openaiModel);

// --- Hardcoded policy chunks (Week 3 will store these in Qdrant) ---
var policyChunks = new[]
{
    "Section 3.1 – Dental Coverage: The annual deductible for dental coverage is $500 per insured member. Preventive care (cleanings, X-rays) is covered at 100% with no deductible. Basic restorative work is covered at 80% after deductible.",
    "Section 4.2 – Vision Coverage: Members are entitled to one eye exam per calendar year at no cost. Frames and lenses are covered up to $200 per year. Contact lenses are covered as an alternative to glasses, up to $150 per year.",
    "Section 5.0 – Emergency Care: Emergency room visits are covered at 90% after a $150 copay. Ambulance services are covered at 100% when medically necessary. Out-of-network emergency care is covered at the same rate as in-network.",
    "Section 6.1 – Prescription Drugs: Tier 1 generic drugs: $10 copay. Tier 2 preferred brand: $35 copay. Tier 3 non-preferred brand: $65 copay. Specialty medications require prior authorization.",
};

var userQuestion = "What is the dental deductible and what does it cover?";

Console.WriteLine("=== Policy RAG Demo — Week 1 ===\n");
Console.WriteLine($"Question: {userQuestion}\n");

// Step 1: Embed all chunks
Console.WriteLine("Embedding policy chunks...");
var chunkEmbeddings = await embedClient.EmbedBatchAsync(policyChunks);

// Step 2: Embed the question
Console.WriteLine("Embedding question...");
var questionEmbedding = await embedClient.EmbedAsync(userQuestion);

// Step 3: Find most relevant chunk
var (bestIdx, score) = VectorMath.FindMostSimilar(questionEmbedding, chunkEmbeddings);
var bestChunk = policyChunks[bestIdx];

Console.WriteLine($"\nMost relevant chunk (similarity: {score:F3}):");
Console.WriteLine($"  \"{bestChunk}\"\n");

// Step 4: Build grounded prompt and ask Claude
var systemPrompt = """
    You are a helpful policy assistant. Answer the user's question using ONLY
    the policy context provided below. If the answer is not in the context,
    say "I don't have that information in the policy documents."
    Always cite the section number when available.
    """;

var userPrompt = $"""
    Policy context:
    {bestChunk}

    Question: {userQuestion}
    """;

Console.WriteLine("Asking Claude...\n");
var answer = await claudeClient.AskAsync(systemPrompt, userPrompt);

Console.WriteLine("Answer:");
Console.WriteLine(answer);
Console.WriteLine("\n--- Done. Week 2: we'll store all chunks in Qdrant instead. ---");
