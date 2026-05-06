using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Week03;

/// <summary>
/// Generates dense vector embeddings via an OpenAI-compatible /v1/embeddings endpoint.
/// Uses raw HttpClient for maximum compatibility with Ollama and other local models.
/// </summary>
public class EmbeddingService
{
    private readonly HttpClient _http;
    private readonly string _model;
    private readonly string _endpoint;

    // nomic-embed-text → 768 dims; text-embedding-3-small → 1536 dims
    public int Dimensions => _model.Contains("nomic") ? 768 : 1536;

    public EmbeddingService(string baseUrl, string apiKey, string model)
    {
        _model    = model;
        _endpoint = baseUrl.EndsWith("/embeddings") ? baseUrl : baseUrl.TrimEnd('/') + "/embeddings";
        _http     = new HttpClient();
        _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
    }

    /// <summary>Embed a single text string.</summary>
    public async Task<float[]> EmbedAsync(string text)
    {
        var results = await EmbedBatchAsync([text]);
        return results[0];
    }

    /// <summary>
    /// Embeds texts one by one (some Ollama versions don't support true batch).
    /// For production, switch to batching once your embedding provider supports it.
    /// </summary>
    public async Task<float[][]> EmbedBatchAsync(IEnumerable<string> texts)
    {
        var results = new List<float[]>();
        foreach (var text in texts)
        {
            var payload = new { model = _model, input = text };
            var response = await _http.PostAsJsonAsync(_endpoint, payload);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<EmbeddingResponse>()
                       ?? throw new Exception("Null embedding response");

            results.Add(json.Data[0].Embedding);
        }
        return results.ToArray();
    }

    // ── JSON response models ──────────────────────────────────────
    private record EmbeddingResponse(
        [property: JsonPropertyName("data")]  List<EmbeddingData>  Data,
        [property: JsonPropertyName("model")] string               Model
    );

    private record EmbeddingData(
        [property: JsonPropertyName("embedding")] float[] Embedding,
        [property: JsonPropertyName("index")]     int     Index
    );
}
