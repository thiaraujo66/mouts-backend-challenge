using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.WebApi.Common;
using FluentValidation;
using System.Text.Json;

namespace Ambev.DeveloperEvaluation.WebApi.Middleware;

/// <summary>
/// Central exception-to-HTTP-response mapping for the API, keeping the standard
/// <see cref="ApiResponse"/> error shape (type/error/detail per .doc/general-api.md)
/// for every failure mode: validation, business-rule violations, not-found and conflicts.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

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
            await WriteResponseAsync(context, StatusCodes.Status400BadRequest, "Validation Failed",
                ex.Errors.Select(error => (ValidationErrorDetail)error));
        }
        catch (DomainException ex)
        {
            await WriteResponseAsync(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            await WriteResponseAsync(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            await WriteResponseAsync(context, StatusCodes.Status409Conflict, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception while processing {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteResponseAsync(context, StatusCodes.Status500InternalServerError, "An unexpected error occurred");
        }
    }

    private static Task WriteResponseAsync(
        HttpContext context,
        int statusCode,
        string message,
        IEnumerable<ValidationErrorDetail>? errors = null)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = new ApiResponse
        {
            Success = false,
            Message = message,
            Errors = errors ?? []
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }
}
