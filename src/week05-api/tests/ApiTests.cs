using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Week05.Tests;

public class ApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetHealth_ReturnsHealthy()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<HealthResponse>();
        Assert.NotNull(content);
        Assert.Equal("Healthy", content.status);
    }

    [Fact]
    public async Task GetHistory_ReturnsEmptyForNewChat()
    {
        // Arrange
        var client = _factory.CreateClient();
        var chatId = Guid.NewGuid().ToString();

        // Act
        var response = await client.GetAsync($"/chat/history/{chatId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var history = await response.Content.ReadFromJsonAsync<List<ChatMessage>>();
        Assert.NotNull(history);
        // It should contain 1 message (system prompt)
        Assert.Single(history);
        Assert.Equal("system", history[0].role);
    }

    private record HealthResponse(string status, string provider, string model, DateTime timestamp);
    private record ChatMessage(string role, string content);
}
