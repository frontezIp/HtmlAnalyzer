using System.Reflection;
using Dapper;

namespace HtmlAnalyzer.DataAccess.Database;

internal sealed class DatabaseInitializer(IDbConnectionFactory connectionFactory)
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();

        if (connection is not System.Data.Common.DbConnection dbConnection)
        {
            throw new InvalidOperationException("The configured connection must be a DbConnection.");
        }

        await dbConnection.OpenAsync(cancellationToken);

        var scripts = typeof(DatabaseInitializer).Assembly
            .GetManifestResourceNames()
            .Where(name => name.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase);

        foreach (var script in scripts)
        {
            await using var stream = typeof(DatabaseInitializer).Assembly
                .GetManifestResourceStream(script)
                ?? throw new InvalidOperationException($"Unable to load database script '{script}'.");

            using var reader = new StreamReader(stream);
            var sql = await reader.ReadToEndAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(sql))
            {
                continue;
            }

            await dbConnection.ExecuteAsync(
                new CommandDefinition(sql, cancellationToken: cancellationToken));
        }
    }
}
