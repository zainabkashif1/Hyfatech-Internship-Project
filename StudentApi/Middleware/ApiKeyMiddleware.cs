namespace StudentApi.Middleware;

public sealed class ApiKeyMiddleware
{
    private const string ApiKeyHeader = "X-Api-Key";
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/api/chat"))
        {
            var expectedKey = _configuration["ApiKey"];
            var suppliedKey = context.Request.Headers[ApiKeyHeader].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(expectedKey) || suppliedKey != expectedKey)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "A valid X-Api-Key header is required." });
                return;
            }
        }

        await _next(context);
    }
}
