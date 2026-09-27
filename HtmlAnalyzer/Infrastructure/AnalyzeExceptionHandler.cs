using System.Text.Json;
using HtmlAnalyzer.BusinessLogic.Dtos;
using Microsoft.AspNetCore.Diagnostics;

namespace HtmlAnalyzer.Infrastructure;

public sealed class AnalyzeExceptionHandler(
    ILogger<AnalyzeExceptionHandler> logger,
    IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception while processing an API request.");
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/json";

        var response = new AnalyzeHtmlResponseDto
        {
            IsError = 1,
            ErrorCode = "INTERNAL_ERROR",
            ErrorMessage = environment.IsDevelopment() ? exception.Message : "An unexpected error occurred."
        };

        await JsonSerializer.SerializeAsync(httpContext.Response.Body, response, cancellationToken: cancellationToken);
        return true;
    }
}
