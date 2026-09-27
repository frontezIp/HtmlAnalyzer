using System.Data;

namespace HtmlAnalyzer.DataAccess.Database;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
