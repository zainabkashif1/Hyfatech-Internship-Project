namespace StudentApi.DTOs;

public sealed class ChatResponse
{
    public string Message { get; init; } = string.Empty;
    public string Model { get; init; } = string.Empty;
}
