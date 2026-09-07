using System.Diagnostics.CodeAnalysis;
using GarageFlowService.Domain.Exceptions;
using System.Text.Json;

namespace GarageFlowService.API.Middleware;

[ExcludeFromCodeCoverage]
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse();

        switch (exception)
        {
            case DomainException domainException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = domainException.Message;
                response.ErrorCode = "DOMAIN_ERROR";
                break;

            case ArgumentException argException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = argException.Message;
                response.ErrorCode = "INVALID_ARGUMENT";
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = "Ocorreu um erro interno no servidor. Por favor, tente novamente mais tarde.";
                response.ErrorCode = "INTERNAL_ERROR";
                break;
        }

        return context.Response.WriteAsJsonAsync(response);
    }
}

public class ErrorResponse
{
    public string ErrorCode { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
