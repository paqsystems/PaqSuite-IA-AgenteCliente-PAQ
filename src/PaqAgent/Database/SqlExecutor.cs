using System.Data;
using Microsoft.Data.SqlClient;

namespace PaqAgent.Database;

public sealed class SqlExecutor : ISqlExecutor
{
    public async Task ExecuteNonQueryAsync(
        string connectionString,
        string sql,
        int commandTimeoutSeconds,
        CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var command = new SqlCommand(sql, connection)
        {
            CommandTimeout = commandTimeoutSeconds
        };
        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<DataTable> QueryAsync(
        string connectionString,
        string sql,
        int commandTimeoutSeconds,
        CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var command = new SqlCommand(sql, connection)
        {
            CommandTimeout = commandTimeoutSeconds
        };
        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        var table = new DataTable();
        table.Load(reader);
        return table;
    }
}
