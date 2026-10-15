using PaqAgent.Options;
using PaqAgent.Partes;
using PaqContracts;

namespace PaqAgent.Tests;

public class OrdenesTrabajoEstadoRunnerTests
{
    [Fact]
    public void Catalog_containsPatchEstadoAndCambioMasivoOrchestrated()
    {
        Assert.True(PartesCatalog.TryGet("PartesProduccion.OrdenesTrabajo.PatchEstado", out var patchDef));
        Assert.Equal(PartesResponseShape.OrdenTrabajoPatchEstado, patchDef.Shape);
        Assert.Equal("(orchestrated)", patchDef.StoredProcedure);
        Assert.Contains("id_orden_trabajo", patchDef.JobParameters);
        Assert.Contains("estado", patchDef.JobParameters);
        Assert.Contains("usuario_id", patchDef.JobParameters);

        Assert.True(PartesCatalog.TryGet("PartesProduccion.OrdenesTrabajo.CambioMasivoEstado", out var masivoDef));
        Assert.Equal(PartesResponseShape.OrdenTrabajoCambioMasivoEstado, masivoDef.Shape);
        Assert.Equal("(orchestrated)", masivoDef.StoredProcedure);
        Assert.Contains("ids_json", masivoDef.JobParameters);
        Assert.Contains("operacion", masivoDef.JobParameters);

        Assert.Equal(67, PartesCatalog.Operations.Count);
    }

    [Fact]
    public async Task PatchEstado_missingKey_returnsInvalidParameters()
    {
        var runner = new OrdenesTrabajoPatchEstadoRunner();
        var outcome = await runner.RunAsync(
            LabOptionsWithSql(),
            new Dictionary<string, object?> { ["_database"] = "EMP", ["estado"] = 1 },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
    }

    [Fact]
    public async Task PatchEstado_invalidEstado_returnsValidation()
    {
        var runner = new OrdenesTrabajoPatchEstadoRunner();
        var outcome = await runner.RunAsync(
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_orden_trabajo"] = 5,
                ["estado"] = 9
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("VALIDATION", outcome.ErrorCode);
    }

    [Fact]
    public async Task PatchEstado_sqlNotConfigured_returnsDegraded()
    {
        var runner = new OrdenesTrabajoPatchEstadoRunner();
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
                ["id_orden_trabajo"] = 5,
                ["estado"] = 1
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Degraded, outcome.Status);
        Assert.Equal("SQL_NOT_CONFIGURED", outcome.ErrorCode);
    }

    [Fact]
    public async Task CambioMasivo_missingIds_returnsInvalidParameters()
    {
        var runner = new OrdenesTrabajoCambioMasivoEstadoRunner();
        var outcome = await runner.RunAsync(
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["operacion"] = "cerrar"
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
    }

    [Fact]
    public async Task CambioMasivo_sqlNotConfigured_returnsDegraded()
    {
        var runner = new OrdenesTrabajoCambioMasivoEstadoRunner();
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
                ["ids_json"] = "[1,2]",
                ["operacion"] = "cerrar"
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Degraded, outcome.Status);
        Assert.Equal("SQL_NOT_CONFIGURED", outcome.ErrorCode);
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
