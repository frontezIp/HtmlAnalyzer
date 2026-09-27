using System.Text.Json.Serialization;

namespace HtmlAnalyzer.BusinessLogic.Dtos;

public sealed class AnalyzeHtmlRequestDto
{
    [JsonPropertyName("selector")]
    public string? Selector { get; init; }

    [JsonPropertyName("attribute")]
    public string? Attribute { get; init; }

    [JsonPropertyName("url_b64")]
    public string? UrlB64 { get; init; }

    [JsonPropertyName("encrypted_text_bytes_b64")]
    public string? EncryptedTextBytesB64 { get; init; }

    [JsonPropertyName("key_bytes_b64")]
    public string? KeyBytesB64 { get; init; }

    [JsonPropertyName("page_b64")]
    public string? PageB64 { get; init; }
}
