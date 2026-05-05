using System.Net.Http.Json;
using System.Text.Json;

namespace Week01;

/// <summary>
/// Minimal Claude API client using raw HttpClient.
/// Week 1: understand the API shape before adding abstraction layers.
/// </summary>
public class ClaudeClient(HttpClient http, string apiKey, string? baseUrl = null, string? model = null)
{
    private readonly string _model = model ?? "claude-sonnet-4-20250514";
    private readonly string _baseUrl = baseUrl ?? "https://api.anthropic.com/v1/messages";

    public async Task<string> AskAsync(string systemPrompt, string userMessage)
    {
        bool isAnthropic = _baseUrl.Contains("anthropic.com");
        
        object request;
        if (isAnthropic)
        {
            request = new
            {
                model = _model,
                max_tokens = 1024,
                system = systemPrompt,
                messages = new[] { new { role = "user", content = userMessage } }
            };
        }
        else
        {
            // OpenAI/Ollama/Groq format
            request = new
            {
                model = _model,
                messages = new[] 
                { 
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userMessage } 
                }
            };
        }

        using var req = new HttpRequestMessage(HttpMethod.Post, _baseUrl);
        if (isAnthropic)
        {
            req.Headers.Add("x-api-key", apiKey);
            req.Headers.Add("anthropic-version", "2023-06-01");
        }
        else
        {
            req.Headers.Add("Authorization", $"Bearer {apiKey}");
        }
        
        req.Content = JsonContent.Create(request);

        var response = await http.SendAsync(req);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        
        if (isAnthropic)
        {
            return body
                .GetProperty("content")[0]
                .GetProperty("text")
                .GetString() ?? string.Empty;
        }
        else
        {
            // OpenAI/Ollama format
            return body
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? string.Empty;
        }
    }
}
