using Dapper;
using HtmlAnalyzer.DataAccess.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HtmlAnalyzer.DataAccess.Database;

public static class DapperServiceCollectionExtensions
{
    private const string ConnectionStringName = "Postgres";

    public static IServiceCollection AddDapperDataAccess(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string '{ConnectionStringName}' is not configured.");
        }

        DefaultTypeMap.MatchNamesWithUnderscores = true;
        services.AddSingleton<IDbConnectionFactory>(
            new NpgsqlConnectionFactory(connectionString));

        services.AddSingleton<DatabaseInitializer>();
        services.AddScoped<IElementRepository, ElementRepository>();

        return services;
    }

    public static async Task InitializeDatabaseAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await services.GetRequiredService<DatabaseInitializer>()
            .InitializeAsync(cancellationToken);
    }
}