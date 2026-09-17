namespace StudentApi.Services;

public interface IOllamaChatService
{
    Task<string> GetResponseAsync(string message, CancellationToken cancellationToken);
}
