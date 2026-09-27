using FluentResults;
using HtmlAnalyzer.BusinessLogic.Dtos;
using HtmlAnalyzer.BusinessLogic.Results;

namespace HtmlAnalyzer.BusinessLogic.Services;

public interface IHtmlAnalyzerService
{
    Task<Result<DecodedAnalyzeHtml>> ProcessAsync(AnalyzeHtmlRequestDto? request, CancellationToken cancellationToken = default);
}
