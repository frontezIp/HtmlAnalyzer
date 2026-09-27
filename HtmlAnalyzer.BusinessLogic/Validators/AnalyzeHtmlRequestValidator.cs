using FluentValidation;
using HtmlAnalyzer.BusinessLogic.Dtos;

namespace HtmlAnalyzer.BusinessLogic.Validators;

public sealed class AnalyzeHtmlRequestValidator : AbstractValidator<AnalyzeHtmlRequestDto>
{
    public AnalyzeHtmlRequestValidator()
    {
        RuleFor(request => request.Selector).NotEmpty().WithMessage("The selector is required.");
        RuleFor(request => request.Attribute).NotEmpty().WithMessage("The attribute is required.");
        RuleFor(request => request.UrlB64).NotEmpty().WithMessage("The Base64 URL is required.");
        RuleFor(request => request.PageB64).NotEmpty().WithMessage("The Base64 page is required.");
        RuleFor(request => request.EncryptedTextBytesB64).NotEmpty().WithMessage("The encrypted text is required.");
        RuleFor(request => request.KeyBytesB64).NotEmpty().WithMessage("The AES key is required.");
    }
}
