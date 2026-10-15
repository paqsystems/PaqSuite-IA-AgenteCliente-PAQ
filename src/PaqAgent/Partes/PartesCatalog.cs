namespace PaqAgent.Partes;

public enum PartesResponseShape
{
    Parametros,
    InformesGestion,
    DashboardGet,
    MaquinaList,
    MaquinaGet,
    MaquinaCreate,
    MaquinaUpdate,
    MaquinaDelete,
    TipoTareaList,
    TipoTareaGet,
    TipoTareaCreate,
    TipoTareaUpdate,
    TipoTareaDelete,
    OperacionList,
    OperacionGet,
    OperacionCreate,
    OperacionUpdate,
    OperacionDelete,
    TurnoList,
    TurnoGet,
    TurnoCreate,
    TurnoUpdate,
    TurnoDelete,
    ConceptoTiempoList,
    ConceptoTiempoGet,
    ConceptoTiempoCreate,
    ConceptoTiempoUpdate,
    ConceptoTiempoDelete,
    OrdenTrabajoList,
    OrdenTrabajoGet,
    OrdenTrabajoCreate,
    OrdenTrabajoUpdate,
    OrdenTrabajoDelete,
    OrdenTrabajoPatchEstado,
    OrdenTrabajoCambioMasivoEstado,
    AsignacionList,
    AsignacionGet,
    AsignacionCreate,
    AsignacionUpdate,
    AsignacionPublicar,
    AsignacionCerrar,
    AsignacionCancelar,
    AsignacionItemList,
    AsignacionItemCreate,
    AsignacionItemUpdate,
    AsignacionItemDelete,
    AsignacionOperariosPlanList,
    AsignacionItemOperariosList,
    AsignacionItemOperariosCreate,
    AsignacionItemOperariosUpdate,
    AsignacionItemOperariosDelete,
    ParteOperarioGet,
    ParteOperarioCreate,
    ParteOperarioUpdate,
    ParteOperarioEnviar,
    ParteOperarioAprobar,
    ParteOperarioDevolver,
    ParteOperarioCerrar,
    ParteOperarioParteContexto,
    ParteOperarioMisPartesList,
    ParteOperarioPartesRevisarList,
    ParteOperarioPartesRevisarGet,
    ParteOperarioEntradasList,
    ParteOperarioEntradasCreate,
    ParteOperarioEntradasUpdate,
    ParteOperarioEntradasDelete,
    ParteOperarioEntradasReclasificar
}

public sealed record PartesOperationDefinition(
    string Operation,
    string StoredProcedure,
    IReadOnlyList<string> JobParameters,
    IReadOnlyDictionary<string, string> JobToSpParameterMap,
    PartesResponseShape Shape);

