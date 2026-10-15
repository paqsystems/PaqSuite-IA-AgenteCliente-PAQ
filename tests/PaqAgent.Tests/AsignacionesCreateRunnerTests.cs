using PaqAgent.Options;
using PaqAgent.Partes;
using PaqContracts;

namespace PaqAgent.Tests;

public class AsignacionesCreateRunnerTests
{
    [Fact]
    public void Catalog_containsCreateOrchestrated()
    {
        Assert.True(PartesCatalog.TryGet("PartesProduccion.Asignaciones.Create", out var def));
        Assert.Equal(PartesResponseShape.AsignacionCreate, def.Shape);
        Assert.Equal("(orchestrated)", def.StoredProcedure);
        Assert.Contains("fecha_asignacion", def.JobParameters);
        Assert.Contains("id_tipo_tarea", def.JobParameters);
        Assert.Contains("usuario_id", def.JobParameters);
        Assert.Equal(67, PartesCatalog.Operations.Count);
    }

    [Fact]
    public async Task Create_missingParams_returnsInvalidParameters()
    {
        var runner = new AsignacionesCreateRunner();
        var outcome = await runner.RunAsync(
            LabOptionsWithSql(),
            new Dictionary<string, object?> { ["fecha_asignacion"] = "2026-09-14" },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
    }

    [Fact]
    public async Task Create_sqlNotConfigured_returnsDegraded()
    {
        var runner = new AsignacionesCreateRunner();
        var outcome = await runner.RunAsync(
            new AgentOptions
            {
                AgentId = "a",
                ClientId = "c",
                AgentToken = "t",
                GatewayUrl = "http://127.0.0.1:5100/agent-hub"
            },
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["fecha_asignacion"] = DateTime.Today.ToString("yyyy-MM-dd"),
                ["id_tipo_tarea"] = 1
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Degraded, outcome.Status);
        Assert.Equal("SQL_NOT_CONFIGURED", outcome.ErrorCode);
    }

    [Fact]
    public async Task Create_fechaPasada_returnsValidation()
    {
        var runner = new AsignacionesCreateRunner();
        var outcome = await runner.RunAsync(
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["fecha_asignacion"] = "2020-01-01",
                ["id_tipo_tarea"] = 1
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("VALIDATION", outcome.ErrorCode);
        Assert.Contains("anterior", outcome.ErrorMessage ?? string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    private static AgentOptions LabOptionsWithSql() =>
        new()
        {
            AgentId = "a",
            ClientId = "c",
            AgentToken = "t",
            GatewayUrl = "http://127.0.0.1:5100/agent-hub",
            Sql = new SqlOptions
            {
                Server = "127.0.0.1",
                Database = "EMP",
                User = "sa",
                Password = "x"
            }
        };
}
