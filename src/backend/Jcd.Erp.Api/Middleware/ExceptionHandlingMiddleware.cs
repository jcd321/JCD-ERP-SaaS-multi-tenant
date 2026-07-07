using System.Text.Json;
using FluentValidation;
using Jcd.Erp.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Npgsql;

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
        catch (DbUpdateException ex) when (TryMapUniqueConstraint(ex, out var domainError))
        {
            _logger.LogWarning(ex, "Unique constraint violation: {Error}", domainError);
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                type = "https://tools.ietf.org/html/rfc7807",
                title = "Conflict",
                status = 400,
                error = domainError,
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

    private static bool TryMapUniqueConstraint(DbUpdateException exception, out string domainError)
    {
        domainError = string.Empty;

        if (exception.InnerException is not PostgresException { SqlState: PostgresErrorCodes.UniqueViolation } postgres)
            return false;

        domainError = postgres.ConstraintName switch
        {
            "ix_roles_tenant_id_name" => "Role.NameAlreadyExists",
            "ix_users_tenant_id_email" => "User.EmailAlreadyExists",
            _ => string.Empty
        };

        return domainError != string.Empty;
    }
}
