using System.Net;
using System.Text.Json;
using Application.Exceptions;

namespace Infrastructure.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message); // log record 

            context.Response.ContentType = "application/json";


            var response = ex switch
            {
                ApiException apiEx => new ErrorResponse
                {
                    StatusCode = apiEx.StatusCode,
                    Message = apiEx.Message,
                    Details = apiEx.Details
                },
                KeyNotFoundException => new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Resource not found"
                },
                ArgumentException => new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Invalid argument"
                },
                _ => new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = _env.IsDevelopment()
                        ? ex.Message
                        : "An unexpected error occurred",
                    Details = _env.IsDevelopment() ? ex.StackTrace : null
                }
            };

            context.Response.StatusCode = response.StatusCode;
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
}