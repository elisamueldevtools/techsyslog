using System.Net;
using System.Text.Json;
using TechsysLog.Application.Common.Errors;
using TechsysLog.Domain.Exceptions;
using AppValidationException = TechsysLog.Application.Common.Exceptions.ValidationException;

namespace TechsysLog.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (AppValidationException ex)
        {
            await WriteAsync(ctx, HttpStatusCode.BadRequest, ValidationProblem.FromFluent(ex.Failures));
        }
        catch (ConflictException ex)
        {
            await WriteAsync(ctx, HttpStatusCode.Conflict, new { type = "conflict", message = ex.Message });
        }
        catch (NotFoundException ex)
        {
            await WriteAsync(ctx, HttpStatusCode.NotFound, new { type = "not_found", message = ex.Message });
        }
        catch (UnauthorizedException ex)
        {
            await WriteAsync(ctx, HttpStatusCode.Unauthorized, new { type = "unauthorized", message = ex.Message });
        }
        catch (DomainException ex)
        {
            await WriteAsync(ctx, HttpStatusCode.BadRequest, new { type = "domain_error", message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteAsync(ctx, HttpStatusCode.InternalServerError, new { type = "internal_error", message = "Internal server error." });
        }
    }

    private static Task WriteAsync(HttpContext ctx, HttpStatusCode status, object payload)
    {
        ctx.Response.StatusCode = (int)status;
        ctx.Response.ContentType = "application/json";
        return ctx.Response.WriteAsync(JsonSerializer.Serialize(payload, JsonOptions));
    }
}
