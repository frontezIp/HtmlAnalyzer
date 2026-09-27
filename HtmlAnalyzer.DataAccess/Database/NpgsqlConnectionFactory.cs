using System.Data;
using Npgsql;

namespace HtmlAnalyzer.DataAccess.Database;

internal sealed class NpgsqlConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public IDbConnection CreateConnection() => new NpgsqlConnection(connectionString);
}
