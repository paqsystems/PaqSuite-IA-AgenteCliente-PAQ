using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PaqAgent.Database;
using PaqAgent.Options;
using PaqContracts;

namespace PaqAgent.Installation;

public sealed class InstallationGatewayRunner
{
    private readonly ISqlExecutor sqlExecutor;
    private readonly ILogger<InstallationGatewayRunner> logger;

    public InstallationGatewayRunner(ISqlExecutor sqlExecutor, ILogger<InstallationGatewayRunner> logger)
    {
        this.sqlExecutor = sqlExecutor;
        this.logger = logger;
    }

    public Task<InstallationOutcome> RunInstallOrUpdateStepAsync(
        AgentOptions agentOptions,
        IReadOnlyDictionary<string, object?> parameters,
        int timeoutSeconds,
        CancellationToken cancellationToken)
    {
        return ApplyManifestStepAsync(agentOptions, parameters, timeoutSeconds, cancellationToken);
    }

    public async Task<InstallationOutcome> RunCompanyProvisionAsync(
        AgentOptions agentOptions,
        IReadOnlyDictionary<string, object?> parameters,
        int timeoutSeconds,
        CancellationToken cancellationToken)
    {
        if (!agentOptions.HasSqlConfig)
        {
            return Degraded("SQL_NOT_CONFIGURED", "sql.server/database no configurados en appsettings.local.json");
        }

        var mode = InstallationParameters.GetString(parameters, "mode");
        if (string.IsNullOrWhiteSpace(mode))
        {
            return Fail("INVALID_PARAMETERS", "mode es obligatorio (initial | additional).");
        }

        var timeout = Math.Max(1, timeoutSeconds);
        var dictionaryConnection = SqlConnectionStringFactory.Build(agentOptions.Sql, timeout);

        if (string.Equals(mode, "initial", StringComparison.OrdinalIgnoreCase))
        {
            return await LoadFirstEmpresaAsync(dictionaryConnection, timeout, cancellationToken).ConfigureAwait(false);
        }

        if (string.Equals(mode, "additional", StringComparison.OrdinalIgnoreCase))
        {
            return Fail(
                "PROVISION_ADDITIONAL_NOT_SUPPORTED",
                "company.provision additional en agente queda pendiente de SP de alta ERP; use modo directo en host.");
        }

        return Fail("INVALID_PARAMETERS", $"mode desconocido: {mode}");
    }

    private async Task<InstallationOutcome> ApplyManifestStepAsync(
        AgentOptions agentOptions,
        IReadOnlyDictionary<string, object?> parameters,
        int timeoutSeconds,
        CancellationToken cancellationToken)
    {
        if (!agentOptions.HasSqlConfig)
        {
            return Degraded("SQL_NOT_CONFIGURED", "sql.server/database no configurados en appsettings.local.json");
        }

        var packRoot = agentOptions.Installation.PackRoot?.Trim();
        if (string.IsNullOrWhiteSpace(packRoot))
        {
            return Fail(
                "INSTALLATION_PACK_ROOT_MISSING",
                "installation.packRoot debe apuntar al release train desplegado en el servidor del agente.");
        }

        if (!InstallationParameters.TryGetObject(parameters, "step", out var stepEl)
            || !InstallationParameters.TryGetObject(parameters, "artifact", out var artifactEl))
        {
            return Fail("INVALID_PARAMETERS", "step y artifact son obligatorios.");
        }

        var stepId = InstallationParameters.GetString(stepEl, "id");
        var kind = InstallationParameters.GetString(stepEl, "kind");
        var target = InstallationParameters.GetString(stepEl, "target");
        var stepPath = InstallationParameters.GetString(stepEl, "path");
        var stepChecksum = InstallationParameters.GetString(stepEl, "checksum")?.ToLowerInvariant();
        var artifactPath = InstallationParameters.GetString(artifactEl, "path");
        var artifactChecksum = InstallationParameters.GetString(artifactEl, "checksum")?.ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(stepId)
            || string.IsNullOrWhiteSpace(kind)
            || string.IsNullOrWhiteSpace(target)
            || string.IsNullOrWhiteSpace(stepPath)
            || string.IsNullOrWhiteSpace(stepChecksum)
            || string.IsNullOrWhiteSpace(artifactPath)
            || string.IsNullOrWhiteSpace(artifactChecksum))
        {
            return Fail("INVALID_PARAMETERS", "step/artifact incompletos (id, kind, target, path, checksum).");
        }

        if (!string.Equals(stepPath, artifactPath, StringComparison.OrdinalIgnoreCase)
            || !string.Equals(stepChecksum, artifactChecksum, StringComparison.Ordinal))
        {
            return Fail("ARTIFACT_MISMATCH", "step.path/checksum no coincide con artifact.");
        }

