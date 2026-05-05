using System.Net.Http.Json;
using System.Text.Json;

namespace Week01;

/// <summary>
/// Minimal Claude API client using raw HttpClient.
/// Week 1: understand the API shape before adding abstraction layers.
/// </summary>
public class ClaudeClient(HttpClient http, string apiKey)
{
    private const string Model = "claude-sonnet-4-20250514";
    private const string BaseUrl = "https://api.anthropic.com/v1/messages";

    public async Task<string> AskAsync(string systemPrompt, string userMessage)
    {
        var request = new
        {
            model = Model,
            max_tokens = 1024,
            system = systemPrompt,
            messages = new[] { new { role = "user", content = userMessage } }
        };

        using var req = new HttpRequestMessage(HttpMethod.Post, BaseUrl);
        req.Headers.Add("x-api-key", apiKey);
        req.Headers.Add("anthropic-version", "2023-06-01");
        req.Content = JsonContent.Create(request);

        var response = await http.SendAsync(req);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return body
            .GetProperty("content")[0]
            .GetProperty("text")
            .GetString() ?? string.Empty;
    }
}
