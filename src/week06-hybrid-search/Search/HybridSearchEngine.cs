using Microsoft.KernelMemory;
using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace Week06.Search;

public class HybridSearchEngine
{
    private readonly IKernelMemory _memory;
    private readonly QdrantClient _qdrantClient;
    private readonly string _collectionName;

    public HybridSearchEngine(IKernelMemory memory, string qdrantEndpoint, string collectionName)
    {
        _memory = memory;
        var uri = new Uri(qdrantEndpoint);
        _qdrantClient = new QdrantClient(uri.Host, uri.Port, https: uri.Scheme == "https");
        _collectionName = collectionName;
    }

    public async Task<List<SearchResult>> SearchAsync(string query, int limit = 5)
    {
        // 1. Dense Search via Kernel Memory
        var denseResults = await _memory.SearchAsync(query, _collectionName, limit: limit * 2);

        // 2. Sparse Search via Qdrant Direct (BM25)
        var queryTokens = BM25Tokenizer.Tokenize(query);
        // var sparseResults = await _qdrantClient.SearchAsync(...) 
        // [Refining search logic for Step 3]

        // 3. Combine with RRF (Simplified logic)
        return denseResults.Results.Select(r => new SearchResult 
        { 
            Text = r.SourceName, 
            Score = 1.0f 
        }).ToList();
    }
}

public class SearchResult
{
    public string Text { get; set; } = string.Empty;
    public float Score { get; set; }
}