        if (!string.Equals(kind, "sql", StringComparison.OrdinalIgnoreCase))
        {
            return Fail("ARTIFACT_KIND_UNSUPPORTED", $"kind '{kind}' no ejecutable en agente (solo sql).");
        }

        var absolutePath = Path.GetFullPath(Path.Combine(packRoot, artifactPath.Replace('/', Path.DirectorySeparatorChar)));
        var normalizedRoot = Path.GetFullPath(packRoot);
        if (!absolutePath.StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase))
        {
            return Fail("ARTIFACT_PATH_ESCAPE", "path del artefacto sale del packRoot.");
        }

        if (!File.Exists(absolutePath))
        {
            return Fail("ARTIFACT_MISSING", $"No existe el archivo en packRoot: {artifactPath}");
        }

        var fileChecksum = ComputeSha256HexFromFile(absolutePath);
        if (!fileChecksum.Equals(artifactChecksum, StringComparison.OrdinalIgnoreCase))
        {
            return Fail("ARTIFACT_CHECKSUM_MISMATCH", $"Checksum distinto para {artifactPath}.");
        }

        var sqlContent = await File.ReadAllTextAsync(absolutePath, Encoding.UTF8, cancellationToken).ConfigureAwait(false);
        var timeout = Math.Max(1, timeoutSeconds);
        var companyIds = InstallationParameters.GetInt32List(parameters, "companyIds");
        var runId = InstallationParameters.GetString(parameters, "runId");
        var manifestId = InstallationParameters.GetString(parameters, "manifestId");

        try
        {
            if (string.Equals(target, "dictionary", StringComparison.OrdinalIgnoreCase))
            {
                var connection = SqlConnectionStringFactory.Build(agentOptions.Sql, timeout);
                await ApplySqlStepOnConnectionAsync(
                    connection,
                    stepId,
                    sqlContent,
                    timeout,
                    cancellationToken).ConfigureAwait(false);
            }
            else if (string.Equals(target, "company", StringComparison.OrdinalIgnoreCase))
            {
                var dictionaryConnection = SqlConnectionStringFactory.Build(agentOptions.Sql, timeout);
                var databases = await ResolveCompanyDatabasesAsync(
                    dictionaryConnection,
                    companyIds,
                    timeout,
                    cancellationToken).ConfigureAwait(false);
                if (databases.Count == 0)
                {
                    return Fail("COMPANY_DATABASES_EMPTY", "No hay bases company para aplicar el paso.");
                }

                foreach (var database in databases)
                {
                    var companyConnection = SqlConnectionStringFactory.Build(agentOptions.Sql, timeout, database);
                    await ApplySqlStepOnConnectionAsync(
                        companyConnection,
                        stepId,
                        sqlContent,
                        timeout,
                        cancellationToken).ConfigureAwait(false);
                }
            }
            else
            {
                return Fail("INVALID_PARAMETERS", $"target desconocido: {target}");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falló paso installation stepId={StepId} runId={RunId}", stepId, runId);
            return Fail("INSTALLATION_STEP_FAILED", ex.Message);
        }

        return new InstallationOutcome
        {
            Status = JobStatuses.Success,
            Data = new Dictionary<string, object?>
            {
                ["stepId"] = stepId,
                ["runId"] = runId,
                ["manifestId"] = manifestId,
                ["target"] = target,
                ["path"] = artifactPath,
            }
        };
    }

    private async Task ApplySqlStepOnConnectionAsync(
        string connectionString,
        string migrationId,
        string sqlContent,
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

        var checksum = SqlMigrationRunner.ComputeSha256Hex(sqlContent);
        if (appliedByName.TryGetValue(migrationId, out var storedChecksum))
        {
            if (!storedChecksum.Equals(checksum, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"La migración {migrationId} ya fue aplicada con otro checksum.");
            }

            logger.LogDebug("Paso installation omitido (ya aplicado) migration={Migration}", migrationId);
            return;
        }

        foreach (var batch in SqlMigrationRunner.SplitSqlBatches(sqlContent))
        {
            await sqlExecutor.ExecuteNonQueryAsync(connectionString, batch, timeout, cancellationToken)
                .ConfigureAwait(false);
        }

        var batchNo = await GetNextBatchAsync(connectionString, timeout, cancellationToken).ConfigureAwait(false);
        var migrationLiteral = migrationId.Replace("'", "''", StringComparison.Ordinal);
        await sqlExecutor.ExecuteNonQueryAsync(
            connectionString,
            $"""
            INSERT INTO dbo.paq_sp_migrations
                (migration, batch, applied_at, checksum_sha256)
            VALUES
                (N'{migrationLiteral}', {batchNo}, SYSUTCDATETIME(), '{checksum}');
            """,
            timeout,
            cancellationToken).ConfigureAwait(false);

        logger.LogInformation("Paso installation aplicado migration={Migration}", migrationId);
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

    private async Task<IReadOnlyList<string>> ResolveCompanyDatabasesAsync(
        string dictionaryConnection,
        IReadOnlyList<int> companyIds,
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
        var idColumn = names.Contains("id") ? "id" : names.Contains("Id") ? "Id" : null;
        if (databaseColumn is null || enabledColumn is null)
        {
            throw new InvalidOperationException("pq_empresa no contiene columnas NombreBD/nombre_bd y Habilita/habilita.");
        }

        var sql = companyIds.Count > 0 && idColumn is not null
            ? $"""
               SELECT [{databaseColumn}] AS database_name
               FROM dbo.pq_empresa
               WHERE [{enabledColumn}] = 1
                 AND [{idColumn}] IN ({string.Join(", ", companyIds)});
               """
            : $"""
               SELECT [{databaseColumn}] AS database_name
               FROM dbo.pq_empresa
               WHERE [{enabledColumn}] = 1;
               """;

        var rows = await sqlExecutor.QueryAsync(dictionaryConnection, sql, timeout, cancellationToken)
            .ConfigureAwait(false);
        return rows.Rows.Cast<DataRow>()
            .Select(row => Convert.ToString(row["database_name"])?.Trim() ?? string.Empty)
            .Where(database => database.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private async Task<InstallationOutcome> LoadFirstEmpresaAsync(
        string dictionaryConnection,
        int timeout,
        CancellationToken cancellationToken)
    {
        var table = await sqlExecutor.QueryAsync(
            dictionaryConnection,
            "SELECT TOP (1) * FROM dbo.pq_empresa ORDER BY id;",
            timeout,
            cancellationToken).ConfigureAwait(false);
        if (table.Rows.Count == 0)
        {
            return Fail(
                "PROVISION_INITIAL_EMPTY",
                "No hay empresas en pq_empresa; complete bootstrap antes de install initial.");
        }

        var row = table.Rows[0];
        var data = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        foreach (DataColumn column in table.Columns)
        {
            var value = row[column];
            data[ToCamelCase(column.ColumnName)] = value == DBNull.Value ? null : value;
        }

        return new InstallationOutcome
        {
            Status = JobStatuses.Success,
            Data = data
        };
    }

    private static string ComputeSha256HexFromFile(string absolutePath)
    {
        return Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(absolutePath)));
    }

    private static string ToCamelCase(string columnName)
    {
        if (string.IsNullOrEmpty(columnName))
        {
            return columnName;
        }

        if (columnName.Length == 1)
        {
            return columnName.ToLowerInvariant();
        }

        return char.ToLowerInvariant(columnName[0]) + columnName[1..];
    }

    private static InstallationOutcome Fail(string code, string message) =>
        new() { Status = JobStatuses.Failed, ErrorCode = code, ErrorMessage = message };

    private static InstallationOutcome Degraded(string code, string message) =>
        new() { Status = JobStatuses.Degraded, ErrorCode = code, ErrorMessage = message };
}

