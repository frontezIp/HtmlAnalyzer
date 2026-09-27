using HtmlAnalyzer.BusinessLogic;
using HtmlAnalyzer.BusinessLogic.Dtos;
using HtmlAnalyzer.DataAccess.Database;
using HtmlAnalyzer.Infrastructure;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var message = string.Join(
                "; ",
                context.ModelState.Values
                    .SelectMany(state => state.Errors)
                    .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                        ? "The request body is invalid."
                        : error.ErrorMessage));

            return new BadRequestObjectResult(new AnalyzeHtmlResponseDto
            {
                IsError = 1,
                ErrorCode = "INVALID_REQUEST",
                ErrorMessage = message
            });
        };
    });

builder.Services.AddHtmlAnalyzerBusinessLogic();
builder.Services.AddDapperDataAccess(builder.Configuration);
builder.Services.AddExceptionHandler<AnalyzeExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "HTML Analyzer API",
        Version = "v1"
    });
});

var app = builder.Build();

await app.Services.InitializeDatabaseAsync();
app.UseExceptionHandler();

app.UseSwagger(options =>
{
    options.RouteTemplate = "api/swagger/{documentName}/swagger.json";
});

app.UseSwaggerUI(options =>
{
    options.RoutePrefix = "api/swagger";
    options.SwaggerEndpoint("/api/swagger/v1/swagger.json", "HTML Analyzer API v1");
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();