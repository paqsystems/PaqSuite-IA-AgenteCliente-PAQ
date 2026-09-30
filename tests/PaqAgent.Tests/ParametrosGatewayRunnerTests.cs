using PaqAgent.Informes;
using PaqAgent.Options;
using PaqAgent.Parametros;
using PaqContracts;

namespace PaqAgent.Tests;

public sealed class ParametrosGatewayRunnerTests
{
    [Fact]
    public async Task RunAsync_rechaza_payload_incompleto_sin_ejecutar_sql()
    {
        var executor = new RecordingExecutor();
        var runner = new ParametrosGatewayRunner(executor);

        var result = await runner.RunAsync(
            new AgentOptions { Sql = new SqlOptions { Server = "server", Database = "company" } },
            new Dictionary<string, object?> { ["programa"] = "Acopios" },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, result.Status);
        Assert.Equal("INVALID_PARAMETERS", result.ErrorCode);
        Assert.Equal(0, executor.CallCount);
    }

    [Fact]
    public async Task RunAsync_devuelve_parametro_actualizado()
    {
        var executor = new RecordingExecutor
        {
            Result = [[new Dictionary<string, object?>
            {
                ["resultCode"] = "OK",
                ["programa"] = "Acopios",
                ["clave"] = "PrefijoArticulo",
                ["tipoValor"] = "S",
                ["valor"] = "AC2"
            }]]
        };
        var runner = new ParametrosGatewayRunner(executor);

        var result = await runner.RunAsync(
            new AgentOptions { Sql = new SqlOptions { Server = "server", Database = "company" } },
            new Dictionary<string, object?>
            {
                ["programa"] = "Acopios",
                ["clave"] = "PrefijoArticulo",
                ["_database"] = "ROBINET",
                ["valor"] = "AC2",
                ["idempotencyKey"] = "idem-1"
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, result.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(result.Data);
        Assert.Equal("AC2", data["valor"]);
        Assert.Equal(1, executor.CallCount);
        Assert.Equal("dbo.PAQ_ParametrosGral_Update", executor.StoredProcedure);
    }

    private sealed class RecordingExecutor : IInformesSpExecutor
    {
        public int CallCount { get; private set; }
        public string? StoredProcedure { get; private set; }
        public IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> Result { get; init; } = [];

        public Task<IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>>> ExecuteAsync(
            string connectionString,
            string storedProcedure,
            IReadOnlyDictionary<string, object?> spParameters,
            int timeoutSeconds,
            CancellationToken cancellationToken)
        {
            CallCount++;
            StoredProcedure = storedProcedure;

            return Task.FromResult(Result);
        }
    }
}