/// <summary>Catálogo D5 + D6.5 + D6.8 + D6.9 + D6.10 — Partes producción (params, informe, maestros, OT, Asignaciones, PartesOperario oleada A+B).</summary>
public static class PartesCatalog
{
    private static readonly Dictionary<string, PartesOperationDefinition> ByOperation =
        new(StringComparer.Ordinal)
        {
            ["PartesProduccion.Parametros.List"] = new(
                "PartesProduccion.Parametros.List",
                "dbo.PAQ_PartesProduccion_ParametrosList",
                Array.Empty<string>(),
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                PartesResponseShape.Parametros),
            ["PartesProduccion.InformesGestion.List"] = new(
                "PartesProduccion.InformesGestion.List",
                "dbo.PAQ_PartesProduccion_InformesGestion",
                new[]
                {
                    "fecha_desde", "fecha_hasta", "id_turno", "id_operario",
                    "id_asignacion", "id_orden_trabajo"
                },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["fecha_desde"] = "FechaDesde",
                    ["fecha_hasta"] = "FechaHasta",
                    ["id_turno"] = "IdTurno",
                    ["id_operario"] = "IdOperario",
                    ["id_asignacion"] = "IdAsignacion",
                    ["id_orden_trabajo"] = "IdOrdenTrabajo"
                },
                PartesResponseShape.InformesGestion),
            ["PartesProduccion.Dashboard.Get"] = new(
                "PartesProduccion.Dashboard.Get",
                "dbo.PAQ_PartesProduccion_DashboardGet",
                new[] { "fecha_desde", "fecha_hasta", "id_turno" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["fecha_desde"] = "FechaDesde",
                    ["fecha_hasta"] = "FechaHasta",
                    ["id_turno"] = "IdTurno"
                },
                PartesResponseShape.DashboardGet),
            ["PartesProduccion.Maquinas.List"] = new(
                "PartesProduccion.Maquinas.List",
                "dbo.PAQ_PartesProduccion_MaquinasList",
                new[] { "filter_activa", "filter_codigo", "filter_nombre", "sort", "dir" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["filter_activa"] = "FilterActiva",
                    ["filter_codigo"] = "FilterCodigo",
                    ["filter_nombre"] = "FilterNombre",
                    ["sort"] = "Sort",
                    ["dir"] = "Dir"
                },
                PartesResponseShape.MaquinaList),
            ["PartesProduccion.Maquinas.Get"] = new(
                "PartesProduccion.Maquinas.Get",
                "dbo.PAQ_PartesProduccion_MaquinasGet",
                new[] { "id_maquina" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["id_maquina"] = "IdMaquina"
                },
                PartesResponseShape.MaquinaGet),
            ["PartesProduccion.Maquinas.Create"] = new(
                "PartesProduccion.Maquinas.Create",
                "dbo.PAQ_PartesProduccion_MaquinasCreate",
                new[] { "codigo", "nombre", "activa" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["codigo"] = "Codigo",
                    ["nombre"] = "Nombre",
                    ["activa"] = "Activa"
                },
                PartesResponseShape.MaquinaCreate),
            ["PartesProduccion.Maquinas.Update"] = new(
                "PartesProduccion.Maquinas.Update",
                "dbo.PAQ_PartesProduccion_MaquinasUpdate",
                new[] { "id_maquina", "nombre", "activa" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["id_maquina"] = "IdMaquina",
                    ["nombre"] = "Nombre",
                    ["activa"] = "Activa"
                },
                PartesResponseShape.MaquinaUpdate),
            ["PartesProduccion.Maquinas.Delete"] = new(
                "PartesProduccion.Maquinas.Delete",
                "dbo.PAQ_PartesProduccion_MaquinasDelete",
                new[] { "id_maquina" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["id_maquina"] = "IdMaquina"
                },
                PartesResponseShape.MaquinaDelete),
            ["PartesProduccion.TiposTarea.List"] = new(
                "PartesProduccion.TiposTarea.List",
                "dbo.PAQ_PartesProduccion_TiposTareaList",
                new[] { "filter_activo", "filter_codigo", "filter_nombre", "sort", "dir" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["filter_activo"] = "FilterActivo",
                    ["filter_codigo"] = "FilterCodigo",
                    ["filter_nombre"] = "FilterNombre",
                    ["sort"] = "Sort",
                    ["dir"] = "Dir"
                },
                PartesResponseShape.TipoTareaList),
            ["PartesProduccion.TiposTarea.Get"] = new(
                "PartesProduccion.TiposTarea.Get",
                "dbo.PAQ_PartesProduccion_TiposTareaGet",
                new[] { "id_tipo_tarea" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["id_tipo_tarea"] = "IdTipoTarea"
                },
                PartesResponseShape.TipoTareaGet),
            ["PartesProduccion.TiposTarea.Create"] = new(
                "PartesProduccion.TiposTarea.Create",
                "dbo.PAQ_PartesProduccion_TiposTareaCreate",
                new[] { "codigo", "nombre", "activo" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["codigo"] = "Codigo",
                    ["nombre"] = "Nombre",
                    ["activo"] = "Activo"
                },
                PartesResponseShape.TipoTareaCreate),
            ["PartesProduccion.TiposTarea.Update"] = new(
                "PartesProduccion.TiposTarea.Update",
                "dbo.PAQ_PartesProduccion_TiposTareaUpdate",
                new[] { "id_tipo_tarea", "nombre", "activo" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["id_tipo_tarea"] = "IdTipoTarea",
                    ["nombre"] = "Nombre",
                    ["activo"] = "Activo"
                },
                PartesResponseShape.TipoTareaUpdate),
            ["PartesProduccion.TiposTarea.Delete"] = new(
                "PartesProduccion.TiposTarea.Delete",
                "dbo.PAQ_PartesProduccion_TiposTareaDelete",
                new[] { "id_tipo_tarea" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["id_tipo_tarea"] = "IdTipoTarea"
                },
                PartesResponseShape.TipoTareaDelete),
            ["PartesProduccion.Operaciones.List"] = new(
                "PartesProduccion.Operaciones.List",
                "dbo.PAQ_PartesProduccion_OperacionesList",
                new[] { "filter_activa", "filter_codigo", "filter_nombre", "sort", "dir" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["filter_activa"] = "FilterActiva",
                    ["filter_codigo"] = "FilterCodigo",
                    ["filter_nombre"] = "FilterNombre",
                    ["sort"] = "Sort",
                    ["dir"] = "Dir"
                },
                PartesResponseShape.OperacionList),
            ["PartesProduccion.Operaciones.Get"] = new(
                "PartesProduccion.Operaciones.Get",
                "dbo.PAQ_PartesProduccion_OperacionesGet",
                new[] { "id_operacion" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["id_operacion"] = "IdOperacion"
                },
                PartesResponseShape.OperacionGet),
            ["PartesProduccion.Operaciones.Create"] = new(
                "PartesProduccion.Operaciones.Create",
                "dbo.PAQ_PartesProduccion_OperacionesCreate",
                new[] { "codigo", "nombre", "activa" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["codigo"] = "Codigo",
                    ["nombre"] = "Nombre",
                    ["activa"] = "Activa"
                },
                PartesResponseShape.OperacionCreate),
            ["PartesProduccion.Operaciones.Update"] = new(
                "PartesProduccion.Operaciones.Update",
                "dbo.PAQ_PartesProduccion_OperacionesUpdate",
                new[] { "id_operacion", "nombre", "activa" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["id_operacion"] = "IdOperacion",
                    ["nombre"] = "Nombre",
                    ["activa"] = "Activa"
                },
                PartesResponseShape.OperacionUpdate),
            ["PartesProduccion.Operaciones.Delete"] = new(
                "PartesProduccion.Operaciones.Delete",
                "dbo.PAQ_PartesProduccion_OperacionesDelete",
                new[] { "id_operacion" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["id_operacion"] = "IdOperacion"
                },
                PartesResponseShape.OperacionDelete),
            ["PartesProduccion.Turnos.List"] = new(
                "PartesProduccion.Turnos.List",
                "dbo.PAQ_PartesProduccion_TurnosList",
                new[] { "filter_activo", "filter_codigo", "filter_nombre", "sort", "dir" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["filter_activo"] = "FilterActivo",
                    ["filter_codigo"] = "FilterCodigo",
                    ["filter_nombre"] = "FilterNombre",
                    ["sort"] = "Sort",
                    ["dir"] = "Dir"
                },
                PartesResponseShape.TurnoList),
            ["PartesProduccion.Turnos.Get"] = new(
                "PartesProduccion.Turnos.Get",
                "dbo.PAQ_PartesProduccion_TurnosGet",
                new[] { "id_turno" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["id_turno"] = "IdTurno"
                },
                PartesResponseShape.TurnoGet),
            ["PartesProduccion.Turnos.Create"] = new(
                "PartesProduccion.Turnos.Create",
                "dbo.PAQ_PartesProduccion_TurnosCreate",
                new[] { "codigo", "nombre", "hora_inicio", "hora_fin", "activo" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["codigo"] = "Codigo",
                    ["nombre"] = "Nombre",
                    ["hora_inicio"] = "HoraInicio",
                    ["hora_fin"] = "HoraFin",
                    ["activo"] = "Activo"
                },
                PartesResponseShape.TurnoCreate),
            ["PartesProduccion.Turnos.Update"] = new(
                "PartesProduccion.Turnos.Update",
                "dbo.PAQ_PartesProduccion_TurnosUpdate",
                new[] { "id_turno", "nombre", "hora_inicio", "hora_fin", "activo" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["id_turno"] = "IdTurno",
                    ["nombre"] = "Nombre",
                    ["hora_inicio"] = "HoraInicio",
                    ["hora_fin"] = "HoraFin",
                    ["activo"] = "Activo"
                },
                PartesResponseShape.TurnoUpdate),
            ["PartesProduccion.Turnos.Delete"] = new(
                "PartesProduccion.Turnos.Delete",
                "dbo.PAQ_PartesProduccion_TurnosDelete",
                new[] { "id_turno" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["id_turno"] = "IdTurno"
                },
                PartesResponseShape.TurnoDelete),
            ["PartesProduccion.ConceptosTiempo.List"] = new(
                "PartesProduccion.ConceptosTiempo.List",
                "dbo.PAQ_PartesProduccion_ConceptosTiempoList",
                new[]
                {
                    "filter_activo", "filter_es_productivo", "filter_id_tipo_tarea", "filter_habitual",
                    "filter_codigo", "filter_nombre", "sort", "dir"
                },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["filter_activo"] = "FilterActivo",
                    ["filter_es_productivo"] = "FilterEsProductivo",
                    ["filter_id_tipo_tarea"] = "FilterIdTipoTarea",
                    ["filter_habitual"] = "FilterHabitual",
                    ["filter_codigo"] = "FilterCodigo",
                    ["filter_nombre"] = "FilterNombre",
                    ["sort"] = "Sort",
                    ["dir"] = "Dir"
                },
                PartesResponseShape.ConceptoTiempoList),
            ["PartesProduccion.ConceptosTiempo.Get"] = new(
                "PartesProduccion.ConceptosTiempo.Get",
                "dbo.PAQ_PartesProduccion_ConceptosTiempoGet",
                new[] { "id_concepto_tiempo" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["id_concepto_tiempo"] = "IdConceptoTiempo"
                },
                PartesResponseShape.ConceptoTiempoGet),
            ["PartesProduccion.ConceptosTiempo.Create"] = new(
                "PartesProduccion.ConceptosTiempo.Create",
                "dbo.PAQ_PartesProduccion_ConceptosTiempoCreate",
                new[] { "codigo", "nombre", "es_productivo", "activo", "habitual", "id_tipo_tarea" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["codigo"] = "Codigo",
                    ["nombre"] = "Nombre",
                    ["es_productivo"] = "EsProductivo",
                    ["activo"] = "Activo",
                    ["habitual"] = "Habitual",
                    ["id_tipo_tarea"] = "IdTipoTarea"
                },
                PartesResponseShape.ConceptoTiempoCreate),
            ["PartesProduccion.ConceptosTiempo.Update"] = new(
                "PartesProduccion.ConceptosTiempo.Update",
                "dbo.PAQ_PartesProduccion_ConceptosTiempoUpdate",
                new[] { "id_concepto_tiempo", "nombre", "es_productivo", "activo", "habitual", "id_tipo_tarea" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["id_concepto_tiempo"] = "IdConceptoTiempo",
                    ["nombre"] = "Nombre",
                    ["es_productivo"] = "EsProductivo",
                    ["activo"] = "Activo",
                    ["habitual"] = "Habitual",
                    ["id_tipo_tarea"] = "IdTipoTarea"
                },
                PartesResponseShape.ConceptoTiempoUpdate),
            ["PartesProduccion.ConceptosTiempo.Delete"] = new(
                "PartesProduccion.ConceptosTiempo.Delete",
                "dbo.PAQ_PartesProduccion_ConceptosTiempoDelete",
                new[] { "id_concepto_tiempo" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["id_concepto_tiempo"] = "IdConceptoTiempo"
                },
                PartesResponseShape.ConceptoTiempoDelete),
            ["PartesProduccion.OrdenesTrabajo.List"] = new(
                "PartesProduccion.OrdenesTrabajo.List",
                "dbo.PAQ_PartesProduccion_OrdenesTrabajoList",
                new[]
                {
                    "agrupado", "page", "page_size", "sort", "dir",
                    "filter_estado", "filter_codigo", "filter_descripcion",
                    "filter_fecha_desde", "filter_fecha_hasta"
                },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["agrupado"] = "Agrupado",
                    ["page"] = "Page",
                    ["page_size"] = "PageSize",
                    ["sort"] = "Sort",
                    ["dir"] = "Dir",
                    ["filter_estado"] = "FilterEstado",
                    ["filter_codigo"] = "FilterCodigo",
                    ["filter_descripcion"] = "FilterDescripcion",
                    ["filter_fecha_desde"] = "FilterFechaDesde",
                    ["filter_fecha_hasta"] = "FilterFechaHasta"
                },
                PartesResponseShape.OrdenTrabajoList),
            ["PartesProduccion.OrdenesTrabajo.Get"] = new(
                "PartesProduccion.OrdenesTrabajo.Get",
                "dbo.PAQ_PartesProduccion_OrdenesTrabajoGet",
                new[] { "id_orden_trabajo", "codigo_ot" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["id_orden_trabajo"] = "IdOrdenTrabajo",
                    ["codigo_ot"] = "CodigoOt"
                },
                PartesResponseShape.OrdenTrabajoGet),
            ["PartesProduccion.OrdenesTrabajo.Create"] = new(
                "PartesProduccion.OrdenesTrabajo.Create",
                "(orchestrated)",
                new[]
                {
                    "codigo", "descripcion", "id_articulo", "cantidad_a_producir",
                    "modo_individual", "id_operacion", "operaciones_incluidas_json",
                    "fecha_inicio_plan", "fecha_fin_plan", "observaciones",
                    "tipo_ref_externa", "id_ref_externa", "usuario_id"
                },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                PartesResponseShape.OrdenTrabajoCreate),
            ["PartesProduccion.OrdenesTrabajo.Update"] = new(
                "PartesProduccion.OrdenesTrabajo.Update",
                "(orchestrated)",
                new[]
                {
                    "id_orden_trabajo", "codigo_ot",
                    "descripcion", "id_articulo", "cantidad_a_producir",
                    "modo_individual", "id_operacion", "operaciones_incluidas_json",
                    "fecha_inicio_plan", "fecha_fin_plan", "observaciones",
                    "estado", "usuario_id"
                },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                PartesResponseShape.OrdenTrabajoUpdate),
            ["PartesProduccion.OrdenesTrabajo.Delete"] = new(
                "PartesProduccion.OrdenesTrabajo.Delete",
                "(orchestrated)",
                new[] { "id_orden_trabajo", "usuario_codigo" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                PartesResponseShape.OrdenTrabajoDelete),
            ["PartesProduccion.OrdenesTrabajo.PatchEstado"] = new(
                "PartesProduccion.OrdenesTrabajo.PatchEstado",
                "(orchestrated)",
                new[] { "id_orden_trabajo", "estado", "usuario_id" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                PartesResponseShape.OrdenTrabajoPatchEstado),
            ["PartesProduccion.OrdenesTrabajo.CambioMasivoEstado"] = new(
                "PartesProduccion.OrdenesTrabajo.CambioMasivoEstado",
                "(orchestrated)",
                new[] { "ids_json", "operacion", "usuario_id" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                PartesResponseShape.OrdenTrabajoCambioMasivoEstado),
            ["PartesProduccion.Asignaciones.List"] = new(
                "PartesProduccion.Asignaciones.List",
                "dbo.PAQ_PartesProduccion_AsignacionesList",
                new[]
                {
                    "page", "page_size", "sort", "dir",
                    "filter_fecha_desde", "filter_fecha_hasta",
                    "filter_id_turno", "filter_id_tipo_tarea", "filter_estado"
                },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["page"] = "Page",
                    ["page_size"] = "PageSize",
                    ["sort"] = "Sort",
                    ["dir"] = "Dir",
                    ["filter_fecha_desde"] = "FilterFechaDesde",
                    ["filter_fecha_hasta"] = "FilterFechaHasta",
                    ["filter_id_turno"] = "FilterIdTurno",
                    ["filter_id_tipo_tarea"] = "FilterIdTipoTarea",
                    ["filter_estado"] = "FilterEstado"
                },
                PartesResponseShape.AsignacionList),
            ["PartesProduccion.Asignaciones.Get"] = new(
                "PartesProduccion.Asignaciones.Get",
                "dbo.PAQ_PartesProduccion_AsignacionesGet",
                new[] { "id_asignacion" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["id_asignacion"] = "IdAsignacion"
                },
                PartesResponseShape.AsignacionGet),
            ["PartesProduccion.Asignaciones.Create"] = new(
                "PartesProduccion.Asignaciones.Create",
                "(orchestrated)",
                new[]
                {
                    "fecha_asignacion", "id_turno", "id_tipo_tarea",
                    "observaciones", "usuario_id"
                },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                PartesResponseShape.AsignacionCreate),
            ["PartesProduccion.Asignaciones.Update"] = new(
                "PartesProduccion.Asignaciones.Update",
                "(orchestrated)",
                new[]
                {
                    "id_asignacion", "fecha_asignacion", "id_turno", "id_tipo_tarea",
                    "observaciones", "usuario_id"
                },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                PartesResponseShape.AsignacionUpdate),
            ["PartesProduccion.Asignaciones.Publicar"] = new(
                "PartesProduccion.Asignaciones.Publicar",
                "(orchestrated)",
                new[] { "id_asignacion" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                PartesResponseShape.AsignacionPublicar),
            ["PartesProduccion.Asignaciones.Cerrar"] = new(
                "PartesProduccion.Asignaciones.Cerrar",
                "(orchestrated)",
                new[] { "id_asignacion", "usuario_id" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                PartesResponseShape.AsignacionCerrar),
            ["PartesProduccion.Asignaciones.Cancelar"] = new(
                "PartesProduccion.Asignaciones.Cancelar",
                "(orchestrated)",
                new[] { "id_asignacion", "usuario_id" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                PartesResponseShape.AsignacionCancelar),
            ["PartesProduccion.Asignaciones.Items.List"] = new(
                "PartesProduccion.Asignaciones.Items.List",
                "dbo.PAQ_PartesProduccion_AsignacionesItemsList",
                new[] { "id_asignacion" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["id_asignacion"] = "IdAsignacion"
                },
                PartesResponseShape.AsignacionItemList),
            ["PartesProduccion.Asignaciones.Items.Create"] = new(
                "PartesProduccion.Asignaciones.Items.Create",
                "(orchestrated)",
                new[]
                {
                    "id_asignacion", "id_orden_trabajo", "id_articulo", "id_operacion", "id_tipo_tarea",
                    "id_maquina", "unidades_hora_std", "unidades_plan", "minutos_plan",
                    "hora_inicio_plan", "hora_fin_plan", "notas_plan", "prioridad",
                    "nro_orden_operacion", "usuario_id"
                },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                PartesResponseShape.AsignacionItemCreate),
            ["PartesProduccion.Asignaciones.Items.Update"] = new(
                "PartesProduccion.Asignaciones.Items.Update",
                "(orchestrated)",
                new[]
                {
                    "id_asignacion", "id_item", "id_orden_trabajo", "id_articulo", "id_operacion", "id_tipo_tarea",
                    "id_maquina", "unidades_hora_std", "unidades_plan", "minutos_plan",
                    "hora_inicio_plan", "hora_fin_plan", "notas_plan", "prioridad",
                    "nro_orden_operacion", "usuario_id"
                },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                PartesResponseShape.AsignacionItemUpdate),
            ["PartesProduccion.Asignaciones.Items.Delete"] = new(
                "PartesProduccion.Asignaciones.Items.Delete",
                "(orchestrated)",
                new[] { "id_asignacion", "id_item" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                PartesResponseShape.AsignacionItemDelete),
            ["PartesProduccion.Asignaciones.OperariosPlan.List"] = new(
                "PartesProduccion.Asignaciones.OperariosPlan.List",
                "(orchestrated)",
                new[] { "id_asignacion" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                PartesResponseShape.AsignacionOperariosPlanList),
            ["PartesProduccion.Asignaciones.Items.Operarios.List"] = new(
                "PartesProduccion.Asignaciones.Items.Operarios.List",
                "dbo.PAQ_PartesProduccion_AsignacionesItemsOperariosList",
                new[] { "id_asignacion", "id_item" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["id_asignacion"] = "IdAsignacion",
                    ["id_item"] = "IdItem"
                },
                PartesResponseShape.AsignacionItemOperariosList),
            ["PartesProduccion.Asignaciones.Items.Operarios.Create"] = new(
                "PartesProduccion.Asignaciones.Items.Operarios.Create",
                "(orchestrated)",
                new[] { "id_asignacion", "id_item", "id_operario", "rol_plan", "usuario_id" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                PartesResponseShape.AsignacionItemOperariosCreate),
            ["PartesProduccion.Asignaciones.Items.Operarios.Update"] = new(
                "PartesProduccion.Asignaciones.Items.Operarios.Update",
                "(orchestrated)",
                new[] { "id_asignacion", "id_item", "id_operarios", "usuario_id" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                PartesResponseShape.AsignacionItemOperariosUpdate),
            ["PartesProduccion.Asignaciones.Items.Operarios.Delete"] = new(
                "PartesProduccion.Asignaciones.Items.Operarios.Delete",
                "(orchestrated)",
                new[] { "id_asignacion", "id_item", "id_operario" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                PartesResponseShape.AsignacionItemOperariosDelete),
            ["PartesProduccion.PartesOperario.Get"] = new(
                "PartesProduccion.PartesOperario.Get",
                "dbo.PAQ_PartesProduccion_PartesOperarioGet",
                new[] { "id_parte_operario", "usuario_id" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["id_parte_operario"] = "IdParteOperario",
                    ["usuario_id"] = "UsuarioId"
                },
                PartesResponseShape.ParteOperarioGet),
            ["PartesProduccion.PartesOperario.Create"] = new(
                "PartesProduccion.PartesOperario.Create",
                "(orchestrated)",
                new[] { "fecha_parte", "id_turno", "usuario_id" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                PartesResponseShape.ParteOperarioCreate),
            ["PartesProduccion.PartesOperario.Update"] = new(
                "PartesProduccion.PartesOperario.Update",
                "(orchestrated)",
                new[] { "id_parte_operario", "observaciones", "usuario_id" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                PartesResponseShape.ParteOperarioUpdate),
            ["PartesProduccion.PartesOperario.Enviar"] = new(
                "PartesProduccion.PartesOperario.Enviar",
                "(orchestrated)",
                new[] { "id_parte_operario", "usuario_id" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                PartesResponseShape.ParteOperarioEnviar),
            ["PartesProduccion.PartesOperario.Aprobar"] = new(
                "PartesProduccion.PartesOperario.Aprobar",
                "(orchestrated)",
                new[] { "id_parte_operario", "usuario_id" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                PartesResponseShape.ParteOperarioAprobar),
            ["PartesProduccion.PartesOperario.Devolver"] = new(
                "PartesProduccion.PartesOperario.Devolver",
                "(orchestrated)",
                new[] { "id_parte_operario", "usuario_id" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                PartesResponseShape.ParteOperarioDevolver),
            ["PartesProduccion.PartesOperario.Cerrar"] = new(
                "PartesProduccion.PartesOperario.Cerrar",
                "(orchestrated)",
                new[] { "id_parte_operario", "usuario_id" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                PartesResponseShape.ParteOperarioCerrar),
            ["PartesProduccion.PartesOperario.ParteContexto"] = new(
                "PartesProduccion.PartesOperario.ParteContexto",
                "dbo.PAQ_PartesProduccion_PartesOperarioParteContexto",
                new[] { "fecha_parte", "id_turno", "usuario_id" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["fecha_parte"] = "FechaParte",
                    ["id_turno"] = "IdTurno",
                    ["usuario_id"] = "UsuarioId"
                },
                PartesResponseShape.ParteOperarioParteContexto),
            ["PartesProduccion.PartesOperario.MisPartes.List"] = new(
                "PartesProduccion.PartesOperario.MisPartes.List",
                "dbo.PAQ_PartesProduccion_PartesOperarioMisPartesList",
                new[]
                {
                    "usuario_id",
                    "filter_fecha_desde", "filter_fecha_hasta", "filter_id_turno", "filter_estado",
                    "sort", "dir", "page", "page_size",
                    "planificado_page", "planificado_page_size"
                },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["usuario_id"] = "UsuarioId",
                    ["filter_fecha_desde"] = "FilterFechaDesde",
                    ["filter_fecha_hasta"] = "FilterFechaHasta",
                    ["filter_id_turno"] = "FilterIdTurno",
                    ["filter_estado"] = "FilterEstado",
                    ["sort"] = "Sort",
                    ["dir"] = "Dir",
                    ["page"] = "Page",
                    ["page_size"] = "PageSize",
                    ["planificado_page"] = "PlanificadoPage",
                    ["planificado_page_size"] = "PlanificadoPageSize"
                },
                PartesResponseShape.ParteOperarioMisPartesList),
            ["PartesProduccion.PartesOperario.PartesRevisar.List"] = new(
                "PartesProduccion.PartesOperario.PartesRevisar.List",
                "dbo.PAQ_PartesProduccion_PartesOperarioPartesRevisarList",
                new[]
                {
                    "filter_fecha_desde", "filter_fecha_hasta", "filter_id_turno",
                    "filter_id_operario", "filter_estado",
                    "sort", "dir", "page", "page_size"
                },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["filter_fecha_desde"] = "FilterFechaDesde",
                    ["filter_fecha_hasta"] = "FilterFechaHasta",
                    ["filter_id_turno"] = "FilterIdTurno",
                    ["filter_id_operario"] = "FilterIdOperario",
                    ["filter_estado"] = "FilterEstado",
                    ["sort"] = "Sort",
                    ["dir"] = "Dir",
                    ["page"] = "Page",
                    ["page_size"] = "PageSize"
                },
                PartesResponseShape.ParteOperarioPartesRevisarList),
            ["PartesProduccion.PartesOperario.PartesRevisar.Get"] = new(
                "PartesProduccion.PartesOperario.PartesRevisar.Get",
                "dbo.PAQ_PartesProduccion_PartesOperarioPartesRevisarGet",
                new[] { "id_parte_operario" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["id_parte_operario"] = "IdParteOperario"
                },
                PartesResponseShape.ParteOperarioPartesRevisarGet),
            ["PartesProduccion.PartesOperario.Entradas.List"] = new(
                "PartesProduccion.PartesOperario.Entradas.List",
                "dbo.PAQ_PartesProduccion_PartesOperarioEntradasList",
                new[] { "id_parte_operario", "usuario_id" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["id_parte_operario"] = "IdParteOperario",
                    ["usuario_id"] = "UsuarioId"
                },
                PartesResponseShape.ParteOperarioEntradasList),
            ["PartesProduccion.PartesOperario.Entradas.Create"] = new(
                "PartesProduccion.PartesOperario.Entradas.Create",
                "(orchestrated)",
                new[]
                {
                    "id_parte_operario", "usuario_id",
                    "id_concepto_tiempo", "minutos", "fecha_hora_desde", "fecha_hora_hasta",
                    "unidades_hechas", "unidades_merma", "unidades_retrabajo", "notas",
                    "id_asignacion_item", "id_orden_trabajo", "nro_orden_operacion", "id_maquina",
                    "id_concepto_tiempo_no_productivo", "minutos_no_productivos"
                },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                PartesResponseShape.ParteOperarioEntradasCreate),
            ["PartesProduccion.PartesOperario.Entradas.Update"] = new(
                "PartesProduccion.PartesOperario.Entradas.Update",
                "(orchestrated)",
                new[]
                {
                    "id_parte_operario", "id_parte_entrada", "usuario_id",
                    "id_concepto_tiempo", "minutos", "fecha_hora_desde", "fecha_hora_hasta",
                    "unidades_hechas", "unidades_merma", "unidades_retrabajo", "notas", "id_maquina"
                },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                PartesResponseShape.ParteOperarioEntradasUpdate),
            ["PartesProduccion.PartesOperario.Entradas.Delete"] = new(
                "PartesProduccion.PartesOperario.Entradas.Delete",
                "(orchestrated)",
                new[] { "id_parte_operario", "id_parte_entrada", "usuario_id" },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                PartesResponseShape.ParteOperarioEntradasDelete),
            ["PartesProduccion.PartesOperario.Entradas.Reclasificar"] = new(
                "PartesProduccion.PartesOperario.Entradas.Reclasificar",
                "(orchestrated)",
                new[]
                {
                    "id_parte_operario", "id_parte_entrada", "usuario_id", "notas_revision",
                    "id_asignacion_item", "id_orden_trabajo", "id_maquina", "id_operacion", "id_concepto_tiempo"
                },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                PartesResponseShape.ParteOperarioEntradasReclasificar),
        };

    public static IReadOnlyCollection<string> Operations => ByOperation.Keys;

    public static bool TryGet(string operation, out PartesOperationDefinition definition) =>
        ByOperation.TryGetValue(operation, out definition!);
}
