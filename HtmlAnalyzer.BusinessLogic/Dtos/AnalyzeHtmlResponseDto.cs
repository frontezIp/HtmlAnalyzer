using System.Text.Json.Serialization;

namespace HtmlAnalyzer.BusinessLogic.Dtos;

public sealed class AnalyzeHtmlResponseDto
{
    [JsonPropertyName("is_error")] public int IsError { get; init; }
    [JsonPropertyName("error_code")] public string ErrorCode { get; init; } = string.Empty;
    [JsonPropertyName("error_message")] public string ErrorMessage { get; init; } = string.Empty;
    [JsonPropertyName("elements_count")] public int ElementsCount { get; init; }
    [JsonPropertyName("emails_count")] public int EmailsCount { get; init; }
    [JsonPropertyName("url")] public string Url { get; init; } = string.Empty;
    [JsonPropertyName("decrypted_plain_text")] public string DecryptedPlainText { get; init; } = string.Empty;
    [JsonPropertyName("elements_attr_list")] public IReadOnlyList<string> ElementsAttrList { get; init; } = [];
    [JsonPropertyName("emails_list")] public IReadOnlyList<string> EmailsList { get; init; } = [];
}
