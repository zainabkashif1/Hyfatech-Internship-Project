namespace StudentApi.Options;

public sealed class OllamaOptions
{
    public string BaseUrl { get; set; } = "http://localhost:11434";
    public string Model { get; set; } = "qwen3.5:0.8b";
    public string SystemPrompt { get; set; } = "You are a helpful assistant.";
    public int TimeoutSeconds { get; set; } = 120;
}
