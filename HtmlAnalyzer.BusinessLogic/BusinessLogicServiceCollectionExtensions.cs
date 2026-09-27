using FluentValidation;
using HtmlAnalyzer.BusinessLogic.Dtos;
using HtmlAnalyzer.BusinessLogic.Services;
using HtmlAnalyzer.BusinessLogic.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace HtmlAnalyzer.BusinessLogic;

public static class BusinessLogicServiceCollectionExtensions
{
    public static IServiceCollection AddHtmlAnalyzerBusinessLogic(this IServiceCollection services)
    {
        services.AddScoped<IValidator<AnalyzeHtmlRequestDto>, AnalyzeHtmlRequestValidator>();
        services.AddScoped<IHtmlAnalyzerService, HtmlAnalyzerService>();

        return services;
    }
}
