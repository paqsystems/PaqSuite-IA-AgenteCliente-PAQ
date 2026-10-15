using PaqAgent.Options;
using PaqAgent.Partes;
using PaqContracts;

namespace PaqAgent.Tests;

public class PartesOperarioMutacionesRunnerTests
{
    [Fact]
    public void Catalog_containsCreateUpdateAndTransicionesOrchestrated()
    {
        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.Create", out var createDef));
        Assert.Equal(PartesResponseShape.ParteOperarioCreate, createDef.Shape);
        Assert.Equal("(orchestrated)", createDef.StoredProcedure);
        Assert.Contains("fecha_parte", createDef.JobParameters);
        Assert.Contains("id_turno", createDef.JobParameters);
        Assert.Contains("usuario_id", createDef.JobParameters);

        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.Update", out var updateDef));
        Assert.Equal(PartesResponseShape.ParteOperarioUpdate, updateDef.Shape);
        Assert.Equal("(orchestrated)", updateDef.StoredProcedure);
        Assert.Contains("id_parte_operario", updateDef.JobParameters);
        Assert.Contains("observaciones", updateDef.JobParameters);
        Assert.Contains("usuario_id", updateDef.JobParameters);

        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.Enviar", out var enviarDef));
        Assert.Equal(PartesResponseShape.ParteOperarioEnviar, enviarDef.Shape);
        Assert.Equal("(orchestrated)", enviarDef.StoredProcedure);
        Assert.Contains("id_parte_operario", enviarDef.JobParameters);
        Assert.Contains("usuario_id", enviarDef.JobParameters);

        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.Aprobar", out var aprobarDef));
        Assert.Equal(PartesResponseShape.ParteOperarioAprobar, aprobarDef.Shape);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.Devolver", out var devolverDef));
        Assert.Equal(PartesResponseShape.ParteOperarioDevolver, devolverDef.Shape);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.Cerrar", out var cerrarDef));
        Assert.Equal(PartesResponseShape.ParteOperarioCerrar, cerrarDef.Shape);

        Assert.Equal(67, PartesCatalog.Operations.Count);
    }

    [Fact]
    public async Task Create_missingParams_returnsInvalidParameters()
    {
        var runner = new PartesOperarioCreateRunner();
        var outcome = await runner.RunAsync(
            LabOptionsWithSql(),
            new Dictionary<string, object?> { ["fecha_parte"] = "2026-09-15" },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
    }

    [Fact]
    public async Task Create_fechaInvalida_returnsValidation()
    {
        var runner = new PartesOperarioCreateRunner();
        var outcome = await runner.RunAsync(
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["fecha_parte"] = "not-a-date",
                ["usuario_id"] = 7
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("VALIDATION", outcome.ErrorCode);
        Assert.Contains("AAAA-MM-DD", outcome.ErrorMessage ?? string.Empty, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Create_sqlNotConfigured_returnsDegraded()
    {
        var runner = new PartesOperarioCreateRunner();
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
                ["fecha_parte"] = "2026-09-15",
                ["usuario_id"] = 7
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Degraded, outcome.Status);
        Assert.Equal("SQL_NOT_CONFIGURED", outcome.ErrorCode);
    }

    [Fact]
    public async Task Update_missingParams_returnsInvalidParameters()
    {
        var runner = new PartesOperarioUpdateRunner();
        var outcome = await runner.RunAsync(
            LabOptionsWithSql(),
            new Dictionary<string, object?> { ["observaciones"] = "nota" },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
    }

    [Fact]
    public async Task Update_observacionesTooLong_returnsValidation()
    {
        var runner = new PartesOperarioUpdateRunner();
        var outcome = await runner.RunAsync(
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_parte_operario"] = 1,
                ["usuario_id"] = 7,
                ["observaciones"] = new string('x', 2001)
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("VALIDATION", outcome.ErrorCode);
        Assert.Contains("2000", outcome.ErrorMessage ?? string.Empty, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Update_sqlNotConfigured_returnsDegraded()
    {
        var runner = new PartesOperarioUpdateRunner();
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
                ["id_parte_operario"] = 1,
                ["usuario_id"] = 7,
                ["observaciones"] = "nota"
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Degraded, outcome.Status);
        Assert.Equal("SQL_NOT_CONFIGURED", outcome.ErrorCode);
    }

    [Fact]
    public async Task Enviar_missingParams_returnsInvalidParameters()
    {
        var runner = new PartesOperarioEnviarRunner();
        var outcome = await runner.RunAsync(
            LabOptionsWithSql(),
            new Dictionary<string, object?> { ["usuario_id"] = 7 },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
    }

    [Fact]
    public async Task Enviar_sqlNotConfigured_returnsDegraded()
    {
        var runner = new PartesOperarioEnviarRunner();
        var outcome = await runner.RunAsync(
            OptionsWithoutSql(),
            TransitionParams(),
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Degraded, outcome.Status);
        Assert.Equal("SQL_NOT_CONFIGURED", outcome.ErrorCode);
    }

    [Fact]
    public async Task Aprobar_missingParams_returnsInvalidParameters()
    {
        var runner = new PartesOperarioAprobarRunner();
        var outcome = await runner.RunAsync(
            LabOptionsWithSql(),
            new Dictionary<string, object?> { ["id_parte_operario"] = 1 },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
    }

    [Fact]
    public async Task Aprobar_sqlNotConfigured_returnsDegraded()
    {
        var runner = new PartesOperarioAprobarRunner();
        var outcome = await runner.RunAsync(
            OptionsWithoutSql(),
            TransitionParams(),
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Degraded, outcome.Status);
        Assert.Equal("SQL_NOT_CONFIGURED", outcome.ErrorCode);
    }

    [Fact]
    public async Task Devolver_missingParams_returnsInvalidParameters()
    {
        var runner = new PartesOperarioDevolverRunner();
        var outcome = await runner.RunAsync(
            LabOptionsWithSql(),
            new Dictionary<string, object?> { ["_database"] = "EMP" },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
    }

    [Fact]
    public async Task Devolver_sqlNotConfigured_returnsDegraded()
    {
        var runner = new PartesOperarioDevolverRunner();
        var outcome = await runner.RunAsync(
            OptionsWithoutSql(),
            TransitionParams(),
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Degraded, outcome.Status);
        Assert.Equal("SQL_NOT_CONFIGURED", outcome.ErrorCode);
    }

    [Fact]
    public async Task Cerrar_missingParams_returnsInvalidParameters()
    {
        var runner = new PartesOperarioCerrarRunner();
        var outcome = await runner.RunAsync(
            LabOptionsWithSql(),
            new Dictionary<string, object?> { ["id_parte_operario"] = 1 },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
    }

    [Fact]
    public async Task Cerrar_sqlNotConfigured_returnsDegraded()
    {
        var runner = new PartesOperarioCerrarRunner();
        var outcome = await runner.RunAsync(
            OptionsWithoutSql(),
            TransitionParams(),
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Degraded, outcome.Status);
        Assert.Equal("SQL_NOT_CONFIGURED", outcome.ErrorCode);
    }

    private static Dictionary<string, object?> TransitionParams() =>
        new()
        {
            ["_database"] = "EMP",
            ["id_parte_operario"] = 1,
            ["usuario_id"] = 7
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
