using System.Data;

namespace PaqAgent.Database;

public interface ISqlExecutor
{
    Task ExecuteNonQueryAsync(
        string connectionString,
        string sql,
        int commandTimeoutSeconds,
        CancellationToken cancellationToken);

    Task<DataTable> QueryAsync(
        string connectionString,
        string sql,
        int commandTimeoutSeconds,
        CancellationToken cancellationToken);
}