internal static class InstallationParameters
{
    public static string? GetString(IReadOnlyDictionary<string, object?> parameters, string key)
    {
        return parameters.TryGetValue(key, out var raw) ? CoerceString(raw) : null;
    }

    public static string? GetString(JsonElement element, string key)
    {
        if (!element.TryGetProperty(key, out var prop))
        {
            return null;
        }

        return prop.ValueKind switch
        {
            JsonValueKind.String => prop.GetString(),
            JsonValueKind.Number => prop.GetRawText(),
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            _ => null
        };
    }

    public static bool TryGetObject(IReadOnlyDictionary<string, object?> parameters, string key, out JsonElement element)
    {
        element = default;
        if (!parameters.TryGetValue(key, out var raw))
        {
            return false;
        }

        if (raw is JsonElement je && je.ValueKind == JsonValueKind.Object)
        {
            element = je;
            return true;
        }

        if (raw is Dictionary<string, object?> dict)
        {
            var json = JsonSerializer.Serialize(dict);
            using var doc = JsonDocument.Parse(json);
            element = doc.RootElement.Clone();
            return element.ValueKind == JsonValueKind.Object;
        }

        return false;
    }

    public static IReadOnlyList<int> GetInt32List(IReadOnlyDictionary<string, object?> parameters, string key)
    {
        if (!parameters.TryGetValue(key, out var raw) || raw is null)
        {
            return Array.Empty<int>();
        }

        if (raw is JsonElement { ValueKind: JsonValueKind.Array } array)
        {
            var list = new List<int>();
            foreach (var item in array.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.Number && item.TryGetInt32(out var n))
                {
                    list.Add(n);
                }
            }

            return list;
        }

        return Array.Empty<int>();
    }

    private static string? CoerceString(object? raw)
    {
        return raw switch
        {
            null => null,
            string s => s,
            JsonElement { ValueKind: JsonValueKind.String } je => je.GetString(),
            JsonElement { ValueKind: JsonValueKind.Number } je => je.GetRawText(),
            _ => raw.ToString()
        };
    }
}
