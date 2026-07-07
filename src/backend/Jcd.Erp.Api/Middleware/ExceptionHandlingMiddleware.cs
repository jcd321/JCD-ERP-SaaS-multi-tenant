using System.Text.Json;
using FluentValidation;
using Jcd.Erp.Domain.Common;

namespace Jcd.Erp.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error");
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/problem+json";

            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                type = "https://tools.ietf.org/html/rfc7807",
                title = "Validation Error",
                status = 400,
                errors,
                traceId = context.TraceIdentifier
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                type = "https://tools.ietf.org/html/rfc7807",
                title = "Internal Server Error",
                status = 500,
                traceId = context.TraceIdentifier
            }));
        }
    }
}
