using HtmlAnalyzer.BusinessLogic.Dtos;
using HtmlAnalyzer.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace HtmlAnalyzer.Controllers;

[ApiController]
[Route("api/analyze")]
public sealed class HtmlAnalyzerController(IHtmlAnalyzerService analyzerService) : ControllerBase
{
    [HttpPost]
    [Consumes("application/json")]
    public async Task<IActionResult> Analyze([FromBody] AnalyzeHtmlRequestDto? request, CancellationToken cancellationToken)
    {
        var result = await analyzerService.ProcessAsync(request, cancellationToken);
        
        if (result.IsFailed)
        {
            var message = string.Join("; ", result.Errors.Select(error => error.Message));
            return BadRequest(new AnalyzeHtmlResponseDto
            {
                IsError = 1,
                ErrorCode = GetErrorCode(message),
                ErrorMessage = message
            });
        }

        return Ok(new AnalyzeHtmlResponseDto
        {
            IsError = 0,
            Url = result.Value.Url,
            DecryptedPlainText = result.Value.DecryptedPlainText,
            ElementsCount = result.Value.ElementsCount,
            EmailsCount = result.Value.EmailsCount,
            ElementsAttrList = result.Value.ElementsAttrList,
            EmailsList = result.Value.EmailsList
        });
    }

    private static string GetErrorCode(string message) =>
        message.Contains("Base64", StringComparison.OrdinalIgnoreCase) ? "INVALID_BASE64" :
        message.Contains("UTF-8", StringComparison.OrdinalIgnoreCase) ? "INVALID_UTF8" :
        message.Contains("AES key", StringComparison.OrdinalIgnoreCase) ? "INVALID_AES_KEY" :
        message.Contains("encrypted text", StringComparison.OrdinalIgnoreCase) ? "INVALID_ENCRYPTED_TEXT" :
        message.Contains("selector", StringComparison.OrdinalIgnoreCase) ? "INVALID_SELECTOR" :
        "VALIDATION_ERROR";
}
