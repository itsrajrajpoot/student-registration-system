using System.Net;
using System.Text.Json;

namespace StudentRegistrationSystem.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = ex.Message,   // 👈 SHOW REAL ERROR
                Detailed = ex.InnerException?.Message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}