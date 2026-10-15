using PaqAgent.Options;
using PaqAgent.Partes;
using PaqContracts;

namespace PaqAgent.Tests;

public class AsignacionesMutacionesCabeceraRunnerTests
{
    [Fact]
    public void Catalog_containsUpdatePublicarCerrarCancelarOrchestrated()
    {
        Assert.True(PartesCatalog.TryGet("PartesProduccion.Asignaciones.Update", out var upd));
        Assert.Equal(PartesResponseShape.AsignacionUpdate, upd.Shape);
        Assert.Equal("(orchestrated)", upd.StoredProcedure);
        Assert.Contains("id_asignacion", upd.JobParameters);
        Assert.Contains("fecha_asignacion", upd.JobParameters);

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Asignaciones.Publicar", out var pub));
        Assert.Equal(PartesResponseShape.AsignacionPublicar, pub.Shape);
        Assert.Equal("(orchestrated)", pub.StoredProcedure);
        Assert.Contains("id_asignacion", pub.JobParameters);

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Asignaciones.Cerrar", out var cer));
        Assert.Equal(PartesResponseShape.AsignacionCerrar, cer.Shape);
        Assert.Contains("usuario_id", cer.JobParameters);

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Asignaciones.Cancelar", out var can));
        Assert.Equal(PartesResponseShape.AsignacionCancelar, can.Shape);
        Assert.Contains("usuario_id", can.JobParameters);

        Assert.Equal(67, PartesCatalog.Operations.Count);
    }

    [Fact]
    public async Task Update_missingParams_returnsInvalidParameters()
    {
        var runner = new AsignacionesUpdateRunner();
        var outcome = await runner.RunAsync(
            LabOptionsWithSql(),
            new Dictionary<string, object?> { ["id_asignacion"] = 1 },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
    }

    [Fact]
    public async Task Update_fechaPasada_returnsValidation()
    {
        var runner = new AsignacionesUpdateRunner();
        var outcome = await runner.RunAsync(
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_asignacion"] = 1,
                ["fecha_asignacion"] = "2020-01-01",
                ["id_tipo_tarea"] = 1
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("VALIDATION", outcome.ErrorCode);
        Assert.Contains("anterior", outcome.ErrorMessage ?? string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Update_sqlNotConfigured_returnsDegraded()
    {
        var runner = new AsignacionesUpdateRunner();
        var outcome = await runner.RunAsync(
            OptionsWithoutSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_asignacion"] = 1,
                ["fecha_asignacion"] = DateTime.Today.ToString("yyyy-MM-dd"),
                ["id_tipo_tarea"] = 1
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Degraded, outcome.Status);
        Assert.Equal("SQL_NOT_CONFIGURED", outcome.ErrorCode);
    }

    [Fact]
    public async Task Publicar_missingParams_returnsInvalidParameters()
    {
        var runner = new AsignacionesPublicarRunner();
        var outcome = await runner.RunAsync(
            LabOptionsWithSql(),
            new Dictionary<string, object?>(),
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
    }

    [Fact]
    public async Task Publicar_sqlNotConfigured_returnsDegraded()
    {
        var runner = new AsignacionesPublicarRunner();
        var outcome = await runner.RunAsync(
            OptionsWithoutSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_asignacion"] = 1
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Degraded, outcome.Status);
        Assert.Equal("SQL_NOT_CONFIGURED", outcome.ErrorCode);
    }

    [Fact]
    public async Task Cerrar_missingParams_returnsInvalidParameters()
    {
        var runner = new AsignacionesCerrarRunner();
        var outcome = await runner.RunAsync(
            LabOptionsWithSql(),
            new Dictionary<string, object?> { ["usuario_id"] = 1 },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
    }

    [Fact]
    public async Task Cerrar_sqlNotConfigured_returnsDegraded()
    {
        var runner = new AsignacionesCerrarRunner();
        var outcome = await runner.RunAsync(
            OptionsWithoutSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_asignacion"] = 1,
                ["usuario_id"] = 1
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Degraded, outcome.Status);
        Assert.Equal("SQL_NOT_CONFIGURED", outcome.ErrorCode);
    }

    [Fact]
    public async Task Cancelar_missingParams_returnsInvalidParameters()
    {
        var runner = new AsignacionesCancelarRunner();
        var outcome = await runner.RunAsync(
            LabOptionsWithSql(),
            new Dictionary<string, object?> { ["id_asignacion"] = 0 },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
    }

    [Fact]
    public async Task Cancelar_sqlNotConfigured_returnsDegraded()
    {
        var runner = new AsignacionesCancelarRunner();
        var outcome = await runner.RunAsync(
            OptionsWithoutSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_asignacion"] = 1,
                ["usuario_id"] = 1
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

    private static AgentOptions OptionsWithoutSql() =>
        new()
        {
            AgentId = "a",
            ClientId = "c",
            AgentToken = "t",
            GatewayUrl = "http://127.0.0.1:5100/agent-hub"
        };
}
