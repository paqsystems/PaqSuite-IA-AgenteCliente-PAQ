using PaqAgent.Options;
using PaqAgent.Partes;
using PaqContracts;

namespace PaqAgent.Tests;

public class PartesEntradasMutacionesRunnerTests
{
    [Fact]
    public void Catalog_containsEntradasOrchestratedOps()
    {
        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.Entradas.Create", out var createDef));
        Assert.Equal(PartesResponseShape.ParteOperarioEntradasCreate, createDef.Shape);
        Assert.Equal("(orchestrated)", createDef.StoredProcedure);
        Assert.Contains("id_parte_operario", createDef.JobParameters);
        Assert.Contains("usuario_id", createDef.JobParameters);
        Assert.Contains("id_concepto_tiempo", createDef.JobParameters);
        Assert.Contains("id_maquina", createDef.JobParameters);

        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.Entradas.Update", out var updateDef));
        Assert.Equal(PartesResponseShape.ParteOperarioEntradasUpdate, updateDef.Shape);
        Assert.Equal("(orchestrated)", updateDef.StoredProcedure);
        Assert.Contains("id_parte_entrada", updateDef.JobParameters);
        Assert.DoesNotContain("id_asignacion_item", updateDef.JobParameters);

        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.Entradas.Delete", out var deleteDef));
        Assert.Equal(PartesResponseShape.ParteOperarioEntradasDelete, deleteDef.Shape);
        Assert.Contains("id_parte_entrada", deleteDef.JobParameters);
        Assert.Contains("usuario_id", deleteDef.JobParameters);

        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.Entradas.Reclasificar", out var reclasificarDef));
        Assert.Equal(PartesResponseShape.ParteOperarioEntradasReclasificar, reclasificarDef.Shape);
        Assert.Contains("notas_revision", reclasificarDef.JobParameters);
        Assert.Contains("usuario_id", reclasificarDef.JobParameters);

        Assert.Equal(67, PartesCatalog.Operations.Count);
    }

    [Fact]
    public async Task Create_missingParams_returnsInvalidParameters()
    {
        var runner = new PartesEntradasCreateRunner();
        var outcome = await runner.RunAsync(
            LabOptionsWithSql(),
            new Dictionary<string, object?> { ["id_parte_operario"] = 1 },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
    }

    [Fact]
    public async Task Create_sqlNotConfigured_returnsDegraded()
    {
        var runner = new PartesEntradasCreateRunner();
        var outcome = await runner.RunAsync(
            OptionsWithoutSql(),
            CreateParams(),
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Degraded, outcome.Status);
        Assert.Equal("SQL_NOT_CONFIGURED", outcome.ErrorCode);
    }

    [Fact]
    public async Task Update_missingParams_returnsInvalidParameters()
    {
        var runner = new PartesEntradasUpdateRunner();
        var outcome = await runner.RunAsync(
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["id_parte_operario"] = 1,
                ["usuario_id"] = 7
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
    }

    [Fact]
    public async Task Update_sqlNotConfigured_returnsDegraded()
    {
        var runner = new PartesEntradasUpdateRunner();
        var outcome = await runner.RunAsync(
            OptionsWithoutSql(),
            UpdateParams(),
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Degraded, outcome.Status);
        Assert.Equal("SQL_NOT_CONFIGURED", outcome.ErrorCode);
    }

    [Fact]
    public async Task Delete_missingParams_returnsInvalidParameters()
    {
        var runner = new PartesEntradasDeleteRunner();
        var outcome = await runner.RunAsync(
            LabOptionsWithSql(),
            new Dictionary<string, object?> { ["id_parte_entrada"] = 81 },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
    }

    [Fact]
    public async Task Delete_sqlNotConfigured_returnsDegraded()
    {
        var runner = new PartesEntradasDeleteRunner();
        var outcome = await runner.RunAsync(
            OptionsWithoutSql(),
            UpdateParams(),
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Degraded, outcome.Status);
        Assert.Equal("SQL_NOT_CONFIGURED", outcome.ErrorCode);
    }

    [Fact]
    public async Task Reclasificar_missingParams_returnsInvalidParameters()
    {
        var runner = new PartesEntradasReclasificarRunner();
        var outcome = await runner.RunAsync(
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_parte_operario"] = 1,
                ["id_parte_entrada"] = 81,
                ["usuario_id"] = 7
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
        Assert.Contains("notas_revision", outcome.ErrorMessage ?? string.Empty, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Reclasificar_sqlNotConfigured_returnsDegraded()
    {
        var runner = new PartesEntradasReclasificarRunner();
        var outcome = await runner.RunAsync(
            OptionsWithoutSql(),
            ReclasificarParams(),
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Degraded, outcome.Status);
        Assert.Equal("SQL_NOT_CONFIGURED", outcome.ErrorCode);
    }

    private static Dictionary<string, object?> CreateParams() =>
        new()
        {
            ["_database"] = "EMP",
            ["id_parte_operario"] = 1,
            ["usuario_id"] = 7,
            ["id_concepto_tiempo"] = 4,
            ["minutos"] = 45,
            ["id_orden_trabajo"] = 40,
            ["id_maquina"] = 3
        };

    private static Dictionary<string, object?> UpdateParams() =>
        new()
        {
            ["_database"] = "EMP",
            ["id_parte_operario"] = 1,
            ["id_parte_entrada"] = 81,
            ["usuario_id"] = 7,
            ["minutos"] = 50
        };

    private static Dictionary<string, object?> ReclasificarParams() =>
        new()
        {
            ["_database"] = "EMP",
            ["id_parte_operario"] = 1,
            ["id_parte_entrada"] = 81,
            ["usuario_id"] = 7,
            ["notas_revision"] = "corrige OT"
        };

    private static AgentOptions OptionsWithoutSql() =>
        new()
        {
            AgentId = "a",
            ClientId = "c",
            AgentToken = "t",
            GatewayUrl = "http://127.0.0.1:5100/agent-hub"
        };

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
