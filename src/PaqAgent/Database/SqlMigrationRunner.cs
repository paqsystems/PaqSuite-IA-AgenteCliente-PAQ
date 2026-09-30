using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using PaqAgent.Options;

namespace PaqAgent.Database;

public sealed class SqlMigrationRunner : ISqlMigrationRunner
{
    private static readonly Regex GoBatchSeparator = new(
        @"^\s*GO(?:\s+\d+)?\s*(?:--.*)?$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

    private readonly ISqlExecutor sqlExecutor;
    private readonly SqlScriptLoader scriptLoader;
    private readonly ILogger<SqlMigrationRunner> logger;

    public SqlMigrationRunner(
        ISqlExecutor sqlExecutor,
        SqlScriptLoader scriptLoader,
        ILogger<SqlMigrationRunner> logger)
    {
        this.sqlExecutor = sqlExecutor;
        this.scriptLoader = scriptLoader;
        this.logger = logger;
    }

    public async Task RunAsync(AgentOptions options, CancellationToken cancellationToken)
    {
        if (!options.SqlMigrations.Enabled)
        {
            logger.LogWarning("SqlMigrationRunner deshabilitado por configuración.");
            return;
        }

        if (!options.HasSqlConfig)
        {
            throw new InvalidOperationException("No hay conexión SQL de diccionario configurada.");
        }

        var timeout = Math.Max(1, options.SqlMigrations.CommandTimeoutSeconds);
        var dictionaryConnection = SqlConnectionStringFactory.Build(options.Sql, timeout);
        var scripts = scriptLoader.Load();

        try
        {
            await ApplyPhaseAsync(
                "dictionary",
                dictionaryConnection,
                scripts.Where(script => script.Phase.Equals("dictionary", StringComparison.OrdinalIgnoreCase)),
                timeout,
                cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            logger.LogError("Falló la fase dictionary; se aborta el arranque del agente.");
            throw;
        }

        var companyScripts = scripts
            .Where(script => script.Phase.Equals("company", StringComparison.OrdinalIgnoreCase))
            .ToArray();
        if (companyScripts.Length == 0)
        {
            return;
        }

        foreach (var database in await GetEnabledCompanyDatabasesAsync(
                     dictionaryConnection,
                     timeout,
                     cancellationToken).ConfigureAwait(false))
        {
            try
            {
                var companyConnection = SqlConnectionStringFactory.Build(options.Sql, timeout, database);
                await ApplyPhaseAsync(
                    "company",
                    companyConnection,
                    companyScripts,
                    timeout,
                    cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                logger.LogWarning(
                    exception,
                    "No se pudieron aplicar migraciones company en {Database}; se continúa.",
                    database);
            }
        }
    }

    public static IReadOnlyList<string> SplitSqlBatches(string sql)
    {
        return GoBatchSeparator.Split(sql)
            .Select(batch => batch.Trim())
            .Where(batch => batch.Length > 0)
            .ToArray();
    }

    public static string ComputeSha256Hex(string content)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(content)));
    }

    private async Task ApplyPhaseAsync(
        string phase,
        string connectionString,
        IEnumerable<SqlMigrationScript> scripts,
        int timeout,
        CancellationToken cancellationToken)
    {
        await sqlExecutor.ExecuteNonQueryAsync(
            connectionString,
            SqlMigrationSchema.EnsureTableSql,
            timeout,
            cancellationToken).ConfigureAwait(false);

        var applied = await sqlExecutor.QueryAsync(
            connectionString,
            "SELECT migration, checksum_sha256 FROM dbo.paq_sp_migrations;",
            timeout,
            cancellationToken).ConfigureAwait(false);
        var appliedByName = applied.Rows.Cast<DataRow>()
            .ToDictionary(
                row => Convert.ToString(row["migration"]) ?? string.Empty,
                row => Convert.ToString(row["checksum_sha256"]) ?? string.Empty,
                StringComparer.OrdinalIgnoreCase);

        var batch = await GetNextBatchAsync(connectionString, timeout, cancellationToken).ConfigureAwait(false);
        foreach (var script in scripts)
        {
            var checksum = ComputeSha256Hex(script.Content);
            if (appliedByName.TryGetValue(script.Migration, out var storedChecksum))
            {
                if (!storedChecksum.Equals(checksum, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        $"La migración {script.Migration} ya fue aplicada con otro checksum.");
                }

                logger.LogDebug("Migración omitida phase={Phase} migration={Migration}", phase, script.Migration);
                continue;
            }

            foreach (var sqlBatch in SplitSqlBatches(script.Content))
            {
                await sqlExecutor.ExecuteNonQueryAsync(
                    connectionString,
                    sqlBatch,
                    timeout,
                    cancellationToken).ConfigureAwait(false);
            }

            var migration = EscapeSqlLiteral(script.Migration);
            await sqlExecutor.ExecuteNonQueryAsync(
                connectionString,
                $"""
                INSERT INTO dbo.paq_sp_migrations
                    (migration, batch, applied_at, checksum_sha256)
                VALUES
                    (N'{migration}', {batch}, SYSUTCDATETIME(), '{checksum}');
                """,
                timeout,
                cancellationToken).ConfigureAwait(false);
            appliedByName[script.Migration] = checksum;
            logger.LogInformation("Migración aplicada phase={Phase} migration={Migration}", phase, script.Migration);
        }
    }

    private async Task<int> GetNextBatchAsync(
        string connectionString,
        int timeout,
        CancellationToken cancellationToken)
    {
        var result = await sqlExecutor.QueryAsync(
            connectionString,
            "SELECT ISNULL(MAX(batch), 0) + 1 AS next_batch FROM dbo.paq_sp_migrations;",
            timeout,
            cancellationToken).ConfigureAwait(false);
        return Convert.ToInt32(result.Rows[0]["next_batch"]);
    }

    private async Task<IReadOnlyList<string>> GetEnabledCompanyDatabasesAsync(
        string dictionaryConnection,
        int timeout,
        CancellationToken cancellationToken)
    {
        var columns = await sqlExecutor.QueryAsync(
            dictionaryConnection,
            """
            SELECT name
            FROM sys.columns
            WHERE object_id = OBJECT_ID(N'dbo.pq_empresa');
            """,
            timeout,
            cancellationToken).ConfigureAwait(false);
        var names = columns.Rows.Cast<DataRow>()
            .Select(row => Convert.ToString(row["name"]) ?? string.Empty)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var databaseColumn = names.Contains("NombreBD") ? "NombreBD" : names.Contains("nombre_bd") ? "nombre_bd" : null;
        var enabledColumn = names.Contains("Habilita") ? "Habilita" : names.Contains("habilita") ? "habilita" : null;
        if (databaseColumn is null || enabledColumn is null)
        {
            throw new InvalidOperationException("pq_empresa no contiene columnas NombreBD/nombre_bd y Habilita/habilita.");
        }

        var rows = await sqlExecutor.QueryAsync(
            dictionaryConnection,
            $"SELECT [{databaseColumn}] AS database_name FROM dbo.pq_empresa WHERE [{enabledColumn}] = 1;",
            timeout,
            cancellationToken).ConfigureAwait(false);
        return rows.Rows.Cast<DataRow>()
            .Select(row => Convert.ToString(row["database_name"])?.Trim() ?? string.Empty)
            .Where(database => database.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static string EscapeSqlLiteral(string value) => value.Replace("'", "''", StringComparison.Ordinal);
}
