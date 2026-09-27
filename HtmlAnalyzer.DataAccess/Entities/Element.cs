namespace HtmlAnalyzer.DataAccess.Entities;

public sealed class Element
{
    public long Id { get; set; }

    public string Value { get; set; } = string.Empty;

    public string HtmlCode { get; set; } = string.Empty;
}
