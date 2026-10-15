using PaqAgent.Options;
using PaqAgent.Partes;
using PaqContracts;

namespace PaqAgent.Tests;

public class OrdenesTrabajoCreateRunnerTests
{
    [Fact]
    public void Catalog_containsCreateOrchestrated()
    {
        Assert.True(PartesCatalog.TryGet("PartesProduccion.OrdenesTrabajo.Create", out var def));
        Assert.Equal(PartesResponseShape.OrdenTrabajoCreate, def.Shape);
        Assert.Equal("(orchestrated)", def.StoredProcedure);
        Assert.Contains("id_articulo", def.JobParameters);
        Assert.Contains("operaciones_incluidas_json", def.JobParameters);
        Assert.Contains("usuario_id", def.JobParameters);
        Assert.Equal(67, PartesCatalog.Operations.Count);
    }

    [Fact]
    public async Task Create_missingParams_returnsInvalidParameters()
    {
        var runner = new OrdenesTrabajoCreateRunner();
        var outcome = await runner.RunAsync(
            LabOptionsWithSql(),
            new Dictionary<string, object?> { ["id_articulo"] = 1 },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
    }

    [Fact]
    public async Task Create_sqlNotConfigured_returnsDegraded()
    {
        var runner = new OrdenesTrabajoCreateRunner();
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
                ["id_articulo"] = 10,
                ["cantidad_a_producir"] = 5,
                ["modo_individual"] = true,
                ["id_operacion"] = 1
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Degraded, outcome.Status);
        Assert.Equal("SQL_NOT_CONFIGURED", outcome.ErrorCode);
    }

    [Fact]
    public async Task Create_missingOperacion_returnsValidation()
    {
        var runner = new OrdenesTrabajoCreateRunner();
        var outcome = await runner.RunAsync(
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_articulo"] = 10,
                ["cantidad_a_producir"] = 5,
                ["modo_individual"] = true
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("VALIDATION", outcome.ErrorCode);
        Assert.Contains("id_operacion", outcome.ErrorMessage ?? string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Create_invalidOperacionesJson_returnsInvalidParameters()
    {
        var runner = new OrdenesTrabajoCreateRunner();
        var outcome = await runner.RunAsync(
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_articulo"] = 10,
                ["cantidad_a_producir"] = 5,
                ["modo_individual"] = false,
                ["operaciones_incluidas_json"] = "{not-json"
            },
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
}
