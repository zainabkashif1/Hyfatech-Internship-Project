using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StudentApi.Options;

namespace StudentApi.Services;

public sealed class OllamaChatService : IOllamaChatService
{
    private readonly HttpClient _httpClient;
    private readonly OllamaOptions _options;
    private readonly ILogger<OllamaChatService> _logger;

    public OllamaChatService(
        HttpClient httpClient,
        IOptions<OllamaOptions> options,
        ILogger<OllamaChatService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string> GetResponseAsync(string message, CancellationToken cancellationToken)
    {
        var request = new OllamaChatRequest
        {
            Model = _options.Model,
            Stream = false,
            Messages =
            [
                new OllamaMessage { Role = "system", Content = _options.SystemPrompt },
                new OllamaMessage { Role = "user", Content = message }
            ]
        };

        using var response = await _httpClient.PostAsJsonAsync("api/chat", request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Ollama returned {Status}: {Body}", response.StatusCode, body);
            throw new HttpRequestException("Ollama request failed");
        }

        using var document = System.Text.Json.JsonDocument.Parse(body);
        var messageNode = document.RootElement.TryGetProperty("message", out var messageElement)
            ? messageElement
            : default;

        var content = messageNode.TryGetProperty("content", out var contentElement)
            ? contentElement.GetString()
            : null;

        if (string.IsNullOrWhiteSpace(content) && messageNode.TryGetProperty("thinking", out var thinkingElement))
        {
            content = thinkingElement.GetString();
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new HttpRequestException("Ollama returned an empty message.");
        }

        return content.Trim();
    }

    private sealed class OllamaChatRequest
    {
        public string Model { get; init; } = string.Empty;
        public bool Stream { get; init; }
        public bool Think { get; init; }
        public IReadOnlyList<OllamaMessage> Messages { get; init; } = [];
    }

    private sealed class OllamaMessage
    {
        public string Role { get; init; } = string.Empty;
        public string Content { get; init; } = string.Empty;
    }

}
