using System.Net.Http.Json;
using System.Text.Json;

namespace Week01;

/// <summary>
/// Pure math helpers for vector operations.
/// No external dependencies — understand the fundamentals first.
/// </summary>
public static class VectorMath
{
    /// <summary>
    /// Cosine similarity between two vectors.
    /// Returns 1.0 = identical direction, 0.0 = orthogonal, -1.0 = opposite.
    /// For text embeddings, higher = more semantically similar.
    /// </summary>
    public static float CosineSimilarity(float[] a, float[] b)
    {
        if (a.Length != b.Length)
            throw new ArgumentException("Vectors must have the same dimension.");

        float dot = 0f, normA = 0f, normB = 0f;
        for (int i = 0; i < a.Length; i++)
        {
            dot   += a[i] * b[i];
            normA += a[i] * a[i];
            normB += b[i] * b[i];
        }

        if (normA == 0f || normB == 0f) return 0f;
        return dot / (MathF.Sqrt(normA) * MathF.Sqrt(normB));
    }

    /// <summary>
    /// Find the most similar vector from a list.
    /// Returns (index, similarity score).
    /// </summary>
    public static (int Index, float Score) FindMostSimilar(float[] query, IList<float[]> candidates)
    {
        int bestIdx = -1;
        float bestScore = float.MinValue;

        for (int i = 0; i < candidates.Count; i++)
        {
            float score = CosineSimilarity(query, candidates[i]);
            if (score > bestScore) { bestScore = score; bestIdx = i; }
        }

        return (bestIdx, bestScore);
    }
}

/// <summary>
/// Thin wrapper to call an embedding API and return float[] vectors.
/// Swap the endpoint to use OpenAI, Voyage, or any compatible provider.
/// </summary>
public class EmbeddingClient(HttpClient http, string apiKey, string? endpoint = null, string? model = null)
{
    // TODO Week 1: swap to your chosen provider endpoint
    // OpenAI:  https://api.openai.com/v1/embeddings  (model: text-embedding-3-small)
    // Voyage:  https://api.voyageai.com/v1/embeddings (model: voyage-3)
    private readonly string _endpoint = endpoint ?? "https://api.openai.com/v1/embeddings";
    private readonly string _model = model ?? "text-embedding-3-small";

    public async Task<float[]> EmbedAsync(string text)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, _endpoint);
        req.Headers.Add("Authorization", $"Bearer {apiKey}");
        req.Content = JsonContent.Create(new { model = _model, input = text });

        var response = await http.SendAsync(req);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        var values = body
            .GetProperty("data")[0]
            .GetProperty("embedding")
            .EnumerateArray()
            .Select(v => v.GetSingle())
            .ToArray();

        return values;
    }

    public async Task<List<float[]>> EmbedBatchAsync(IEnumerable<string> texts)
    {
        var results = new List<float[]>();
        // Note: Week 1 — sequential for clarity. Week 7 adds caching + batching.
        foreach (var text in texts)
            results.Add(await EmbedAsync(text));
        return results;
    }
}
