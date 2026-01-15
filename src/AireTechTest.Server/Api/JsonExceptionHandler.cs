using System.Text.Json;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AireTechTest.Server.Api;

/// <summary>
/// Handles JSON deserialization exceptions and returns appropriate ProblemDetails responses.
/// </summary>
public sealed class JsonExceptionHandler : IExceptionHandler
{
    private readonly ILogger<JsonExceptionHandler> _logger;

    public JsonExceptionHandler(ILogger<JsonExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Handle both direct JsonException and BadHttpRequestException wrapping JsonException
        JsonException? jsonException = exception as JsonException;

        if (jsonException is null && exception is BadHttpRequestException badRequestException)
        {
            jsonException = badRequestException.InnerException as JsonException;
        }

        if (jsonException is null)
        {
            return false;
        }

        _logger.LogWarning(
            exception,
            "JSON deserialization error for request {Method} {Path}",
            httpContext.Request.Method,
            httpContext.Request.Path);

        // Create errors dictionary matching validation problem format
        var errors = new Dictionary<string, string[]>();

        string fieldName = !string.IsNullOrEmpty(jsonException.Path)
            ? ConvertJsonPathToFieldName(jsonException.Path)
            : "request";

        errors[fieldName] = [GetUserFriendlyMessage(jsonException)];

        var problemDetails = new HttpValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "One or more validation errors occurred.",
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static string GetUserFriendlyMessage(JsonException exception)
    {
        string? path = exception.Path;

        if (exception.Message.Contains("could not be converted", StringComparison.OrdinalIgnoreCase))
        {
            if (!string.IsNullOrEmpty(path))
            {
                string fieldName = ConvertJsonPathToFieldName(path);
                return $"The value provided for '{fieldName}' is not in a valid format.";
            }
            return "A value in the request is not in a valid format.";
        }

        if (exception.Message.Contains("is invalid", StringComparison.OrdinalIgnoreCase))
        {
            if (!string.IsNullOrEmpty(path))
            {
                string fieldName = ConvertJsonPathToFieldName(path);
                return $"The value provided for '{fieldName}' is invalid.";
            }
            return "A value in the request is invalid.";
        }

        return "The request body contains invalid JSON.";
    }

    private static string ConvertJsonPathToFieldName(string jsonPath)
    {
        // Convert JSON path like "$.dateOfBirth" to "dateOfBirth"
        if (jsonPath.StartsWith("$."))
        {
            return jsonPath[2..];
        }
        if (jsonPath.StartsWith("$['") && jsonPath.EndsWith("']"))
        {
            return jsonPath[3..^2];
        }
        return jsonPath.TrimStart('$', '.');
    }
}
