using HtmlAnalyzer.DataAccess.Entities;

namespace HtmlAnalyzer.DataAccess.Repositories;

public interface IElementRepository
{
    Task<Element> AddAsync(
        Element element,
        CancellationToken cancellationToken = default);
}
