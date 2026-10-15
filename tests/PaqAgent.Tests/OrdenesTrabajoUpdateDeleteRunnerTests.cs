using PaqAgent.Options;
using PaqAgent.Partes;
using PaqContracts;

namespace PaqAgent.Tests;

public class OrdenesTrabajoUpdateDeleteRunnerTests
{
    [Fact]
    public void Catalog_containsUpdateAndDeleteOrchestrated()
    {
        Assert.True(PartesCatalog.TryGet("PartesProduccion.OrdenesTrabajo.Update", out var updateDef));
        Assert.Equal(PartesResponseShape.OrdenTrabajoUpdate, updateDef.Shape);
        Assert.Equal("(orchestrated)", updateDef.StoredProcedure);
        Assert.Contains("id_orden_trabajo", updateDef.JobParameters);
        Assert.Contains("codigo_ot", updateDef.JobParameters);
        Assert.Contains("operaciones_incluidas_json", updateDef.JobParameters);

        Assert.True(PartesCatalog.TryGet("PartesProduccion.OrdenesTrabajo.Delete", out var deleteDef));
        Assert.Equal(PartesResponseShape.OrdenTrabajoDelete, deleteDef.Shape);
        Assert.Equal("(orchestrated)", deleteDef.StoredProcedure);
        Assert.Contains("id_orden_trabajo", deleteDef.JobParameters);
        Assert.Contains("usuario_codigo", deleteDef.JobParameters);

        Assert.Equal(67, PartesCatalog.Operations.Count);
    }

    [Fact]
    public async Task Update_missingKey_returnsInvalidParameters()
    {
        var runner = new OrdenesTrabajoUpdateRunner();
        var outcome = await runner.RunAsync(
            LabOptionsWithSql(),
            new Dictionary<string, object?> { ["_database"] = "EMP", ["id_articulo"] = 1 },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
    }

    [Fact]
    public async Task Update_sqlNotConfigured_returnsDegraded()
    {
        var runner = new OrdenesTrabajoUpdateRunner();
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
                ["id_orden_trabajo"] = 10,
                ["id_articulo"] = 1,
                ["id_operacion"] = 2
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Degraded, outcome.Status);
        Assert.Equal("SQL_NOT_CONFIGURED", outcome.ErrorCode);
    }

    [Fact]
    public async Task Delete_missingId_returnsInvalidParameters()
    {
        var runner = new OrdenesTrabajoDeleteRunner();
        var outcome = await runner.RunAsync(
            LabOptionsWithSql(),
            new Dictionary<string, object?> { ["_database"] = "EMP" },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
    }

    [Fact]
    public async Task Delete_empUser_returnsForbidden()
    {
        var runner = new OrdenesTrabajoDeleteRunner();
        var outcome = await runner.RunAsync(
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_orden_trabajo"] = 5,
                ["usuario_codigo"] = "EMP"
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("FORBIDDEN", outcome.ErrorCode);
    }

    [Fact]
    public async Task Delete_sqlNotConfigured_returnsDegraded()
    {
        var runner = new OrdenesTrabajoDeleteRunner();
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
                ["id_orden_trabajo"] = 5
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
