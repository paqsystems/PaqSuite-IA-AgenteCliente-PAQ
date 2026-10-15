using PaqAgent.Options;
using PaqAgent.Partes;
using PaqContracts;

namespace PaqAgent.Tests;

public class AsignacionesItemsOperariosRunnerTests
{
    [Fact]
    public void Catalog_containsItemsAndOperariosOps()
    {
        Assert.True(PartesCatalog.TryGet("PartesProduccion.Asignaciones.Items.List", out var list));
        Assert.Equal(PartesResponseShape.AsignacionItemList, list.Shape);
        Assert.Equal("dbo.PAQ_PartesProduccion_AsignacionesItemsList", list.StoredProcedure);

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Asignaciones.Items.Create", out var create));
        Assert.Equal("(orchestrated)", create.StoredProcedure);

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Asignaciones.OperariosPlan.List", out var plan));
        Assert.Equal(PartesResponseShape.AsignacionOperariosPlanList, plan.Shape);

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Asignaciones.Items.Operarios.List", out var opList));
        Assert.Equal("dbo.PAQ_PartesProduccion_AsignacionesItemsOperariosList", opList.StoredProcedure);

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Asignaciones.Items.Operarios.Create", out var opCreate));
        Assert.Equal(PartesResponseShape.AsignacionItemOperariosCreate, opCreate.Shape);

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Asignaciones.Items.Operarios.Update", out var opUpdate));
        Assert.Contains("id_operarios", opUpdate.JobParameters);

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Asignaciones.Items.Operarios.Delete", out var opDelete));
        Assert.Contains("id_operario", opDelete.JobParameters);

        Assert.Equal(67, PartesCatalog.Operations.Count);
    }

    [Fact]
    public async Task ItemsCreate_missingParams_returnsInvalidParameters()
    {
        var runner = new AsignacionesItemsCreateRunner();
        var outcome = await runner.RunAsync(
            LabOptionsWithSql(),
            new Dictionary<string, object?> { ["id_asignacion"] = 1 },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
    }

    [Fact]
    public async Task ItemsDelete_sqlNotConfigured_returnsDegraded()
    {
        var runner = new AsignacionesItemsDeleteRunner();
        var outcome = await runner.RunAsync(
            OptionsWithoutSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_asignacion"] = 1,
                ["id_item"] = 1
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Degraded, outcome.Status);
        Assert.Equal("SQL_NOT_CONFIGURED", outcome.ErrorCode);
    }

    [Fact]
    public async Task OperariosPlan_missingParams_returnsInvalidParameters()
    {
        var runner = new AsignacionesOperariosPlanListRunner();
        var outcome = await runner.RunAsync(
            LabOptionsWithSql(),
            new Dictionary<string, object?>(),
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
    }

    [Fact]
    public async Task OperariosCreate_sqlNotConfigured_returnsDegraded()
    {
        var runner = new AsignacionesItemsOperariosCreateRunner();
        var outcome = await runner.RunAsync(
            OptionsWithoutSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_asignacion"] = 1,
                ["id_item"] = 1,
                ["id_operario"] = 1
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Degraded, outcome.Status);
        Assert.Equal("SQL_NOT_CONFIGURED", outcome.ErrorCode);
    }

    [Fact]
    public async Task OperariosUpdate_missingIdOperarios_returnsInvalidParameters()
    {
        var runner = new AsignacionesItemsOperariosUpdateRunner();
        var outcome = await runner.RunAsync(
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_asignacion"] = 1,
                ["id_item"] = 1
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
    }

    [Fact]
    public async Task OperariosDelete_missingParams_returnsInvalidParameters()
    {
        var runner = new AsignacionesItemsOperariosDeleteRunner();
        var outcome = await runner.RunAsync(
            LabOptionsWithSql(),
            new Dictionary<string, object?> { ["id_asignacion"] = 1 },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
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

    private static AgentOptions OptionsWithoutSql() =>
        new()
        {
            AgentId = "a",
            ClientId = "c",
            AgentToken = "t",
            GatewayUrl = "http://127.0.0.1:5100/agent-hub"
        };
}
