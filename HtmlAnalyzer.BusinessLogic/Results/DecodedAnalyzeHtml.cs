using System.Text.Json.Serialization;

namespace HtmlAnalyzer.BusinessLogic.Results;

public sealed class DecodedAnalyzeHtml(
    string url,
    int elementsCount,
    int emailsCount,
    IReadOnlyList<string> elementsAttrList,
    IReadOnlyList<string> emailsList,
    string decryptedPlainText)
{
    [JsonPropertyName("url")]
    public string Url { get; } = url;

    [JsonPropertyName("elements_count")]
    public int ElementsCount { get; } = elementsCount;

    [JsonPropertyName("emails_count")]
    public int EmailsCount { get; } = emailsCount;

    [JsonPropertyName("elements_attr_list")]
    public IReadOnlyList<string> ElementsAttrList { get; } = elementsAttrList;

    [JsonPropertyName("emails_list")]
    public IReadOnlyList<string> EmailsList { get; } = emailsList;

    [JsonPropertyName("decrypted_plain_text")]
    public string DecryptedPlainText { get; } = decryptedPlainText;
}
