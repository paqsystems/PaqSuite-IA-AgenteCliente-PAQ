using PaqAgent.Informes;
using PaqAgent.Options;
using PaqAgent.Partes;
using PaqContracts;

namespace PaqAgent.Tests;

public class PartesGatewayRunnerTests
{
    [Fact]
    public void Catalog_containsThirtyPartesOps()
    {
        Assert.Equal(67, PartesCatalog.Operations.Count);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.Parametros.List", out var paramsDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_ParametrosList", paramsDef.StoredProcedure);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.InformesGestion.List", out var informesDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_InformesGestion", informesDef.StoredProcedure);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.Dashboard.Get", out var dashboardDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_DashboardGet", dashboardDef.StoredProcedure);
        Assert.Equal(PartesResponseShape.DashboardGet, dashboardDef.Shape);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.Maquinas.List", out var maquinasListDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_MaquinasList", maquinasListDef.StoredProcedure);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.Maquinas.Get", out var maquinasDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_MaquinasGet", maquinasDef.StoredProcedure);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.Maquinas.Create", out var createDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_MaquinasCreate", createDef.StoredProcedure);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.Maquinas.Update", out var updateDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_MaquinasUpdate", updateDef.StoredProcedure);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.Maquinas.Delete", out var deleteDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_MaquinasDelete", deleteDef.StoredProcedure);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.TiposTarea.List", out var tiposListDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_TiposTareaList", tiposListDef.StoredProcedure);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.TiposTarea.Get", out var tiposGetDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_TiposTareaGet", tiposGetDef.StoredProcedure);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.TiposTarea.Create", out var tiposCreateDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_TiposTareaCreate", tiposCreateDef.StoredProcedure);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.TiposTarea.Update", out var tiposUpdateDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_TiposTareaUpdate", tiposUpdateDef.StoredProcedure);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.TiposTarea.Delete", out var tiposDeleteDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_TiposTareaDelete", tiposDeleteDef.StoredProcedure);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.Operaciones.List", out var opsListDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_OperacionesList", opsListDef.StoredProcedure);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.Operaciones.Get", out var opsGetDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_OperacionesGet", opsGetDef.StoredProcedure);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.Operaciones.Create", out var opsCreateDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_OperacionesCreate", opsCreateDef.StoredProcedure);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.Operaciones.Update", out var opsUpdateDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_OperacionesUpdate", opsUpdateDef.StoredProcedure);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.Operaciones.Delete", out var opsDeleteDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_OperacionesDelete", opsDeleteDef.StoredProcedure);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.Turnos.List", out var turnosListDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_TurnosList", turnosListDef.StoredProcedure);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.Turnos.Get", out var turnosGetDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_TurnosGet", turnosGetDef.StoredProcedure);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.Turnos.Create", out var turnosCreateDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_TurnosCreate", turnosCreateDef.StoredProcedure);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.Turnos.Update", out var turnosUpdateDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_TurnosUpdate", turnosUpdateDef.StoredProcedure);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.Turnos.Delete", out var turnosDeleteDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_TurnosDelete", turnosDeleteDef.StoredProcedure);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.ConceptosTiempo.List", out var conceptosListDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_ConceptosTiempoList", conceptosListDef.StoredProcedure);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.ConceptosTiempo.Get", out var conceptosGetDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_ConceptosTiempoGet", conceptosGetDef.StoredProcedure);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.ConceptosTiempo.Create", out var conceptosCreateDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_ConceptosTiempoCreate", conceptosCreateDef.StoredProcedure);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.ConceptosTiempo.Update", out var conceptosUpdateDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_ConceptosTiempoUpdate", conceptosUpdateDef.StoredProcedure);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.ConceptosTiempo.Delete", out var conceptosDeleteDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_ConceptosTiempoDelete", conceptosDeleteDef.StoredProcedure);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.OrdenesTrabajo.List", out var otListDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_OrdenesTrabajoList", otListDef.StoredProcedure);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.OrdenesTrabajo.Get", out var otGetDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_OrdenesTrabajoGet", otGetDef.StoredProcedure);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.OrdenesTrabajo.Create", out var otCreateDef));
        Assert.Equal("(orchestrated)", otCreateDef.StoredProcedure);
        Assert.Equal(PartesResponseShape.OrdenTrabajoCreate, otCreateDef.Shape);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.Asignaciones.List", out var asignacionesListDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_AsignacionesList", asignacionesListDef.StoredProcedure);
        Assert.Equal(PartesResponseShape.AsignacionList, asignacionesListDef.Shape);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.Asignaciones.Get", out var asignacionesGetDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_AsignacionesGet", asignacionesGetDef.StoredProcedure);
        Assert.Equal(PartesResponseShape.AsignacionGet, asignacionesGetDef.Shape);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.Asignaciones.Create", out var asignacionesCreateDef));
        Assert.Equal("(orchestrated)", asignacionesCreateDef.StoredProcedure);
        Assert.Equal(PartesResponseShape.AsignacionCreate, asignacionesCreateDef.Shape);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.Get", out var partesOperarioGetDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_PartesOperarioGet", partesOperarioGetDef.StoredProcedure);
        Assert.Equal(PartesResponseShape.ParteOperarioGet, partesOperarioGetDef.Shape);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.Create", out var partesOperarioCreateDef));
        Assert.Equal("(orchestrated)", partesOperarioCreateDef.StoredProcedure);
        Assert.Equal(PartesResponseShape.ParteOperarioCreate, partesOperarioCreateDef.Shape);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.Update", out var partesOperarioUpdateDef));
        Assert.Equal("(orchestrated)", partesOperarioUpdateDef.StoredProcedure);
        Assert.Equal(PartesResponseShape.ParteOperarioUpdate, partesOperarioUpdateDef.Shape);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.Enviar", out var partesOperarioEnviarDef));
        Assert.Equal("(orchestrated)", partesOperarioEnviarDef.StoredProcedure);
        Assert.Equal(PartesResponseShape.ParteOperarioEnviar, partesOperarioEnviarDef.Shape);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.Aprobar", out var partesOperarioAprobarDef));
        Assert.Equal(PartesResponseShape.ParteOperarioAprobar, partesOperarioAprobarDef.Shape);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.Devolver", out var partesOperarioDevolverDef));
        Assert.Equal(PartesResponseShape.ParteOperarioDevolver, partesOperarioDevolverDef.Shape);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.Cerrar", out var partesOperarioCerrarDef));
        Assert.Equal(PartesResponseShape.ParteOperarioCerrar, partesOperarioCerrarDef.Shape);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.ParteContexto", out var parteContextoDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_PartesOperarioParteContexto", parteContextoDef.StoredProcedure);
        Assert.Equal(PartesResponseShape.ParteOperarioParteContexto, parteContextoDef.Shape);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.MisPartes.List", out var misPartesDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_PartesOperarioMisPartesList", misPartesDef.StoredProcedure);
        Assert.Equal(PartesResponseShape.ParteOperarioMisPartesList, misPartesDef.Shape);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.PartesRevisar.List", out var partesRevisarDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_PartesOperarioPartesRevisarList", partesRevisarDef.StoredProcedure);
        Assert.Equal(PartesResponseShape.ParteOperarioPartesRevisarList, partesRevisarDef.Shape);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.PartesRevisar.Get", out var partesRevisarGetDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_PartesOperarioPartesRevisarGet", partesRevisarGetDef.StoredProcedure);
        Assert.Equal(PartesResponseShape.ParteOperarioPartesRevisarGet, partesRevisarGetDef.Shape);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.Entradas.List", out var entradasListDef));
        Assert.Equal("dbo.PAQ_PartesProduccion_PartesOperarioEntradasList", entradasListDef.StoredProcedure);
        Assert.Equal(PartesResponseShape.ParteOperarioEntradasList, entradasListDef.Shape);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.Entradas.Create", out var entradasCreateDef));
        Assert.Equal("(orchestrated)", entradasCreateDef.StoredProcedure);
        Assert.Equal(PartesResponseShape.ParteOperarioEntradasCreate, entradasCreateDef.Shape);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.Entradas.Update", out var entradasUpdateDef));
        Assert.Equal(PartesResponseShape.ParteOperarioEntradasUpdate, entradasUpdateDef.Shape);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.Entradas.Delete", out var entradasDeleteDef));
        Assert.Equal(PartesResponseShape.ParteOperarioEntradasDelete, entradasDeleteDef.Shape);
        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.Entradas.Reclasificar", out var entradasReclasificarDef));
        Assert.Equal(PartesResponseShape.ParteOperarioEntradasReclasificar, entradasReclasificarDef.Shape);
    }

    [Fact]
    public async Task RunAsync_missingDatabase_returnsInvalidParameters()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) => throw new InvalidOperationException("no call")));
        Assert.True(PartesCatalog.TryGet("PartesProduccion.Parametros.List", out var def));

        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>(),
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
    }

    [Fact]
    public async Task RunAsync_parametros_mapsPayloadWithPrograma()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) =>
            new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase) { ["total_filas"] = 1 }
                },
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["Programa"] = "PartesProduccion",
                        ["Clave"] = "MinutosMinimos",
                        ["Tipo_Valor"] = "I",
                        ["Valor_Int"] = 15
                    }
                }
            }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Parametros.List", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?> { ["_database"] = "EMP" },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        var parametros = Assert.IsAssignableFrom<IEnumerable<Dictionary<string, object?>>>(data["parametros"]).ToList();
        Assert.Single(parametros);
        Assert.Equal("PartesProduccion", parametros[0]["Programa"]);
        Assert.Equal("MinutosMinimos", parametros[0]["Clave"]);
        Assert.Equal(15, parametros[0]["ValorInt"]);
    }

    [Fact]
    public async Task RunAsync_informesGestion_mapsItemsAndResumen()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_InformesGestion", sp);
            Assert.Equal("2026-01-01", spParams["FechaDesde"]);
            Assert.Equal(3, spParams["IdTurno"]);
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase) { ["total_filas"] = 1 }
                },
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["id_parte_entrada"] = 10,
                        ["fecha_parte"] = new DateTime(2026, 1, 15),
                        ["productive_minutes"] = 60,
                        ["non_productive_minutes"] = 10,
                        ["units_done"] = 5m,
                        ["theoretical_units"] = 6m,
                        ["efficiency_pct"] = 83.33m
                    }
                },
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["total_productive_minutes"] = 60,
                        ["total_non_productive_minutes"] = 10,
                        ["total_theoretical_units"] = 6m,
                        ["total_units_done"] = 5m
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.InformesGestion.List", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["fecha_desde"] = "2026-01-01",
                ["fecha_hasta"] = "2026-01-31",
                ["id_turno"] = 3
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        var items = Assert.IsAssignableFrom<IEnumerable<Dictionary<string, object?>>>(data["items"]).ToList();
        Assert.Single(items);
        Assert.Equal(10, items[0]["id_parte_entrada"]);
        Assert.Equal(new DateTime(2026, 1, 15), items[0]["fecha"]);

        var resumen = Assert.IsType<Dictionary<string, object?>>(data["resumen"]);
        Assert.Equal(60, resumen["productive_minutes"]);
        Assert.Equal(83.33m, resumen["efficiency_pct"]);
    }

    [Fact]
    public async Task RunAsync_maquinasGet_mapsPayload()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_MaquinasGet", sp);
            Assert.Equal(7, spParams["IdMaquina"]);
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["id"] = 7,
                        ["codigo"] = "M01",
                        ["nombre"] = "Fresadora",
                        ["activa"] = true
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Maquinas.Get", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_maquina"] = 7
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        Assert.Equal(7, data["id"]);
        Assert.Equal("M01", data["codigo"]);
        Assert.Equal("Fresadora", data["nombre"]);
        Assert.Equal(true, data["activa"]);
    }

    [Fact]
    public async Task RunAsync_maquinasGet_missingId_returnsInvalidParameters()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) => throw new InvalidOperationException("no call")));
        Assert.True(PartesCatalog.TryGet("PartesProduccion.Maquinas.Get", out var def));

        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?> { ["_database"] = "EMP" },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
    }

    [Fact]
    public async Task RunAsync_maquinasGet_emptyResult_returnsNotFound()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) =>
            new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>()
            }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Maquinas.Get", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_maquina"] = 99
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("NOT_FOUND", outcome.ErrorCode);
    }

    [Fact]
    public async Task RunAsync_maquinasCreate_mapsPayloadAndDefaultsActiva()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_MaquinasCreate", sp);
            Assert.Equal("M01", spParams["Codigo"]);
            Assert.Equal("Fresadora", spParams["Nombre"]);
            Assert.Equal(true, spParams["Activa"]);
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["resultCode"] = "OK",
                        ["id"] = 12,
                        ["codigo"] = "M01",
                        ["nombre"] = "Fresadora",
                        ["activa"] = true
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Maquinas.Create", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["codigo"] = " M01 ",
                ["nombre"] = " Fresadora "
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        Assert.Equal(12, data["id"]);
        Assert.Equal("M01", data["codigo"]);
        Assert.Equal("Fresadora", data["nombre"]);
        Assert.Equal(true, data["activa"]);
    }

    [Fact]
    public async Task RunAsync_maquinasCreate_missingCodigo_returnsInvalidParameters()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) => throw new InvalidOperationException("no call")));
        Assert.True(PartesCatalog.TryGet("PartesProduccion.Maquinas.Create", out var def));

        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["nombre"] = "Fresadora"
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
    }

    [Fact]
    public async Task RunAsync_maquinasCreate_duplicateCode_returnsDuplicate()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) =>
            new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["resultCode"] = "DUPLICATE_CODE",
                        ["id"] = null,
                        ["codigo"] = null,
                        ["nombre"] = null,
                        ["activa"] = null
                    }
                }
            }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Maquinas.Create", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["codigo"] = "M01",
                ["nombre"] = "Fresadora",
                ["activa"] = false
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("DUPLICATE_CODE", outcome.ErrorCode);
    }

    [Fact]
    public async Task RunAsync_maquinasUpdate_mapsPayload()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_MaquinasUpdate", sp);
            Assert.Equal(12, spParams["IdMaquina"]);
            Assert.Equal("Fresadora CNC", spParams["Nombre"]);
            Assert.Equal(false, spParams["Activa"]);
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["resultCode"] = "OK",
                        ["id"] = 12,
                        ["codigo"] = "M01",
                        ["nombre"] = "Fresadora CNC",
                        ["activa"] = false
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Maquinas.Update", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_maquina"] = 12,
                ["nombre"] = " Fresadora CNC ",
                ["activa"] = false
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        Assert.Equal(12, data["id"]);
        Assert.Equal("M01", data["codigo"]);
        Assert.Equal("Fresadora CNC", data["nombre"]);
        Assert.Equal(false, data["activa"]);
    }

    [Fact]
    public async Task RunAsync_maquinasUpdate_withoutPatchFields_returnsInvalidParameters()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) => throw new InvalidOperationException("no call")));
        Assert.True(PartesCatalog.TryGet("PartesProduccion.Maquinas.Update", out var def));

        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_maquina"] = 12
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
    }

    [Fact]
    public async Task RunAsync_maquinasUpdate_notFound_returnsNotFound()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) =>
            new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["resultCode"] = "NOT_FOUND",
                        ["id"] = null,
                        ["codigo"] = null,
                        ["nombre"] = null,
                        ["activa"] = null
                    }
                }
            }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Maquinas.Update", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_maquina"] = 99,
                ["activa"] = true
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("NOT_FOUND", outcome.ErrorCode);
    }

    [Fact]
    public async Task RunAsync_maquinasDelete_mapsPayload()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_MaquinasDelete", sp);
            Assert.Equal(12, spParams["IdMaquina"]);
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["resultCode"] = "OK",
                        ["id"] = 12,
                        ["eliminado"] = true
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Maquinas.Delete", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_maquina"] = 12
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        Assert.Equal(12, data["id"]);
        Assert.Equal(true, data["eliminado"]);
    }

    [Fact]
    public async Task RunAsync_maquinasDelete_referenced_returnsConflictCode()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) =>
            new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["resultCode"] = "REFERENCED",
                        ["id"] = 12,
                        ["eliminado"] = false
                    }
                }
            }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Maquinas.Delete", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_maquina"] = 12
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("REFERENCED", outcome.ErrorCode);
    }

    [Fact]
    public async Task RunAsync_maquinasDelete_missingId_returnsInvalidParameters()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) => throw new InvalidOperationException("no call")));
        Assert.True(PartesCatalog.TryGet("PartesProduccion.Maquinas.Delete", out var def));

        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?> { ["_database"] = "EMP" },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
    }

    [Fact]
    public async Task RunAsync_tiposTareaGet_mapsPayload()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_TiposTareaGet", sp);
            Assert.Equal(5, spParams["IdTipoTarea"]);
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["id"] = 5,
                        ["codigo"] = "TT01",
                        ["nombre"] = "Setup",
                        ["activo"] = true
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.TiposTarea.Get", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_tipo_tarea"] = 5
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        Assert.Equal(5, data["id"]);
        Assert.Equal("TT01", data["codigo"]);
        Assert.Equal("Setup", data["nombre"]);
        Assert.Equal(true, data["activo"]);
    }

    [Fact]
    public async Task RunAsync_tiposTareaCreate_mapsPayloadAndDefaultsActivo()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_TiposTareaCreate", sp);
            Assert.Equal("TT01", spParams["Codigo"]);
            Assert.Equal("Setup", spParams["Nombre"]);
            Assert.Equal(true, spParams["Activo"]);
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["resultCode"] = "OK",
                        ["id"] = 8,
                        ["codigo"] = "TT01",
                        ["nombre"] = "Setup",
                        ["activo"] = true
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.TiposTarea.Create", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["codigo"] = " TT01 ",
                ["nombre"] = " Setup "
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        Assert.Equal(8, data["id"]);
        Assert.Equal("TT01", data["codigo"]);
        Assert.Equal("Setup", data["nombre"]);
        Assert.Equal(true, data["activo"]);
    }

    [Fact]
    public async Task RunAsync_tiposTareaCreate_duplicateCode_returnsDuplicate()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) =>
            new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["resultCode"] = "DUPLICATE_CODE",
                        ["id"] = null,
                        ["codigo"] = null,
                        ["nombre"] = null,
                        ["activo"] = null
                    }
                }
            }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.TiposTarea.Create", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["codigo"] = "TT01",
                ["nombre"] = "Setup",
                ["activo"] = false
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("DUPLICATE_CODE", outcome.ErrorCode);
    }

    [Fact]
    public async Task RunAsync_tiposTareaUpdate_mapsPayload()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_TiposTareaUpdate", sp);
            Assert.Equal(8, spParams["IdTipoTarea"]);
            Assert.Equal("Setup CNC", spParams["Nombre"]);
            Assert.Equal(false, spParams["Activo"]);
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["resultCode"] = "OK",
                        ["id"] = 8,
                        ["codigo"] = "TT01",
                        ["nombre"] = "Setup CNC",
                        ["activo"] = false
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.TiposTarea.Update", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_tipo_tarea"] = 8,
                ["nombre"] = " Setup CNC ",
                ["activo"] = false
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        Assert.Equal(8, data["id"]);
        Assert.Equal("TT01", data["codigo"]);
        Assert.Equal("Setup CNC", data["nombre"]);
        Assert.Equal(false, data["activo"]);
    }

    [Fact]
    public async Task RunAsync_tiposTareaDelete_mapsPayload()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_TiposTareaDelete", sp);
            Assert.Equal(8, spParams["IdTipoTarea"]);
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["resultCode"] = "OK",
                        ["id"] = 8,
                        ["eliminado"] = true
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.TiposTarea.Delete", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_tipo_tarea"] = 8
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        Assert.Equal(8, data["id"]);
        Assert.Equal(true, data["eliminado"]);
    }

    [Fact]
    public async Task RunAsync_tiposTareaDelete_referenced_returnsConflictCode()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) =>
            new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["resultCode"] = "REFERENCED",
                        ["id"] = 8,
                        ["eliminado"] = false
                    }
                }
            }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.TiposTarea.Delete", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_tipo_tarea"] = 8
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("REFERENCED", outcome.ErrorCode);
    }

    [Fact]
    public async Task RunAsync_tiposTareaGet_emptyResult_returnsNotFound()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) =>
            new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>()
            }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.TiposTarea.Get", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_tipo_tarea"] = 99
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("NOT_FOUND", outcome.ErrorCode);
    }

    [Fact]
    public async Task RunAsync_operacionesGet_mapsPayload()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_OperacionesGet", sp);
            Assert.Equal(3, spParams["IdOperacion"]);
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["id"] = 3,
                        ["codigo"] = "OP01",
                        ["nombre"] = "Corte",
                        ["activa"] = true
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Operaciones.Get", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_operacion"] = 3
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        Assert.Equal(3, data["id"]);
        Assert.Equal("OP01", data["codigo"]);
        Assert.Equal("Corte", data["nombre"]);
        Assert.Equal(true, data["activa"]);
    }

    [Fact]
    public async Task RunAsync_operacionesCreate_mapsPayloadAndDefaultsActiva()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_OperacionesCreate", sp);
            Assert.Equal("OP01", spParams["Codigo"]);
            Assert.Equal("Corte", spParams["Nombre"]);
            Assert.Equal(true, spParams["Activa"]);
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["resultCode"] = "OK",
                        ["id"] = 15,
                        ["codigo"] = "OP01",
                        ["nombre"] = "Corte",
                        ["activa"] = true
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Operaciones.Create", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["codigo"] = " OP01 ",
                ["nombre"] = " Corte "
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        Assert.Equal(15, data["id"]);
        Assert.Equal("OP01", data["codigo"]);
        Assert.Equal("Corte", data["nombre"]);
        Assert.Equal(true, data["activa"]);
    }

    [Fact]
    public async Task RunAsync_operacionesCreate_duplicateCode_returnsDuplicate()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) =>
            new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["resultCode"] = "DUPLICATE_CODE",
                        ["id"] = null,
                        ["codigo"] = null,
                        ["nombre"] = null,
                        ["activa"] = null
                    }
                }
            }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Operaciones.Create", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["codigo"] = "OP01",
                ["nombre"] = "Corte",
                ["activa"] = false
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("DUPLICATE_CODE", outcome.ErrorCode);
    }

    [Fact]
    public async Task RunAsync_operacionesUpdate_mapsPayload()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_OperacionesUpdate", sp);
            Assert.Equal(15, spParams["IdOperacion"]);
            Assert.Equal("Corte fino", spParams["Nombre"]);
            Assert.Equal(false, spParams["Activa"]);
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["resultCode"] = "OK",
                        ["id"] = 15,
                        ["codigo"] = "OP01",
                        ["nombre"] = "Corte fino",
                        ["activa"] = false
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Operaciones.Update", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_operacion"] = 15,
                ["nombre"] = " Corte fino ",
                ["activa"] = false
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        Assert.Equal(15, data["id"]);
        Assert.Equal("OP01", data["codigo"]);
        Assert.Equal("Corte fino", data["nombre"]);
        Assert.Equal(false, data["activa"]);
    }

    [Fact]
    public async Task RunAsync_operacionesDelete_mapsPayload()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_OperacionesDelete", sp);
            Assert.Equal(15, spParams["IdOperacion"]);
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["resultCode"] = "OK",
                        ["id"] = 15,
                        ["eliminado"] = true
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Operaciones.Delete", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_operacion"] = 15
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        Assert.Equal(15, data["id"]);
        Assert.Equal(true, data["eliminado"]);
    }

    [Fact]
    public async Task RunAsync_operacionesDelete_referenced_returnsConflictCode()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) =>
            new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["resultCode"] = "REFERENCED",
                        ["id"] = 15,
                        ["eliminado"] = false
                    }
                }
            }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Operaciones.Delete", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_operacion"] = 15
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("REFERENCED", outcome.ErrorCode);
    }

    [Fact]
    public async Task RunAsync_operacionesGet_emptyResult_returnsNotFound()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) =>
            new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>()
            }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Operaciones.Get", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_operacion"] = 99
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("NOT_FOUND", outcome.ErrorCode);
    }

    [Fact]
    public async Task RunAsync_turnosGet_mapsPayload()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_TurnosGet", sp);
            Assert.Equal(7, spParams["IdTurno"]);
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["id"] = 7,
                        ["codigo"] = "TM",
                        ["nombre"] = "Manana",
                        ["hora_inicio"] = "06:00:00",
                        ["hora_fin"] = "14:00:00",
                        ["activo"] = true
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Turnos.Get", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_turno"] = 7
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        Assert.Equal(7, data["id"]);
        Assert.Equal("TM", data["codigo"]);
        Assert.Equal("Manana", data["nombre"]);
        Assert.Equal("06:00:00", data["hora_inicio"]);
        Assert.Equal("14:00:00", data["hora_fin"]);
        Assert.Equal(true, data["activo"]);
    }

    [Fact]
    public async Task RunAsync_turnosCreate_mapsPayloadAndNormalizesHours()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_TurnosCreate", sp);
            Assert.Equal("TM", spParams["Codigo"]);
            Assert.Equal("Manana", spParams["Nombre"]);
            Assert.Equal("06:00:00", spParams["HoraInicio"]);
            Assert.Equal("23:59:59", spParams["HoraFin"]);
            Assert.Equal(true, spParams["Activo"]);
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["resultCode"] = "OK",
                        ["id"] = 21,
                        ["codigo"] = "TM",
                        ["nombre"] = "Manana",
                        ["hora_inicio"] = "06:00:00",
                        ["hora_fin"] = "23:59:59",
                        ["activo"] = true
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Turnos.Create", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["codigo"] = " TM ",
                ["nombre"] = " Manana ",
                ["hora_inicio"] = "6:00",
                ["hora_fin"] = "24:00"
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        Assert.Equal(21, data["id"]);
        Assert.Equal("TM", data["codigo"]);
        Assert.Equal("Manana", data["nombre"]);
        Assert.Equal("06:00:00", data["hora_inicio"]);
        Assert.Equal("23:59:59", data["hora_fin"]);
        Assert.Equal(true, data["activo"]);
    }

    [Fact]
    public async Task RunAsync_turnosCreate_invalidSchedule_returnsInvalidSchedule()
    {
        var called = false;
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) =>
        {
            called = true;
            throw new InvalidOperationException("SP no debe ejecutarse");
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Turnos.Create", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["codigo"] = "TM",
                ["nombre"] = "Manana",
                ["hora_inicio"] = "14:00:00",
                ["hora_fin"] = "06:00:00"
            },
            30,
            CancellationToken.None);

        Assert.False(called);
        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_SCHEDULE", outcome.ErrorCode);
    }

    [Fact]
    public async Task RunAsync_turnosUpdate_mapsPayloadAndSetHoraFlags()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_TurnosUpdate", sp);
            Assert.Equal(21, spParams["IdTurno"]);
            Assert.Equal("Manana extendida", spParams["Nombre"]);
            Assert.Equal("07:30:00", spParams["HoraInicio"]);
            Assert.Equal(true, spParams["SetHoraInicio"]);
            Assert.Equal(false, spParams["SetHoraFin"]);
            Assert.False(spParams.ContainsKey("HoraFin"));
            Assert.Equal(false, spParams["Activo"]);
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["resultCode"] = "OK",
                        ["id"] = 21,
                        ["codigo"] = "TM",
                        ["nombre"] = "Manana extendida",
                        ["hora_inicio"] = "07:30:00",
                        ["hora_fin"] = "14:00:00",
                        ["activo"] = false
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Turnos.Update", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_turno"] = 21,
                ["nombre"] = " Manana extendida ",
                ["hora_inicio"] = "7:30",
                ["activo"] = false
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        Assert.Equal(21, data["id"]);
        Assert.Equal("TM", data["codigo"]);
        Assert.Equal("Manana extendida", data["nombre"]);
        Assert.Equal("07:30:00", data["hora_inicio"]);
        Assert.Equal("14:00:00", data["hora_fin"]);
        Assert.Equal(false, data["activo"]);
    }

    [Fact]
    public async Task RunAsync_turnosDelete_mapsPayload()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_TurnosDelete", sp);
            Assert.Equal(21, spParams["IdTurno"]);
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["resultCode"] = "OK",
                        ["id"] = 21,
                        ["eliminado"] = true
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Turnos.Delete", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_turno"] = 21
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        Assert.Equal(21, data["id"]);
        Assert.Equal(true, data["eliminado"]);
    }

    [Fact]
    public async Task RunAsync_turnosDelete_referenced_returnsConflictCode()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) =>
            new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["resultCode"] = "REFERENCED",
                        ["id"] = 21,
                        ["eliminado"] = false
                    }
                }
            }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Turnos.Delete", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_turno"] = 21
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("REFERENCED", outcome.ErrorCode);
    }

    [Fact]
    public async Task RunAsync_conceptosTiempoGet_mapsPayload()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_ConceptosTiempoGet", sp);
            Assert.Equal(9, spParams["IdConceptoTiempo"]);
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["id"] = 9,
                        ["codigo"] = "PROD",
                        ["nombre"] = "Productivo",
                        ["es_productivo"] = true,
                        ["activo"] = true,
                        ["habitual"] = true,
                        ["id_tipo_tarea"] = 3,
                        ["tipo_tarea_codigo"] = "TT",
                        ["tipo_tarea_nombre"] = "Tarea",
                        ["tipo_tarea_label"] = "TT — Tarea"
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.ConceptosTiempo.Get", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_concepto_tiempo"] = 9
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        Assert.Equal(9, data["id"]);
        Assert.Equal("PROD", data["codigo"]);
        Assert.Equal("Productivo", data["nombre"]);
        Assert.Equal(true, data["es_productivo"]);
        Assert.Equal(true, data["activo"]);
        Assert.Equal(true, data["habitual"]);
        Assert.Equal(3, data["id_tipo_tarea"]);
        Assert.Equal("TT", data["tipo_tarea_codigo"]);
        Assert.Equal("Tarea", data["tipo_tarea_nombre"]);
        Assert.Equal("TT — Tarea", data["tipo_tarea_label"]);
        Assert.False(data.ContainsKey("aviso_habitual"));
    }

    [Fact]
    public async Task RunAsync_conceptosTiempoCreate_mapsPayload()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_ConceptosTiempoCreate", sp);
            Assert.Equal("PROD", spParams["Codigo"]);
            Assert.Equal("Productivo", spParams["Nombre"]);
            Assert.Equal(true, spParams["EsProductivo"]);
            Assert.Equal(true, spParams["Activo"]);
            Assert.Equal(false, spParams["Habitual"]);
            Assert.Equal(3, spParams["IdTipoTarea"]);
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["resultCode"] = "OK",
                        ["id"] = 31,
                        ["codigo"] = "PROD",
                        ["nombre"] = "Productivo",
                        ["es_productivo"] = true,
                        ["activo"] = true,
                        ["habitual"] = false,
                        ["id_tipo_tarea"] = 3,
                        ["tipo_tarea_codigo"] = "TT",
                        ["tipo_tarea_nombre"] = "Tarea",
                        ["tipo_tarea_label"] = "TT — Tarea",
                        ["aviso_reemplazado"] = null,
                        ["aviso_concepto_id"] = null,
                        ["aviso_concepto_codigo"] = null,
                        ["aviso_concepto_nombre"] = null
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.ConceptosTiempo.Create", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["codigo"] = " PROD ",
                ["nombre"] = " Productivo ",
                ["id_tipo_tarea"] = 3
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        Assert.Equal(31, data["id"]);
        Assert.Equal("PROD", data["codigo"]);
        Assert.Equal(false, data["habitual"]);
        Assert.Equal(3, data["id_tipo_tarea"]);
        Assert.False(data.ContainsKey("aviso_habitual"));
    }

    [Fact]
    public async Task RunAsync_conceptosTiempoCreate_habitualWithAviso_mapsAvisoHabitual()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_ConceptosTiempoCreate", sp);
            Assert.Equal(true, spParams["Habitual"]);
            Assert.Equal(3, spParams["IdTipoTarea"]);
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["resultCode"] = "OK",
                        ["id"] = 32,
                        ["codigo"] = "HAB2",
                        ["nombre"] = "Nuevo habitual",
                        ["es_productivo"] = true,
                        ["activo"] = true,
                        ["habitual"] = true,
                        ["id_tipo_tarea"] = 3,
                        ["tipo_tarea_codigo"] = "TT",
                        ["tipo_tarea_nombre"] = "Tarea",
                        ["tipo_tarea_label"] = "TT — Tarea",
                        ["aviso_reemplazado"] = true,
                        ["aviso_concepto_id"] = 11,
                        ["aviso_concepto_codigo"] = "HAB1",
                        ["aviso_concepto_nombre"] = "Habitual previo"
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.ConceptosTiempo.Create", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["codigo"] = "HAB2",
                ["nombre"] = "Nuevo habitual",
                ["habitual"] = true,
                ["id_tipo_tarea"] = 3
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        Assert.Equal(32, data["id"]);
        Assert.Equal(true, data["habitual"]);
        var aviso = Assert.IsType<Dictionary<string, object?>>(data["aviso_habitual"]);
        Assert.Equal(true, aviso["reemplazado"]);
        var anterior = Assert.IsType<Dictionary<string, object?>>(aviso["concepto_anterior"]);
        Assert.Equal(11, anterior["id"]);
        Assert.Equal("HAB1", anterior["codigo"]);
        Assert.Equal("Habitual previo", anterior["nombre"]);
    }

    [Fact]
    public async Task RunAsync_conceptosTiempoUpdate_mapsPayloadAndSetIdTipoFlag()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_ConceptosTiempoUpdate", sp);
            Assert.Equal(31, spParams["IdConceptoTiempo"]);
            Assert.Equal("Productivo editado", spParams["Nombre"]);
            Assert.Equal(false, spParams["Activo"]);
            Assert.Equal(true, spParams["SetIdTipoTarea"]);
            Assert.Equal(5, spParams["IdTipoTarea"]);
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["resultCode"] = "OK",
                        ["id"] = 31,
                        ["codigo"] = "PROD",
                        ["nombre"] = "Productivo editado",
                        ["es_productivo"] = true,
                        ["activo"] = false,
                        ["habitual"] = false,
                        ["id_tipo_tarea"] = 5,
                        ["tipo_tarea_codigo"] = "OT",
                        ["tipo_tarea_nombre"] = "Otra",
                        ["tipo_tarea_label"] = "OT — Otra",
                        ["aviso_reemplazado"] = null
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.ConceptosTiempo.Update", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_concepto_tiempo"] = 31,
                ["nombre"] = " Productivo editado ",
                ["activo"] = false,
                ["id_tipo_tarea"] = 5
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        Assert.Equal(31, data["id"]);
        Assert.Equal("Productivo editado", data["nombre"]);
        Assert.Equal(false, data["activo"]);
        Assert.Equal(5, data["id_tipo_tarea"]);
        Assert.Equal("OT — Otra", data["tipo_tarea_label"]);
    }

    [Fact]
    public async Task RunAsync_conceptosTiempoDelete_mapsPayload()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_ConceptosTiempoDelete", sp);
            Assert.Equal(31, spParams["IdConceptoTiempo"]);
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["resultCode"] = "OK",
                        ["id"] = 31,
                        ["eliminado"] = true
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.ConceptosTiempo.Delete", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_concepto_tiempo"] = 31
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        Assert.Equal(31, data["id"]);
        Assert.Equal(true, data["eliminado"]);
    }

    [Fact]
    public async Task RunAsync_conceptosTiempoDelete_referenced_returnsConflictCode()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) =>
            new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["resultCode"] = "REFERENCED",
                        ["id"] = 31,
                        ["eliminado"] = false
                    }
                }
            }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.ConceptosTiempo.Delete", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_concepto_tiempo"] = 31
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("REFERENCED", outcome.ErrorCode);
    }

    [Fact]
    public async Task RunAsync_maquinasList_mapsParamsAndEnvelope()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_MaquinasList", sp);
            Assert.Equal(true, spParams["FilterActiva"]);
            Assert.Equal("M0", spParams["FilterCodigo"]);
            Assert.Equal("Fre", spParams["FilterNombre"]);
            Assert.Equal("CODIGO_MAQUINA", spParams["Sort"]);
            Assert.Equal("asc", spParams["Dir"]);
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["id"] = 7,
                        ["codigo"] = "M01",
                        ["nombre"] = "Fresadora",
                        ["activa"] = true
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Maquinas.List", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["filter_activa"] = true,
                ["filter_codigo"] = "M0",
                ["filter_nombre"] = "Fre",
                ["sort"] = "CODIGO_MAQUINA",
                ["dir"] = "asc"
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        var items = Assert.IsAssignableFrom<IEnumerable<Dictionary<string, object?>>>(data["items"]).ToList();
        Assert.Single(items);
        Assert.Equal(7, items[0]["id"]);
        Assert.Equal(1, data["page"]);
        Assert.Equal(1, data["page_size"]);
        Assert.Equal(1, data["total"]);
        Assert.Equal(1, data["total_pages"]);
    }

    [Fact]
    public async Task RunAsync_maquinasList_emptyTable_returnsEmptyEnvelope()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) =>
            new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>()
            }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Maquinas.List", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?> { ["_database"] = "EMP" },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        var items = Assert.IsAssignableFrom<IEnumerable<Dictionary<string, object?>>>(data["items"]).ToList();
        Assert.Empty(items);
        Assert.Equal(0, data["page_size"]);
        Assert.Equal(0, data["total"]);
        Assert.Equal(1, data["total_pages"]);
    }

    [Fact]
    public async Task RunAsync_tiposTareaList_mapsParamsAndEnvelope()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_TiposTareaList", sp);
            Assert.Equal(false, spParams["FilterActivo"]);
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["id"] = 2,
                        ["codigo"] = "TT1",
                        ["nombre"] = "Setup",
                        ["activo"] = false
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.TiposTarea.List", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["filter_activo"] = false
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        var items = Assert.IsAssignableFrom<IEnumerable<Dictionary<string, object?>>>(data["items"]).ToList();
        Assert.Single(items);
        Assert.Equal("TT1", items[0]["codigo"]);
        Assert.Equal(false, items[0]["activo"]);
    }

    [Fact]
    public async Task RunAsync_operacionesList_emptyTable_returnsEmptyEnvelope()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, _) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_OperacionesList", sp);
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>()
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Operaciones.List", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?> { ["_database"] = "EMP" },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        Assert.Equal(0, data["total"]);
    }

    [Fact]
    public async Task RunAsync_turnosList_mapsHoursEnvelope()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, _) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_TurnosList", sp);
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["id"] = 1,
                        ["codigo"] = "T1",
                        ["nombre"] = "Mañana",
                        ["hora_inicio"] = "06:00:00",
                        ["hora_fin"] = "14:00:00",
                        ["activo"] = true
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Turnos.List", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?> { ["_database"] = "EMP" },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        var items = Assert.IsAssignableFrom<IEnumerable<Dictionary<string, object?>>>(data["items"]).ToList();
        Assert.Equal("06:00:00", items[0]["hora_inicio"]);
        Assert.Equal("14:00:00", items[0]["hora_fin"]);
    }

    [Fact]
    public async Task RunAsync_conceptosTiempoList_mapsFiltersAndTipoTarea()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_ConceptosTiempoList", sp);
            Assert.Equal(true, spParams["FilterActivo"]);
            Assert.Equal(true, spParams["FilterEsProductivo"]);
            Assert.Equal(5, spParams["FilterIdTipoTarea"]);
            Assert.Equal(true, spParams["FilterHabitual"]);
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["id"] = 9,
                        ["codigo"] = "CT1",
                        ["nombre"] = "Productivo",
                        ["es_productivo"] = true,
                        ["activo"] = true,
                        ["habitual"] = true,
                        ["id_tipo_tarea"] = 5,
                        ["tipo_tarea_codigo"] = "TT1",
                        ["tipo_tarea_nombre"] = "Setup",
                        ["tipo_tarea_label"] = "TT1 — Setup"
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.ConceptosTiempo.List", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["filter_activo"] = true,
                ["filter_es_productivo"] = true,
                ["filter_id_tipo_tarea"] = 5,
                ["filter_habitual"] = true
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        var items = Assert.IsAssignableFrom<IEnumerable<Dictionary<string, object?>>>(data["items"]).ToList();
        Assert.Single(items);
        Assert.Equal(5, items[0]["id_tipo_tarea"]);
        Assert.Equal("TT1 — Setup", items[0]["tipo_tarea_label"]);
    }

    [Fact]
    public async Task RunAsync_conceptosTiempoList_emptyTable_returnsEmptyEnvelope()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) =>
            new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>()
            }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.ConceptosTiempo.List", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?> { ["_database"] = "EMP" },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        Assert.Equal(0, data["total"]);
        Assert.Equal(1, data["total_pages"]);
    }

    [Fact]
    public async Task RunAsync_ordenesTrabajoList_mapsParamsAgrupadoAndEnvelope()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_OrdenesTrabajoList", sp);
            Assert.Equal(true, spParams["Agrupado"]);
            Assert.Equal(1, spParams["Page"]);
            Assert.Equal(50, spParams["PageSize"]);
            Assert.Equal(1, spParams["FilterEstado"]);
            Assert.Equal("OT", spParams["FilterCodigo"]);
            Assert.Equal("prod", spParams["FilterDescripcion"]);
            Assert.Equal("2026-01-01", spParams["FilterFechaDesde"]);
            Assert.Equal("2026-12-31", spParams["FilterFechaHasta"]);
            Assert.Equal("CODIGO_OT", spParams["Sort"]);
            Assert.Equal("ASC", spParams["Dir"]);
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["page"] = 1,
                        ["page_size"] = 50,
                        ["total"] = 1,
                        ["total_pages"] = 1,
                        ["agrupado"] = true
                    }
                },
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["id"] = 10,
                        ["id_orden_trabajo_representativo"] = 10,
                        ["codigo"] = "OT-001",
                        ["descripcion"] = "Producto A",
                        ["id_articulo"] = 5,
                        ["articulo_codigo"] = "ART1",
                        ["articulo_label"] = "ART1 – Articulo 1",
                        ["cantidad_a_producir"] = 100,
                        ["fecha_inicio_plan"] = new DateTime(2026, 1, 2),
                        ["fecha_fin_plan"] = new DateTime(2026, 1, 10),
                        ["estado"] = 1,
                        ["estado_label"] = "Abierta",
                        ["observaciones"] = null,
                        ["fecha_alta"] = new DateTime(2026, 1, 1, 8, 30, 0)
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.OrdenesTrabajo.List", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["agrupado"] = true,
                ["page"] = 1,
                ["page_size"] = 50,
                ["filter_estado"] = 1,
                ["filter_codigo"] = "OT",
                ["filter_descripcion"] = "prod",
                ["filter_fecha_desde"] = "2026-01-01",
                ["filter_fecha_hasta"] = "2026-12-31",
                ["sort"] = "CODIGO_OT",
                ["dir"] = "ASC"
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        var items = Assert.IsAssignableFrom<IEnumerable<Dictionary<string, object?>>>(data["items"]).ToList();
        Assert.Single(items);
        Assert.Equal(10, items[0]["id"]);
        Assert.Equal(10, items[0]["id_orden_trabajo_representativo"]);
        Assert.Equal("OT-001", items[0]["codigo"]);
        Assert.Equal("2026-01-02", items[0]["fecha_inicio_plan"]);
        Assert.Equal(1, data["page"]);
        Assert.Equal(50, data["page_size"]);
        Assert.Equal(1, data["total"]);
        Assert.Equal(1, data["total_pages"]);
        Assert.False(items[0].ContainsKey("id_operacion"));
    }

    [Fact]
    public async Task RunAsync_ordenesTrabajoList_porFila_mapsOperacion()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_OrdenesTrabajoList", sp);
            Assert.Equal(false, spParams["Agrupado"]);
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["page"] = 1,
                        ["page_size"] = 20,
                        ["total"] = 1,
                        ["total_pages"] = 1,
                        ["agrupado"] = false
                    }
                },
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["id"] = 11,
                        ["codigo"] = "OT-001",
                        ["descripcion"] = "Producto A",
                        ["id_operacion"] = 3,
                        ["operacion_codigo"] = "OP1",
                        ["operacion_nombre"] = "Corte",
                        ["operacion_label"] = "OP1 – Corte",
                        ["cantidad_a_producir"] = 50,
                        ["estado"] = 0,
                        ["estado_label"] = "Borrador",
                        ["nro_orden"] = 2
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.OrdenesTrabajo.List", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["agrupado"] = false,
                ["page"] = 1,
                ["page_size"] = 20
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        var items = Assert.IsAssignableFrom<IEnumerable<Dictionary<string, object?>>>(data["items"]).ToList();
        Assert.Single(items);
        Assert.Equal(3, items[0]["id_operacion"]);
        Assert.Equal("OP1 – Corte", items[0]["operacion_label"]);
        Assert.Equal(2, items[0]["nro_orden"]);
        Assert.False(items[0].ContainsKey("id_orden_trabajo_representativo"));
    }

    [Fact]
    public async Task RunAsync_ordenesTrabajoList_emptyTable_returnsEmptyEnvelope()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) =>
            new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["page"] = 1,
                        ["page_size"] = 50,
                        ["total"] = 0,
                        ["total_pages"] = 0,
                        ["agrupado"] = true
                    }
                },
                new List<Dictionary<string, object?>>()
            }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.OrdenesTrabajo.List", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?> { ["_database"] = "EMP" },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        var items = Assert.IsAssignableFrom<IEnumerable<Dictionary<string, object?>>>(data["items"]).ToList();
        Assert.Empty(items);
        Assert.Equal(50, data["page_size"]);
        Assert.Equal(0, data["total"]);
        Assert.Equal(0, data["total_pages"]);
    }

    [Fact]
    public async Task RunAsync_asignacionesList_mapsParamsAndEnvelope()
    {
        IReadOnlyDictionary<string, object?>? captured = null;
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_AsignacionesList", sp);
            captured = spParams;
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["page"] = 2,
                        ["page_size"] = 25,
                        ["total"] = 26,
                        ["total_pages"] = 2
                    }
                },
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["id"] = 7,
                        ["fecha_asignacion"] = new DateTime(2026, 9, 14),
                        ["id_turno"] = 3,
                        ["turno_codigo"] = "T1",
                        ["id_tipo_tarea"] = 4,
                        ["tipo_tarea_codigo"] = "PROD",
                        ["tipo_tarea_nombre"] = "Producción",
                        ["supervisor_id"] = 12,
                        ["supervisor_codigo"] = null,
                        ["supervisor_nombre"] = null,
                        ["supervisor_label"] = null,
                        ["estado"] = 1,
                        ["estado_label"] = "Publicada",
                        ["fecha_publicacion"] = new DateTime(2026, 9, 14, 10, 0, 0),
                        ["fecha_cierre"] = null
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Asignaciones.List", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["page"] = 2,
                ["page_size"] = 25,
                ["sort"] = "FECHA_ASIGNACION",
                ["dir"] = "DESC",
                ["filter_fecha_desde"] = "2026-09-01",
                ["filter_fecha_hasta"] = "2026-09-30",
                ["filter_id_turno"] = 3,
                ["filter_id_tipo_tarea"] = 4,
                ["filter_estado"] = 1
            },
            30,
            CancellationToken.None);

        Assert.NotNull(captured);
        Assert.Equal(2, captured!["Page"]);
        Assert.Equal(25, captured["PageSize"]);
        Assert.Equal("FECHA_ASIGNACION", captured["Sort"]);
        Assert.Equal("DESC", captured["Dir"]);
        Assert.Equal("2026-09-01", captured["FilterFechaDesde"]);
        Assert.Equal("2026-09-30", captured["FilterFechaHasta"]);
        Assert.Equal(3, captured["FilterIdTurno"]);
        Assert.Equal(4, captured["FilterIdTipoTarea"]);
        Assert.Equal(1, captured["FilterEstado"]);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        Assert.Equal(2, data["page"]);
        Assert.Equal(25, data["page_size"]);
        Assert.Equal(26, data["total"]);
        Assert.Equal(2, data["total_pages"]);
        var items = Assert.IsAssignableFrom<IEnumerable<Dictionary<string, object?>>>(data["items"]).ToList();
        Assert.Single(items);
        Assert.Equal(7, items[0]["id"]);
        Assert.Equal("2026-09-14", items[0]["fecha_asignacion"]);
        Assert.Equal("T1", items[0]["turno_codigo"]);
        Assert.Equal("PROD", items[0]["tipo_tarea_codigo"]);
        Assert.Equal("Producción", items[0]["tipo_tarea_nombre"]);
        Assert.Equal(12, items[0]["supervisor_id"]);
        Assert.Null(items[0]["supervisor_label"]);
        Assert.Equal(1, items[0]["estado"]);
        Assert.Equal("Publicada", items[0]["estado_label"]);
        Assert.Equal("2026-09-14 10:00:00", items[0]["fecha_publicacion"]);
        Assert.False(items[0].ContainsKey("observaciones"));
    }

    [Fact]
    public async Task RunAsync_asignacionesList_emptyTable_returnsEmptyEnvelope()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) =>
            new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["page"] = 1,
                        ["page_size"] = 50,
                        ["total"] = 0,
                        ["total_pages"] = 0
                    }
                },
                new List<Dictionary<string, object?>>()
            }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Asignaciones.List", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?> { ["_database"] = "EMP" },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        var items = Assert.IsAssignableFrom<IEnumerable<Dictionary<string, object?>>>(data["items"]).ToList();
        Assert.Empty(items);
        Assert.Equal(50, data["page_size"]);
        Assert.Equal(0, data["total"]);
        Assert.Equal(0, data["total_pages"]);
    }

    [Fact]
    public async Task RunAsync_asignacionesList_fechaDesdeMayorHasta_returnsValidation()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) =>
            throw new InvalidOperationException("no call")));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Asignaciones.List", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["filter_fecha_desde"] = "2026-09-30",
                ["filter_fecha_hasta"] = "2026-09-01"
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("VALIDATION", outcome.ErrorCode);
        Assert.Contains("fecha desde", outcome.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RunAsync_asignacionesGet_mapsParamsAndPayloadWithObservaciones()
    {
        IReadOnlyDictionary<string, object?>? captured = null;
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_AsignacionesGet", sp);
            captured = spParams;
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["id"] = 7,
                        ["fecha_asignacion"] = new DateTime(2026, 9, 14),
                        ["id_turno"] = 3,
                        ["turno_codigo"] = "T1",
                        ["id_tipo_tarea"] = 4,
                        ["tipo_tarea_codigo"] = "PROD",
                        ["tipo_tarea_nombre"] = "Producción",
                        ["supervisor_id"] = 12,
                        ["supervisor_codigo"] = null,
                        ["supervisor_nombre"] = null,
                        ["supervisor_label"] = null,
                        ["estado"] = 0,
                        ["estado_label"] = "Borrador",
                        ["fecha_publicacion"] = null,
                        ["fecha_cierre"] = null,
                        ["observaciones"] = "Nota show"
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Asignaciones.Get", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_asignacion"] = 7
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        Assert.NotNull(captured);
        Assert.Equal(7, captured!["IdAsignacion"]);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        Assert.Equal(7, data["id"]);
        Assert.Equal("2026-09-14", data["fecha_asignacion"]);
        Assert.Equal("Nota show", data["observaciones"]);
        Assert.Null(data["supervisor_label"]);
        Assert.Equal("Borrador", data["estado_label"]);
    }

    [Fact]
    public async Task RunAsync_asignacionesGet_empty_returnsNotFound()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) =>
            new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                Array.Empty<Dictionary<string, object?>>()
            }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.Asignaciones.Get", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_asignacion"] = 999
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("NOT_FOUND", outcome.ErrorCode);
    }

    [Fact]
    public async Task RunAsync_partesOperarioGet_mapsParamsAndPayload()
    {
        IReadOnlyDictionary<string, object?>? captured = null;
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_PartesOperarioGet", sp);
            captured = spParams;
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["no_legajo"] = 0,
                        ["not_found"] = 0
                    }
                },
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["id"] = 42,
                        ["fecha_parte"] = new DateTime(2026, 9, 15),
                        ["id_turno"] = 3,
                        ["turno_nombre"] = "Mañana",
                        ["estado"] = 0,
                        ["observaciones"] = "Obs show"
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.Get", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_parte_operario"] = 42,
                ["usuario_id"] = 7
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        Assert.NotNull(captured);
        Assert.Equal(42, captured!["IdParteOperario"]);
        Assert.Equal(7, captured["UsuarioId"]);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        Assert.Equal(42, data["id"]);
        Assert.Equal("2026-09-15", data["fecha_parte"]);
        Assert.Equal(3, data["id_turno"]);
        Assert.Equal("Mañana", data["turno_nombre"]);
        Assert.Equal(0, data["estado"]);
        Assert.Equal("Obs show", data["observaciones"]);
    }

    [Fact]
    public async Task RunAsync_partesOperarioGet_noLegajo_returnsNoLegajo()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) =>
            new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["no_legajo"] = 1,
                        ["not_found"] = 0
                    }
                },
                Array.Empty<Dictionary<string, object?>>()
            }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.Get", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_parte_operario"] = 1,
                ["usuario_id"] = 99
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("NO_LEGAJO", outcome.ErrorCode);
    }

    [Fact]
    public async Task RunAsync_partesOperarioGet_notFound_returnsNotFound()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) =>
            new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["no_legajo"] = 0,
                        ["not_found"] = 1
                    }
                },
                Array.Empty<Dictionary<string, object?>>()
            }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.Get", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_parte_operario"] = 999,
                ["usuario_id"] = 7
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("NOT_FOUND", outcome.ErrorCode);
    }

    [Fact]
    public async Task RunAsync_partesOperarioGet_missingUsuario_returnsInvalidParameters()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) => throw new InvalidOperationException("no call")));
        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.Get", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_parte_operario"] = 1
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
    }

    [Fact]
    public async Task RunAsync_parteContexto_mapsParamsAndPayload()
    {
        IReadOnlyDictionary<string, object?>? captured = null;
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_PartesOperarioParteContexto", sp);
            captured = spParams;
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["id_parte_operario"] = 88
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.ParteContexto", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["fecha_parte"] = "2026-09-15",
                ["id_turno"] = 3,
                ["usuario_id"] = 7
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        Assert.NotNull(captured);
        Assert.Equal(7, captured!["UsuarioId"]);
        Assert.Equal(3, captured["IdTurno"]);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        Assert.Equal(88, data["id_parte_operario"]);
    }

    [Fact]
    public async Task RunAsync_parteContexto_empty_returnsNullId()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) =>
            new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["id_parte_operario"] = null
                    }
                }
            }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.ParteContexto", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["fecha_parte"] = "2026-09-15",
                ["usuario_id"] = 7
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        Assert.Null(data["id_parte_operario"]);
    }

    [Fact]
    public async Task RunAsync_parteContexto_missingFecha_returnsInvalidParameters()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) => throw new InvalidOperationException("no call")));
        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.ParteContexto", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["usuario_id"] = 7
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
    }

    [Fact]
    public async Task RunAsync_misPartesList_mapsEnvelope()
    {
        IReadOnlyDictionary<string, object?>? captured = null;
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_PartesOperarioMisPartesList", sp);
            captured = spParams;
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["page"] = 1,
                        ["page_size"] = 50,
                        ["total"] = 1,
                        ["total_pages"] = 1,
                        ["planificado_page"] = 1,
                        ["planificado_page_size"] = 50,
                        ["planificado_total"] = 1,
                        ["planificado_total_pages"] = 1,
                        ["autenticado"] = true,
                        ["id_usuario"] = 7,
                        ["tabla_legajos_existe"] = true,
                        ["tiene_legajo_vinculado"] = true,
                        ["id_legajo_pk_usado_como_id_operario"] = 12,
                        ["legajo_nro"] = "001",
                        ["legajo_apellido"] = "Perez",
                        ["legajo_nombre"] = "Juan",
                        ["tabla_partes_operario_existe"] = true,
                        ["tablas_planificado_existen"] = true,
                        ["filter_fecha_desde"] = new DateTime(2026, 9, 1),
                        ["filter_id_turno"] = 3
                    }
                },
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["id"] = 42,
                        ["fecha_parte"] = new DateTime(2026, 9, 15),
                        ["id_turno"] = 3,
                        ["turno_codigo"] = "T1",
                        ["turno_nombre"] = "Mañana",
                        ["estado"] = 0,
                        ["fecha_apertura"] = new DateTime(2026, 9, 15, 8, 0, 0),
                        ["fecha_cierre"] = null,
                        ["observaciones"] = "ok"
                    }
                },
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["id_asignacion_item"] = 9,
                        ["id_asignacion"] = 4,
                        ["fecha_asignacion"] = new DateTime(2026, 9, 15),
                        ["supervisor_id"] = 20,
                        ["supervisor_label"] = null,
                        ["es_generada_usuario"] = 0,
                        ["registrado_en_parte"] = 1,
                        ["row_key"] = "plan-9"
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.MisPartes.List", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["usuario_id"] = 7,
                ["filter_fecha_desde"] = "2026-09-01",
                ["filter_id_turno"] = 3
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        Assert.Equal(7, captured!["UsuarioId"]);
        Assert.Equal("2026-09-01", captured["FilterFechaDesde"]?.ToString());
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        var items = Assert.IsAssignableFrom<IEnumerable<Dictionary<string, object?>>>(data["items"]).ToList();
        Assert.Single(items);
        Assert.Equal(42, items[0]["id"]);
        Assert.Equal("2026-09-15", items[0]["fecha_parte"]);
        var planificado = Assert.IsAssignableFrom<IEnumerable<Dictionary<string, object?>>>(data["planificado_items"]).ToList();
        Assert.Single(planificado);
        Assert.Equal("plan-9", planificado[0]["row_key"]);
        Assert.Equal(true, planificado[0]["registrado_en_parte"]);
        Assert.Null(planificado[0]["supervisor_label"]);
        var verificacion = Assert.IsType<Dictionary<string, object?>>(data["verificacion_operario"]);
        Assert.Equal(true, verificacion["tiene_legajo_vinculado"]);
        Assert.Equal(12, verificacion["id_legajo_pk_usado_como_id_operario"]);
        var filtros = Assert.IsType<Dictionary<string, object?>>(verificacion["filtros_solicitados"]);
        Assert.Equal("2026-09-01", filtros["fecha_desde"]);
        Assert.Equal(3, filtros["id_turno"]);
    }

    [Fact]
    public async Task RunAsync_misPartesList_missingUsuario_returnsInvalidParameters()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) => throw new InvalidOperationException("no call")));
        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.MisPartes.List", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?> { ["_database"] = "EMP" },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
    }

    [Fact]
    public async Task RunAsync_partesRevisarList_mapsEnvelope()
    {
        IReadOnlyDictionary<string, object?>? captured = null;
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_PartesOperarioPartesRevisarList", sp);
            captured = spParams;
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["page"] = 1,
                        ["page_size"] = 50,
                        ["total"] = 1,
                        ["total_pages"] = 1
                    }
                },
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["id"] = 15,
                        ["id_operario"] = 12,
                        ["operario_nombre"] = "Perez Juan",
                        ["fecha_parte"] = new DateTime(2026, 9, 15),
                        ["id_turno"] = 3,
                        ["turno_codigo"] = "T1",
                        ["turno_nombre"] = "Mañana",
                        ["estado"] = 1,
                        ["fecha_envio"] = new DateTime(2026, 9, 15, 18, 0, 0),
                        ["cantidad_entradas"] = 4
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.PartesRevisar.List", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["filter_estado"] = 1
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        Assert.Equal(1, captured!["FilterEstado"]);
        Assert.False(captured.ContainsKey("UsuarioId"));
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        var items = Assert.IsAssignableFrom<IEnumerable<Dictionary<string, object?>>>(data["items"]).ToList();
        Assert.Single(items);
        Assert.Equal(15, items[0]["id"]);
        Assert.Equal("Perez Juan", items[0]["operario_nombre"]);
        Assert.Equal(4, items[0]["cantidad_entradas"]);
        Assert.Equal("2026-09-15 18:00:00", items[0]["fecha_envio"]);
        Assert.Equal(50, data["page_size"]);
    }

    [Fact]
    public async Task RunAsync_partesRevisarGet_mapsHeaderAndEntradas()
    {
        IReadOnlyDictionary<string, object?>? captured = null;
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_PartesOperarioPartesRevisarGet", sp);
            captured = spParams;
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["id"] = 15,
                        ["id_operario"] = 12,
                        ["operario_nombre"] = "Perez Juan",
                        ["fecha_parte"] = new DateTime(2026, 9, 15),
                        ["id_turno"] = 3,
                        ["turno_nombre"] = "Mañana",
                        ["estado"] = 1,
                        ["observaciones"] = "Obs supervisor"
                    }
                },
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["id"] = 81,
                        ["id_concepto_tiempo"] = 4,
                        ["concepto_nombre"] = "Produccion",
                        ["minutos"] = 45,
                        ["unidades_hechas"] = 12.5m,
                        ["notas"] = "ok",
                        ["notas_revision"] = null
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.PartesRevisar.Get", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_parte_operario"] = 15
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        Assert.Equal(15, captured!["IdParteOperario"]);
        Assert.False(captured.ContainsKey("UsuarioId"));
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        Assert.Equal(15, data["id"]);
        Assert.Equal(12, data["id_operario"]);
        Assert.Equal("Perez Juan", data["operario_nombre"]);
        Assert.Equal("2026-09-15", data["fecha_parte"]);
        Assert.Equal("Obs supervisor", data["observaciones"]);
        var entradas = Assert.IsAssignableFrom<IEnumerable<Dictionary<string, object?>>>(data["entradas"]).ToList();
        Assert.Single(entradas);
        Assert.Equal(81, entradas[0]["id"]);
        Assert.Equal(4, entradas[0]["id_concepto_tiempo"]);
        Assert.Equal("Produccion", entradas[0]["concepto_nombre"]);
        Assert.Equal(45, entradas[0]["minutos"]);
        Assert.Equal(12.5, entradas[0]["unidades_hechas"]);
        Assert.Equal("ok", entradas[0]["notas"]);
        Assert.Null(entradas[0]["notas_revision"]);
    }

    [Fact]
    public async Task RunAsync_partesRevisarGet_notFound_returnsNotFound()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) =>
            new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                Array.Empty<Dictionary<string, object?>>(),
                Array.Empty<Dictionary<string, object?>>()
            }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.PartesRevisar.Get", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_parte_operario"] = 999
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("NOT_FOUND", outcome.ErrorCode);
    }

    [Fact]
    public async Task RunAsync_partesRevisarGet_missingId_returnsInvalidParameters()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) => throw new InvalidOperationException("no call")));
        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.PartesRevisar.Get", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?> { ["_database"] = "EMP" },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
    }

    [Fact]
    public async Task RunAsync_partesEntradasList_mapsItems()
    {
        IReadOnlyDictionary<string, object?>? captured = null;
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_PartesOperarioEntradasList", sp);
            captured = spParams;
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["no_legajo"] = 0,
                        ["not_found"] = 0
                    }
                },
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["id"] = 81,
                        ["id_asignacion_item"] = 12,
                        ["id_orden_trabajo"] = 40,
                        ["codigo_ot"] = "OT-40",
                        ["origen_carga"] = "plan",
                        ["id_maquina"] = 3,
                        ["maquina_etiqueta"] = "M1 — Prensa",
                        ["id_concepto_tiempo"] = 4,
                        ["concepto_nombre"] = "Produccion",
                        ["es_productivo"] = true,
                        ["minutos"] = 45,
                        ["fecha_hora_desde"] = new DateTime(2026, 9, 15, 8, 0, 0),
                        ["fecha_hora_hasta"] = new DateTime(2026, 9, 15, 8, 45, 0),
                        ["unidades_hechas"] = 12.5m,
                        ["unidades_merma"] = 0.5m,
                        ["unidades_retrabajo"] = 1m,
                        ["notas"] = "ok",
                        ["nro_orden_operacion"] = 2
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.Entradas.List", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_parte_operario"] = 15,
                ["usuario_id"] = 7
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        Assert.Equal(15, captured!["IdParteOperario"]);
        Assert.Equal(7, captured["UsuarioId"]);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        var items = Assert.IsAssignableFrom<IEnumerable<Dictionary<string, object?>>>(data["items"]).ToList();
        Assert.Single(items);
        Assert.Equal(81, items[0]["id"]);
        Assert.Equal(12, items[0]["id_asignacion_item"]);
        Assert.Equal("OT-40", items[0]["codigo_ot"]);
        Assert.Equal("plan", items[0]["origen_carga"]);
        Assert.Equal("M1 — Prensa", items[0]["maquina_etiqueta"]);
        Assert.Equal(true, items[0]["es_productivo"]);
        Assert.Equal(45, items[0]["minutos"]);
        Assert.Equal("2026-09-15 08:00:00", items[0]["fecha_hora_desde"]);
        Assert.Equal("2026-09-15 08:45:00", items[0]["fecha_hora_hasta"]);
        Assert.Equal(12.5, items[0]["unidades_hechas"]);
        Assert.Equal(0.5, items[0]["unidades_merma"]);
        Assert.Equal(1.0, items[0]["unidades_retrabajo"]);
        Assert.Equal("ok", items[0]["notas"]);
        Assert.Equal(2, items[0]["nro_orden_operacion"]);
    }

    [Fact]
    public async Task RunAsync_partesEntradasList_noLegajo_returnsNoLegajo()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) =>
            new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["no_legajo"] = 1,
                        ["not_found"] = 0
                    }
                },
                Array.Empty<Dictionary<string, object?>>()
            }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.Entradas.List", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_parte_operario"] = 15,
                ["usuario_id"] = 7
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("NO_LEGAJO", outcome.ErrorCode);
    }

    [Fact]
    public async Task RunAsync_partesEntradasList_notFound_returnsNotFound()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) =>
            new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["no_legajo"] = 0,
                        ["not_found"] = 1
                    }
                },
                Array.Empty<Dictionary<string, object?>>()
            }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.Entradas.List", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_parte_operario"] = 15,
                ["usuario_id"] = 7
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("NOT_FOUND", outcome.ErrorCode);
        Assert.Equal("Parte no encontrado o no editable", outcome.ErrorMessage);
    }

    [Fact]
    public async Task RunAsync_partesEntradasList_missingParams_returnsInvalidParameters()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) => throw new InvalidOperationException("no call")));
        Assert.True(PartesCatalog.TryGet("PartesProduccion.PartesOperario.Entradas.List", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_parte_operario"] = 15
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
    }

    [Fact]
    public async Task RunAsync_ordenesTrabajoGet_byId_mapsDetail()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_OrdenesTrabajoGet", sp);
            Assert.Equal(10, spParams["IdOrdenTrabajo"]);
            Assert.Null(spParams["CodigoOt"]);
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["id"] = 10,
                        ["codigo"] = "OT-001",
                        ["descripcion"] = "Producto A",
                        ["id_articulo"] = 5,
                        ["articulo_codigo"] = "ART1",
                        ["articulo_label"] = "ART1 – Articulo 1",
                        ["id_operacion"] = 3,
                        ["operacion_codigo"] = "OP1",
                        ["operacion_nombre"] = "Corte",
                        ["operacion_label"] = "OP1 – Corte",
                        ["cantidad_a_producir"] = 100,
                        ["fecha_inicio_plan"] = new DateTime(2026, 1, 2),
                        ["fecha_fin_plan"] = new DateTime(2026, 1, 10),
                        ["estado"] = 1,
                        ["estado_label"] = "Abierta",
                        ["observaciones"] = null,
                        ["fecha_alta"] = new DateTime(2026, 1, 1, 8, 30, 0),
                        ["nro_orden"] = 1
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.OrdenesTrabajo.Get", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_orden_trabajo"] = 10
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        Assert.Equal(10, data["id"]);
        Assert.Equal("OT-001", data["codigo"]);
        Assert.Equal(3, data["id_operacion"]);
        Assert.Equal("OP1 – Corte", data["operacion_label"]);
        Assert.Equal(1, data["nro_orden"]);
        Assert.Equal("2026-01-02", data["fecha_inicio_plan"]);
        Assert.False(data.ContainsKey("operaciones_incluidas"));
    }

    [Fact]
    public async Task RunAsync_ordenesTrabajoGet_byCodigo_mapsLote()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, sp, spParams) =>
        {
            Assert.Equal("dbo.PAQ_PartesProduccion_OrdenesTrabajoGet", sp);
            Assert.Equal(0, spParams["IdOrdenTrabajo"]);
            Assert.Equal("OT-001", spParams["CodigoOt"]);
            return new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>
                {
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["id"] = 10,
                        ["codigo"] = "OT-001",
                        ["descripcion"] = "Producto A",
                        ["id_articulo"] = 5,
                        ["articulo_label"] = "ART1 – Articulo 1",
                        ["id_operacion"] = 3,
                        ["operacion_label"] = "OP1 – Corte",
                        ["cantidad_a_producir"] = 100,
                        ["fecha_inicio_plan"] = "2026-01-02",
                        ["fecha_fin_plan"] = "2026-01-10",
                        ["estado"] = 1,
                        ["estado_label"] = "Abierta",
                        ["observaciones"] = null,
                        ["nro_orden"] = 1
                    },
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["id"] = 11,
                        ["codigo"] = "OT-001",
                        ["descripcion"] = "Producto A",
                        ["id_articulo"] = 5,
                        ["articulo_label"] = "ART1 – Articulo 1",
                        ["id_operacion"] = 4,
                        ["operacion_label"] = "OP2 – Armado",
                        ["cantidad_a_producir"] = 100,
                        ["estado"] = 2,
                        ["estado_label"] = "Cerrada",
                        ["nro_orden"] = 2
                    }
                }
            };
        }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.OrdenesTrabajo.Get", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["codigo_ot"] = "OT-001"
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        var data = Assert.IsType<Dictionary<string, object?>>(outcome.Data);
        Assert.Equal("OT-001", data["codigo"]);
        Assert.Equal(2, data["estado"]);
        Assert.Equal("Cerrada", data["estado_label"]);
        Assert.Equal(false, data["modo_individual"]);
        Assert.Equal(10, data["id_orden_trabajo_representativo"]);
        var ops = Assert.IsAssignableFrom<IEnumerable<Dictionary<string, object?>>>(data["operaciones_incluidas"]).ToList();
        Assert.Equal(2, ops.Count);
        Assert.Equal(3, ops[0]["id_operacion"]);
        Assert.Equal(10, ops[0]["id_orden_trabajo"]);
        Assert.Equal(4, ops[1]["id_operacion"]);
    }

    [Fact]
    public async Task RunAsync_ordenesTrabajoGet_missingParams_returnsInvalidParameters()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) => throw new InvalidOperationException("no call")));
        Assert.True(PartesCatalog.TryGet("PartesProduccion.OrdenesTrabajo.Get", out var def));

        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?> { ["_database"] = "EMP" },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INVALID_PARAMETERS", outcome.ErrorCode);
    }

    [Fact]
    public async Task RunAsync_ordenesTrabajoGet_emptyResult_returnsNotFound()
    {
        var runner = new PartesGatewayRunner(new FakeExecutor((_, _, _) =>
            new List<IReadOnlyList<Dictionary<string, object?>>>
            {
                new List<Dictionary<string, object?>>()
            }));

        Assert.True(PartesCatalog.TryGet("PartesProduccion.OrdenesTrabajo.Get", out var def));
        var outcome = await runner.RunAsync(
            def,
            LabOptionsWithSql(),
            new Dictionary<string, object?>
            {
                ["_database"] = "EMP",
                ["id_orden_trabajo"] = 99
            },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("NOT_FOUND", outcome.ErrorCode);
    }

    private static AgentOptions LabOptionsWithSql() =>
        new()
    {
            AgentId = "a",
            ClientId = "c",
            AgentToken = "t",
            GatewayUrl = "http://127.0.0.1:5100/agent-hub",
            Sql = new SqlOptions { Server = "localhost", Database = "dic", User = "sa", Password = "x" }
        };

    private sealed class FakeExecutor : IInformesSpExecutor
    {
        private readonly Func<string, string, IReadOnlyDictionary<string, object?>, IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>>> factory;

        public FakeExecutor(
            Func<string, string, IReadOnlyDictionary<string, object?>, IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>>> factory)
        {
            this.factory = factory;
        }

        public Task<IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>>> ExecuteAsync(
            string connectionString,
            string storedProcedure,
            IReadOnlyDictionary<string, object?> spParameters,
            int timeoutSeconds,
            CancellationToken cancellationToken) =>
            Task.FromResult(factory(connectionString, storedProcedure, spParameters));
    }
}
