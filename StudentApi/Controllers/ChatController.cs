using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StudentApi.DTOs;
using StudentApi.Options;
using StudentApi.Services;

namespace StudentApi.Controllers;

[ApiController]
[Route("api/chat")]
public sealed class ChatController : ControllerBase
{
    private readonly IOllamaChatService _chatService;
    private readonly OllamaOptions _options;
    private readonly ILogger<ChatController> _logger;

    public ChatController(
        IOllamaChatService chatService,
        IOptions<OllamaOptions> options,
        ILogger<ChatController> logger)
    {
        _chatService = chatService;
        _options = options.Value;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ChatResponse>> Chat(
        ChatRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _chatService.GetResponseAsync(request.Message, cancellationToken);
            return Ok(new ChatResponse { Message = response, Model = _options.Model });
        }
        catch (HttpRequestException exception)
        {
            _logger.LogError(exception, "Ollama is unavailable.");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                error = "The Ollama model is unavailable. Make sure Ollama is running."
            });
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return StatusCode(StatusCodes.Status504GatewayTimeout, new
            {
                error = "The Ollama request timed out."
            });
        }
    }
}
