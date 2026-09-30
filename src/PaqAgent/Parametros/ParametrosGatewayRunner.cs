using System.Text.Json;
using PaqAgent.Options;
using PaqAgent.Informes;
using PaqContracts;

namespace PaqAgent.Parametros;

public sealed class ParametrosOutcome
{
    public string Status { get; init; } = JobStatuses.Failed;
    public object? Data { get; init; }
    public string? ErrorCode { get; init; }
    public string? ErrorMessage { get; init; }
}

public sealed class ParametrosGatewayRunner
{
    private const string StoredProcedure = "dbo.PAQ_ParametrosGral_Update";
    private readonly IInformesSpExecutor companySpExecutor;

    public ParametrosGatewayRunner(IInformesSpExecutor companySpExecutor)
    {
        this.companySpExecutor = companySpExecutor;
    }

    public async Task<ParametrosOutcome> RunAsync(
        AgentOptions agentOptions,
        IReadOnlyDictionary<string, object?> parameters,
        int timeoutSeconds,
        CancellationToken cancellationToken)
    {
        var programa = GetString(parameters, "programa");
        var clave = GetString(parameters, "clave");
        var idempotencyKey = GetString(parameters, "idempotencyKey");
        var databaseOverride = GetString(parameters, "_database");

        if (string.IsNullOrWhiteSpace(programa)
            || string.IsNullOrWhiteSpace(clave)
            || string.IsNullOrWhiteSpace(idempotencyKey)
            || string.IsNullOrWhiteSpace(databaseOverride))
        {
            return Fail("INVALID_PARAMETERS", "programa, clave, _database e idempotencyKey son obligatorios.");
        }

        if (!agentOptions.HasSqlConfig)
        {
            return new ParametrosOutcome
            {
                Status = JobStatuses.Degraded,
                ErrorCode = "SQL_NOT_CONFIGURED",
                ErrorMessage = "sql.server/database no configurados en appsettings.local.json"
            };
        }

        try
        {
            var connectionString = SqlConnectionStringFactory.Build(
                agentOptions.Sql,
                connectTimeoutSeconds: 15,
                databaseOverride: databaseOverride);
            var resultSets = await companySpExecutor.ExecuteAsync(
                connectionString,
                StoredProcedure,
                new Dictionary<string, object?>
                {
                    ["Programa"] = programa,
                    ["Clave"] = clave,
                    ["Valor"] = NormalizeValue(parameters.TryGetValue("valor", out var value) ? value : null),
                    ["IdempotencyKey"] = idempotencyKey,
                    ["CompanyId"] = parameters.TryGetValue("companyId", out var companyId) ? companyId : null
                },
                timeoutSeconds,
                cancellationToken).ConfigureAwait(false);

            var row = resultSets.FirstOrDefault()?.FirstOrDefault();
            if (row is null)
            {
                return Fail("NOT_FOUND", "Parámetro no encontrado.");
            }

            var resultCode = GetString(row, "resultCode");
            if (!string.Equals(resultCode, "OK", StringComparison.OrdinalIgnoreCase))
            {
                return Fail(resultCode ?? "PARAMETRO_UPDATE_FAILED", GetString(row, "errorMessage") ?? "No se pudo actualizar el parámetro.");
            }

            return new ParametrosOutcome
            {
                Status = JobStatuses.Success,
                Data = new Dictionary<string, object?>
                {
                    ["programa"] = GetString(row, "programa"),
                    ["clave"] = GetString(row, "clave"),
                    ["tipoValor"] = GetString(row, "tipoValor"),
                    ["valor"] = row.TryGetValue("valor", out var updatedValue) ? updatedValue : null,
                    ["idempotencyKey"] = idempotencyKey
                }
            };
        }
        catch (Exception exception)
        {
            return Fail("SQL_ERROR", exception.GetType().Name + ": " + exception.Message);
        }
    }

    private static ParametrosOutcome Fail(string errorCode, string errorMessage) =>
        new()
        {
            Status = JobStatuses.Failed,
            ErrorCode = errorCode,
            ErrorMessage = errorMessage
        };

    private static string? GetString(IReadOnlyDictionary<string, object?> values, string key) =>
        values.TryGetValue(key, out var value) && value is not null ? value.ToString() : null;

    private static object? NormalizeValue(object? value) =>
        value is JsonElement jsonElement
            ? jsonElement.ValueKind switch
            {
                JsonValueKind.Null => null,
                JsonValueKind.String => jsonElement.GetString(),
                _ => jsonElement.GetRawText()
            }
            : value;
}
