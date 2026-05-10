using Microsoft.KernelMemory;

namespace Week06.Reranking;

public class LocalReranker
{
    private readonly IKernelMemory _memory;

    public LocalReranker(IKernelMemory memory)
    {
        _memory = memory;
    }

    public async Task<List<Search.SearchResult>> RerankAsync(string query, List<Search.SearchResult> results)
    {
        Console.WriteLine($"Reranking {results.Count} results using Ollama...");

        // In a real scenario, we'd use a Cross-Encoder or a specific rerank prompt.
        // For this setup, we'll scaffold the logic to ask Ollama for a relevance score.
        foreach (var result in results)
        {
            var prompt = $"On a scale of 0-10, how relevant is this text to the query: '{query}'?\n\nText: {result.Text}\n\nScore:";
            // var response = await _memory.AskAsync(prompt); 
            // result.Score = ParseScore(response);
        }

        return results.OrderByDescending(r => r.Score).ToList();
    }
}
