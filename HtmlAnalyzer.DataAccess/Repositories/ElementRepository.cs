using System.Data.Common;
using Dapper;
using HtmlAnalyzer.DataAccess.Database;
using HtmlAnalyzer.DataAccess.Entities;

namespace HtmlAnalyzer.DataAccess.Repositories;

public sealed class ElementRepository(IDbConnectionFactory connectionFactory) : IElementRepository
{
    private const string InsertSql = """
        INSERT INTO elements (value, html_code)
        VALUES (@Value, @HtmlCode)
        RETURNING id;
        """;

    public async Task<Element> AddAsync(
        Element element,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(element);

        using var connection = connectionFactory.CreateConnection();

        if (connection is not DbConnection dbConnection)
        {
            throw new InvalidOperationException("The configured connection must be a DbConnection.");
        }

        await dbConnection.OpenAsync(cancellationToken);

        element.Id = await dbConnection.ExecuteScalarAsync<long>(
            new CommandDefinition(InsertSql, element, cancellationToken: cancellationToken));

        return element;
    }
}
