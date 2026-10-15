using System.Text.Json;
using PaqAgent.Informes;
using PaqAgent.Options;
using PaqContracts;

namespace PaqAgent.Partes;

public sealed class PartesOutcome
{
    public string Status { get; init; } = JobStatuses.Failed;
    public object? Data { get; init; }
    public string? ErrorCode { get; init; }
    public string? ErrorMessage { get; init; }
}

public sealed class PartesGatewayRunner
{
    private readonly IInformesSpExecutor companySpExecutor;

    public PartesGatewayRunner(IInformesSpExecutor companySpExecutor)
    {
        this.companySpExecutor = companySpExecutor;
    }

    public async Task<PartesOutcome> RunAsync(
        PartesOperationDefinition definition,
        AgentOptions agentOptions,
        IReadOnlyDictionary<string, object?> parameters,
        int timeoutSeconds,
        CancellationToken cancellationToken)
    {
        var databaseOverride = ExtractString(parameters, "_database");
        if (string.IsNullOrWhiteSpace(databaseOverride))
        {
            return Fail("INVALID_PARAMETERS", "El parametro _database es obligatorio para ops PartesProduccion.");
        }

        if (!agentOptions.HasSqlConfig)
        {
            return new PartesOutcome
            {
                Status = JobStatuses.Degraded,
                ErrorCode = "SQL_NOT_CONFIGURED",
                ErrorMessage = "sql.server/database no configurados en appsettings.local.json"
            };
        }

        try
        {
            var connectionString = SqlConnectionStringFactory.Build(
                agentOptions.Sql,
                connectTimeoutSeconds: 15,
                databaseOverride: databaseOverride);

            var spParams = MapSpParameters(definition, parameters);

            if (definition.Shape == PartesResponseShape.MaquinaGet
                || definition.Shape == PartesResponseShape.MaquinaDelete)
            {
                if (!TryGetPositiveInt(spParams, "IdMaquina", out var idMaquina))
                {
                    return Fail("INVALID_PARAMETERS", "El parametro id_maquina es obligatorio y debe ser > 0.");
                }

                spParams["IdMaquina"] = idMaquina;
            }

            if (definition.Shape == PartesResponseShape.OperacionGet
                || definition.Shape == PartesResponseShape.OperacionDelete)
            {
                if (!TryGetPositiveInt(spParams, "IdOperacion", out var idOperacion))
                {
                    return Fail("INVALID_PARAMETERS", "El parametro id_operacion es obligatorio y debe ser > 0.");
                }

                spParams["IdOperacion"] = idOperacion;
            }

            if (definition.Shape == PartesResponseShape.TurnoGet
                || definition.Shape == PartesResponseShape.TurnoDelete)
            {
                if (!TryGetPositiveInt(spParams, "IdTurno", out var idTurno))
                {
                    return Fail("INVALID_PARAMETERS", "El parametro id_turno es obligatorio y debe ser > 0.");
                }

                spParams["IdTurno"] = idTurno;
            }

            if (definition.Shape == PartesResponseShape.ConceptoTiempoGet
                || definition.Shape == PartesResponseShape.ConceptoTiempoDelete)
            {
                if (!TryGetPositiveInt(spParams, "IdConceptoTiempo", out var idConceptoTiempo))
                {
                    return Fail("INVALID_PARAMETERS", "El parametro id_concepto_tiempo es obligatorio y debe ser > 0.");
                }

                spParams["IdConceptoTiempo"] = idConceptoTiempo;
            }

            var ordenTrabajoGetById = false;
            if (definition.Shape == PartesResponseShape.OrdenTrabajoGet)
            {
                if (!TryNormalizeOrdenTrabajoGetParams(spParams, out ordenTrabajoGetById, out var otGetError))
                {
                    return Fail("INVALID_PARAMETERS", otGetError);
                }
            }

            if (definition.Shape == PartesResponseShape.TipoTareaGet
                || definition.Shape == PartesResponseShape.TipoTareaDelete)
            {
                if (!TryGetPositiveInt(spParams, "IdTipoTarea", out var idTipoTarea))
                {
                    return Fail("INVALID_PARAMETERS", "El parametro id_tipo_tarea es obligatorio y debe ser > 0.");
                }

                spParams["IdTipoTarea"] = idTipoTarea;
            }

            if (definition.Shape == PartesResponseShape.AsignacionItemList)
            {
                if (!TryGetPositiveInt(spParams, "IdAsignacion", out var idAsignacion))
                {
                    return Fail("INVALID_PARAMETERS", "El parametro id_asignacion es obligatorio y debe ser > 0.");
                }

                spParams["IdAsignacion"] = idAsignacion;
            }

            if (definition.Shape == PartesResponseShape.AsignacionItemOperariosList)
            {
                if (!TryGetPositiveInt(spParams, "IdAsignacion", out var idAsignacion)
                    || !TryGetPositiveInt(spParams, "IdItem", out var idItem))
                {
                    return Fail("INVALID_PARAMETERS", "Los parametros id_asignacion e id_item son obligatorios y deben ser > 0.");
                }

                spParams["IdAsignacion"] = idAsignacion;
                spParams["IdItem"] = idItem;
            }

            if (definition.Shape == PartesResponseShape.ParteOperarioGet)
            {
                if (!TryGetPositiveInt(spParams, "IdParteOperario", out var idParteOperario)
                    || !TryGetPositiveInt(spParams, "UsuarioId", out var usuarioId))
                {
                    return Fail(
                        "INVALID_PARAMETERS",
                        "Los parametros id_parte_operario y usuario_id son obligatorios y deben ser > 0.");
                }

                spParams["IdParteOperario"] = idParteOperario;
                spParams["UsuarioId"] = usuarioId;
            }

            if (definition.Shape == PartesResponseShape.ParteOperarioParteContexto)
            {
                if (!TryGetPositiveInt(spParams, "UsuarioId", out var usuarioIdContexto))
                {
                    return Fail("INVALID_PARAMETERS", "El parametro usuario_id es obligatorio y debe ser > 0.");
                }

                spParams["UsuarioId"] = usuarioIdContexto;
                if (!TryParseRequiredDate(spParams, "FechaParte", "fecha_parte", out var fechaError))
                {
                    return Fail(fechaError.StartsWith("El parametro fecha_parte es obligatorio", StringComparison.Ordinal)
                        ? "INVALID_PARAMETERS"
                        : "VALIDATION", fechaError);
                }
            }

            if (definition.Shape == PartesResponseShape.ParteOperarioMisPartesList)
            {
                if (!TryGetPositiveInt(spParams, "UsuarioId", out var usuarioIdMisPartes))
                {
                    return Fail("INVALID_PARAMETERS", "El parametro usuario_id es obligatorio y debe ser > 0.");
                }

                spParams["UsuarioId"] = usuarioIdMisPartes;
                if (!TryValidateAsignacionListDates(spParams, out var misPartesDateError))
                {
                    return Fail("VALIDATION", misPartesDateError);
                }
            }

            if (definition.Shape == PartesResponseShape.ParteOperarioPartesRevisarList)
            {
                if (!TryValidateAsignacionListDates(spParams, out var revisarDateError))
                {
                    return Fail("VALIDATION", revisarDateError);
                }
            }

            if (definition.Shape == PartesResponseShape.ParteOperarioPartesRevisarGet)
            {
                if (!TryGetPositiveInt(spParams, "IdParteOperario", out var idParteRevisar))
                {
                    return Fail(
                        "INVALID_PARAMETERS",
                        "El parametro id_parte_operario es obligatorio y debe ser > 0.");
                }

                spParams["IdParteOperario"] = idParteRevisar;
            }

            if (definition.Shape == PartesResponseShape.ParteOperarioEntradasList)
            {
                if (!TryGetPositiveInt(spParams, "IdParteOperario", out var idParteEntradas)
                    || !TryGetPositiveInt(spParams, "UsuarioId", out var usuarioIdEntradas))
                {
                    return Fail(
                        "INVALID_PARAMETERS",
                        "Los parametros id_parte_operario y usuario_id son obligatorios y deben ser > 0.");
                }

                spParams["IdParteOperario"] = idParteEntradas;
                spParams["UsuarioId"] = usuarioIdEntradas;
            }

            if (definition.Shape == PartesResponseShape.MaquinaCreate
                || definition.Shape == PartesResponseShape.OperacionCreate)
            {
                if (!TryNormalizeMaquinaCreateParams(spParams, out var errorMessage))
                {
                    return Fail("INVALID_PARAMETERS", errorMessage);
                }
            }

            if (definition.Shape == PartesResponseShape.MaquinaUpdate)
            {
                if (!TryNormalizeMaquinaUpdateParams(spParams, out var errorMessage))
                {
                    return Fail("INVALID_PARAMETERS", errorMessage);
                }
            }

            if (definition.Shape == PartesResponseShape.OperacionUpdate)
            {
                if (!TryNormalizeOperacionUpdateParams(spParams, out var errorMessage))
                {
                    return Fail("INVALID_PARAMETERS", errorMessage);
                }
            }

            if (definition.Shape == PartesResponseShape.TipoTareaCreate)
            {
                if (!TryNormalizeTipoTareaCreateParams(spParams, out var errorMessage))
                {
                    return Fail("INVALID_PARAMETERS", errorMessage);
                }
            }

            if (definition.Shape == PartesResponseShape.TipoTareaUpdate)
            {
                if (!TryNormalizeTipoTareaUpdateParams(spParams, out var errorMessage))
                {
                    return Fail("INVALID_PARAMETERS", errorMessage);
                }
            }

            if (definition.Shape == PartesResponseShape.TurnoCreate)
            {
                if (!TryNormalizeTurnoCreateParams(spParams, out var errorCode, out var errorMessage))
                {
                    return Fail(errorCode, errorMessage);
                }
            }

            if (definition.Shape == PartesResponseShape.TurnoUpdate)
            {
                if (!TryNormalizeTurnoUpdateParams(spParams, parameters, out var errorCode, out var errorMessage))
                {
                    return Fail(errorCode, errorMessage);
                }
            }

            if (definition.Shape == PartesResponseShape.ConceptoTiempoCreate)
            {
                if (!TryNormalizeConceptoTiempoCreateParams(spParams, out var errorCode, out var errorMessage))
                {
                    return Fail(errorCode, errorMessage);
                }
            }

            if (definition.Shape == PartesResponseShape.ConceptoTiempoUpdate)
            {
                if (!TryNormalizeConceptoTiempoUpdateParams(spParams, parameters, out var errorCode, out var errorMessage))
                {
                    return Fail(errorCode, errorMessage);
                }
            }

            if (definition.Shape == PartesResponseShape.AsignacionList)
            {
                if (!TryValidateAsignacionListDates(spParams, out var dateError))
                {
                    return Fail("VALIDATION", dateError);
                }
            }

            var resultSets = await companySpExecutor
                .ExecuteAsync(
                    connectionString,
                    definition.StoredProcedure,
                    spParams,
                    timeoutSeconds,
                    cancellationToken)
                .ConfigureAwait(false);

            return definition.Shape switch
            {
                PartesResponseShape.Parametros => Ok(BuildParametros(resultSets)),
                PartesResponseShape.InformesGestion => Ok(BuildInformesGestion(resultSets)),
                PartesResponseShape.DashboardGet => Ok(BuildDashboardGet(resultSets)),
                PartesResponseShape.MaquinaList => Ok(BuildMaquinaList(resultSets)),
                PartesResponseShape.MaquinaGet => BuildMaquinaGet(resultSets),
                PartesResponseShape.MaquinaCreate => BuildMaquinaMutation(resultSets, "crear"),
                PartesResponseShape.MaquinaUpdate => BuildMaquinaMutation(resultSets, "actualizar"),
                PartesResponseShape.MaquinaDelete => BuildMaquinaDelete(resultSets),
                PartesResponseShape.TipoTareaList => Ok(BuildTipoTareaList(resultSets)),
                PartesResponseShape.TipoTareaGet => BuildTipoTareaGet(resultSets),
                PartesResponseShape.TipoTareaCreate => BuildTipoTareaMutation(resultSets, "crear"),
                PartesResponseShape.TipoTareaUpdate => BuildTipoTareaMutation(resultSets, "actualizar"),
                PartesResponseShape.TipoTareaDelete => BuildTipoTareaDelete(resultSets),
                PartesResponseShape.OperacionList => Ok(BuildOperacionList(resultSets)),
                PartesResponseShape.OperacionGet => BuildOperacionGet(resultSets),
                PartesResponseShape.OperacionCreate => BuildOperacionMutation(resultSets, "crear"),
                PartesResponseShape.OperacionUpdate => BuildOperacionMutation(resultSets, "actualizar"),
                PartesResponseShape.OperacionDelete => BuildOperacionDelete(resultSets),
                PartesResponseShape.TurnoList => Ok(BuildTurnoList(resultSets)),
                PartesResponseShape.TurnoGet => BuildTurnoGet(resultSets),
                PartesResponseShape.TurnoCreate => BuildTurnoMutation(resultSets, "crear"),
                PartesResponseShape.TurnoUpdate => BuildTurnoMutation(resultSets, "actualizar"),
                PartesResponseShape.TurnoDelete => BuildTurnoDelete(resultSets),
                PartesResponseShape.ConceptoTiempoList => Ok(BuildConceptoTiempoList(resultSets)),
                PartesResponseShape.ConceptoTiempoGet => BuildConceptoTiempoGet(resultSets),
                PartesResponseShape.ConceptoTiempoCreate => BuildConceptoTiempoMutation(resultSets, "crear"),
                PartesResponseShape.ConceptoTiempoUpdate => BuildConceptoTiempoMutation(resultSets, "actualizar"),
                PartesResponseShape.ConceptoTiempoDelete => BuildConceptoTiempoDelete(resultSets),
                PartesResponseShape.OrdenTrabajoList => Ok(BuildOrdenTrabajoList(resultSets)),
                PartesResponseShape.OrdenTrabajoGet => BuildOrdenTrabajoGet(resultSets, ordenTrabajoGetById),
                PartesResponseShape.AsignacionList => Ok(BuildAsignacionList(resultSets)),
                PartesResponseShape.AsignacionGet => BuildAsignacionGet(resultSets),
                PartesResponseShape.AsignacionItemList => BuildAsignacionItemList(resultSets),
                PartesResponseShape.AsignacionItemOperariosList => BuildAsignacionItemOperariosList(resultSets),
                PartesResponseShape.ParteOperarioGet => BuildParteOperarioGet(resultSets),
                PartesResponseShape.ParteOperarioParteContexto => BuildParteOperarioParteContexto(resultSets),
                PartesResponseShape.ParteOperarioMisPartesList => Ok(BuildParteOperarioMisPartesList(resultSets)),
                PartesResponseShape.ParteOperarioPartesRevisarList => Ok(BuildParteOperarioPartesRevisarList(resultSets)),
                PartesResponseShape.ParteOperarioPartesRevisarGet => BuildParteOperarioPartesRevisarGet(resultSets),
                PartesResponseShape.ParteOperarioEntradasList => BuildParteOperarioEntradasList(resultSets),
                _ => Fail("INTERNAL_ERROR", $"Shape Partes no soportado: {definition.Shape}")
            };
        }
        catch (Exception ex)
        {
            return Fail("SQL_ERROR", ex.GetType().Name + ": " + ex.Message);
        }
    }

    private static object BuildParametros(IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets)
    {
        var filas = resultSets.ElementAtOrDefault(1) ?? Array.Empty<Dictionary<string, object?>>();
        return new Dictionary<string, object?>
        {
            ["parametros"] = filas.Select(MapParametro).ToList()
        };
    }

    private static object BuildDashboardGet(IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets)
    {
        var kpisRow = resultSets.ElementAtOrDefault(0)?.FirstOrDefault();
        var kpis = kpisRow is null
            ? new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            : Keyed(kpisRow);
        var operarios = resultSets.ElementAtOrDefault(1) ?? Array.Empty<Dictionary<string, object?>>();
        var maquinas = resultSets.ElementAtOrDefault(2) ?? Array.Empty<Dictionary<string, object?>>();

        return new Dictionary<string, object?>
        {
            ["partes_abiertos"] = GetInt(kpis, "partes_abiertos") ?? 0,
            ["partes_enviados"] = GetInt(kpis, "partes_enviados") ?? 0,
            ["partes_aprobados"] = GetInt(kpis, "partes_aprobados") ?? 0,
            ["partes_cerrados"] = GetInt(kpis, "partes_cerrados") ?? 0,
            ["horas_productivas"] = (double)(GetDecimal(kpis, "horas_productivas") ?? 0m),
            ["horas_improductivas"] = (double)(GetDecimal(kpis, "horas_improductivas") ?? 0m),
            ["top_operarios"] = operarios.Select(row =>
            {
                var keyed = Keyed(row);
                return new Dictionary<string, object?>
                {
                    ["id_operario"] = GetInt(keyed, "id_operario") ?? 0,
                    ["nombre"] = GetString(keyed, "nombre") ?? string.Empty,
                    ["horas_productivas"] = (double)(GetDecimal(keyed, "horas_productivas") ?? 0m)
                };
            }).ToList(),
            ["top_maquinas"] = maquinas.Select(row =>
            {
                var keyed = Keyed(row);
                return new Dictionary<string, object?>
                {
                    ["id_maquina"] = GetInt(keyed, "id_maquina") ?? 0,
                    ["codigo"] = GetString(keyed, "codigo") ?? string.Empty,
                    ["horas_productivas"] = (double)(GetDecimal(keyed, "horas_productivas") ?? 0m)
                };
            }).ToList()
        };
    }

    private static object BuildInformesGestion(IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets)
    {
        var filas = resultSets.ElementAtOrDefault(1) ?? Array.Empty<Dictionary<string, object?>>();
        var resumenRow = resultSets.ElementAtOrDefault(2)?.FirstOrDefault();
        return new Dictionary<string, object?>
        {
            ["items"] = filas.Select(MapItem).ToList(),
            ["resumen"] = MapResumen(resumenRow)
        };
    }

    private static object BuildListEnvelope(IReadOnlyList<Dictionary<string, object?>> items)
    {
        var count = items.Count;
        return new Dictionary<string, object?>
        {
            ["items"] = items,
            ["page"] = 1,
            ["page_size"] = count,
            ["total"] = count,
            ["total_pages"] = 1
        };
    }

    private static object BuildMaquinaList(IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets)
    {
        var filas = resultSets.ElementAtOrDefault(0) ?? Array.Empty<Dictionary<string, object?>>();
        var items = filas.Select(row => MapMaquinaPayload(Keyed(row))).ToList();
        return BuildListEnvelope(items);
    }

    private static object BuildTipoTareaList(IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets)
    {
        var filas = resultSets.ElementAtOrDefault(0) ?? Array.Empty<Dictionary<string, object?>>();
        var items = filas.Select(row => MapTipoTareaPayload(Keyed(row))).ToList();
        return BuildListEnvelope(items);
    }

    private static object BuildOperacionList(IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets)
    {
        var filas = resultSets.ElementAtOrDefault(0) ?? Array.Empty<Dictionary<string, object?>>();
        var items = filas.Select(row => MapMaquinaPayload(Keyed(row))).ToList();
        return BuildListEnvelope(items);
    }

    private static object BuildTurnoList(IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets)
    {
        var filas = resultSets.ElementAtOrDefault(0) ?? Array.Empty<Dictionary<string, object?>>();
        var items = filas.Select(row => MapTurnoPayload(Keyed(row))).ToList();
        return BuildListEnvelope(items);
    }

    private static object BuildConceptoTiempoList(IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets)
    {
        var filas = resultSets.ElementAtOrDefault(0) ?? Array.Empty<Dictionary<string, object?>>();
        var items = filas.Select(row => MapConceptoTiempoListItem(Keyed(row))).ToList();
        return BuildListEnvelope(items);
    }

    private static object BuildOrdenTrabajoList(IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets)
    {
        var metaRow = resultSets.ElementAtOrDefault(0)?.FirstOrDefault();
        var meta = metaRow is null ? new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase) : Keyed(metaRow);
        var filas = resultSets.ElementAtOrDefault(1) ?? Array.Empty<Dictionary<string, object?>>();
        var agrupado = GetBool(meta, "agrupado") ?? true;
        var items = filas.Select(row => MapOrdenTrabajoListItem(Keyed(row), agrupado)).ToList();

        return new Dictionary<string, object?>
        {
            ["items"] = items,
            ["page"] = GetInt(meta, "page") ?? 1,
            ["page_size"] = GetInt(meta, "page_size") ?? 50,
            ["total"] = GetInt(meta, "total") ?? items.Count,
            ["total_pages"] = GetInt(meta, "total_pages") ?? 0
        };
    }

    private static object BuildAsignacionList(IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets)
    {
        var metaRow = resultSets.ElementAtOrDefault(0)?.FirstOrDefault();
        var meta = metaRow is null ? new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase) : Keyed(metaRow);
        var filas = resultSets.ElementAtOrDefault(1) ?? Array.Empty<Dictionary<string, object?>>();
        var items = filas.Select(row => MapAsignacionListItem(Keyed(row))).ToList();

        return new Dictionary<string, object?>
        {
            ["items"] = items,
            ["page"] = GetInt(meta, "page") ?? 1,
            ["page_size"] = GetInt(meta, "page_size") ?? 50,
            ["total"] = GetInt(meta, "total") ?? items.Count,
            ["total_pages"] = GetInt(meta, "total_pages") ?? 0
        };
    }

    private static Dictionary<string, object?> MapAsignacionListItem(Dictionary<string, object?> keyed)
    {
        var estado = GetInt(keyed, "estado") ?? 0;
        var estadoLabel = GetString(keyed, "estado_label")
            ?? estado switch
            {
                0 => "Borrador",
                1 => "Publicada",
                2 => "Cerrada",
                3 => "Anulada",
                _ => "Desconocido"
            };

        return new Dictionary<string, object?>
        {
            ["id"] = GetInt(keyed, "id") ?? 0,
            ["fecha_asignacion"] = FormatDateOnly(keyed.TryGetValue("fecha_asignacion", out var fecha) ? fecha : null),
            ["id_turno"] = GetInt(keyed, "id_turno"),
            ["turno_codigo"] = GetString(keyed, "turno_codigo"),
            ["id_tipo_tarea"] = GetInt(keyed, "id_tipo_tarea"),
            ["tipo_tarea_codigo"] = GetString(keyed, "tipo_tarea_codigo"),
            ["tipo_tarea_nombre"] = GetString(keyed, "tipo_tarea_nombre"),
            ["supervisor_id"] = GetInt(keyed, "supervisor_id"),
            ["supervisor_codigo"] = GetString(keyed, "supervisor_codigo"),
            ["supervisor_nombre"] = GetString(keyed, "supervisor_nombre"),
            ["supervisor_label"] = GetString(keyed, "supervisor_label"),
            ["estado"] = estado,
            ["estado_label"] = estadoLabel,
            ["fecha_publicacion"] = FormatDateTime(keyed.TryGetValue("fecha_publicacion", out var fp) ? fp : null),
            ["fecha_cierre"] = FormatDateTime(keyed.TryGetValue("fecha_cierre", out var fc) ? fc : null)
        };
    }

    private static PartesOutcome BuildAsignacionGet(IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets)
    {
        var row = resultSets.ElementAtOrDefault(0)?.FirstOrDefault();
        if (row is null)
        {
            return Fail("NOT_FOUND", "Asignación no encontrada");
        }

        var keyed = Keyed(row);
        var payload = MapAsignacionListItem(keyed);
        payload["observaciones"] = GetString(keyed, "observaciones");
        return Ok(payload);
    }

    private static PartesOutcome BuildAsignacionItemList(IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets)
    {
        var metaRow = resultSets.ElementAtOrDefault(0)?.FirstOrDefault();
        if (metaRow is not null)
        {
            var meta = Keyed(metaRow);
            if ((GetInt(meta, "not_found") ?? 0) == 1)
            {
                return Fail("NOT_FOUND", "Asignación no encontrada");
            }
        }

        var filas = resultSets.ElementAtOrDefault(1) ?? Array.Empty<Dictionary<string, object?>>();
        // Compat: si el SP no devolvió meta RS0 (tabla ausente), items vienen en RS0
        if (resultSets.Count == 1 && metaRow is not null && !metaRow.ContainsKey("not_found"))
        {
            filas = resultSets[0];
        }

        var items = filas.Select(row => MapAsignacionItem(Keyed(row))).ToList();
        return Ok(new Dictionary<string, object?> { ["items"] = items });
    }

    private static PartesOutcome BuildAsignacionItemOperariosList(
        IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets)
    {
        var metaRow = resultSets.ElementAtOrDefault(0)?.FirstOrDefault();
        if (metaRow is not null)
        {
            var meta = Keyed(metaRow);
            if ((GetInt(meta, "not_found") ?? 0) == 1)
            {
                return Fail("NOT_FOUND", "Tarea no encontrada");
            }
        }

        var filas = resultSets.ElementAtOrDefault(1) ?? Array.Empty<Dictionary<string, object?>>();
        if (resultSets.Count == 1 && metaRow is not null && !metaRow.ContainsKey("not_found"))
        {
            filas = resultSets[0];
        }

        var items = filas.Select(row =>
        {
            var keyed = Keyed(row);
            return new Dictionary<string, object?>
            {
                ["id"] = GetInt(keyed, "id") ?? 0,
                ["id_operario"] = GetInt(keyed, "id_operario"),
                ["nro_legajo"] = GetString(keyed, "nro_legajo"),
                ["nombre_completo"] = GetString(keyed, "nombre_completo"),
                ["rol_plan"] = GetString(keyed, "rol_plan")
            };
        }).ToList();

        return Ok(new Dictionary<string, object?> { ["items"] = items });
    }

    private static PartesOutcome BuildParteOperarioGet(
        IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets)
    {
        var metaRow = resultSets.ElementAtOrDefault(0)?.FirstOrDefault();
        if (metaRow is not null)
        {
            var meta = Keyed(metaRow);
            if ((GetInt(meta, "no_legajo") ?? 0) == 1)
            {
                return Fail("NO_LEGAJO", "Usuario sin legajo");
            }

            if ((GetInt(meta, "not_found") ?? 0) == 1)
            {
                return Fail("NOT_FOUND", "Parte no encontrado");
            }
        }

        var row = resultSets.ElementAtOrDefault(1)?.FirstOrDefault();
        if (row is null)
        {
            return Fail("NOT_FOUND", "Parte no encontrado");
        }

        var keyed = Keyed(row);
        return Ok(new Dictionary<string, object?>
        {
            ["id"] = GetInt(keyed, "id") ?? 0,
            ["fecha_parte"] = FormatDateOnly(keyed.TryGetValue("fecha_parte", out var fechaParte) ? fechaParte : null),
            ["id_turno"] = GetInt(keyed, "id_turno"),
            ["turno_nombre"] = GetString(keyed, "turno_nombre"),
            ["estado"] = GetInt(keyed, "estado") ?? 0,
            ["observaciones"] = GetString(keyed, "observaciones")
        });
    }

    private static PartesOutcome BuildParteOperarioParteContexto(
        IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets)
    {
        var row = resultSets.ElementAtOrDefault(0)?.FirstOrDefault();
        int? id = null;
        if (row is not null)
        {
            var keyed = Keyed(row);
            id = GetInt(keyed, "id_parte_operario") ?? GetInt(keyed, "id");
        }

        return Ok(new Dictionary<string, object?>
        {
            ["id_parte_operario"] = id is > 0 ? id : null
        });
    }

    private static object BuildParteOperarioMisPartesList(
        IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets)
    {
        var metaRow = resultSets.ElementAtOrDefault(0)?.FirstOrDefault();
        var meta = metaRow is null
            ? new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            : Keyed(metaRow);
        var cabeceras = resultSets.ElementAtOrDefault(1) ?? Array.Empty<Dictionary<string, object?>>();
        var planificado = resultSets.ElementAtOrDefault(2) ?? Array.Empty<Dictionary<string, object?>>();

        return new Dictionary<string, object?>
        {
            ["items"] = cabeceras.Select(row => MapMisPartesCabecera(Keyed(row))).ToList(),
            ["planificado_items"] = planificado.Select(row => MapMisPartesPlanificadoItem(Keyed(row))).ToList(),
            ["planificado_page"] = GetInt(meta, "planificado_page") ?? 1,
            ["planificado_page_size"] = GetInt(meta, "planificado_page_size") ?? 0,
            ["planificado_total"] = GetInt(meta, "planificado_total") ?? planificado.Count,
            ["planificado_total_pages"] = GetInt(meta, "planificado_total_pages") ?? 0,
            ["verificacion_operario"] = MapVerificacionOperario(meta),
            ["page"] = GetInt(meta, "page") ?? 1,
            ["page_size"] = GetInt(meta, "page_size") ?? 0,
            ["total"] = GetInt(meta, "total") ?? cabeceras.Count,
            ["total_pages"] = GetInt(meta, "total_pages") ?? 0
        };
    }

    private static Dictionary<string, object?> MapVerificacionOperario(Dictionary<string, object?> meta) =>
        new()
        {
            ["autenticado"] = GetBool(meta, "autenticado") ?? false,
            ["id_usuario"] = GetInt(meta, "id_usuario"),
            ["usuario_codigo"] = GetString(meta, "usuario_codigo"),
            ["usuario_nombre"] = GetString(meta, "usuario_nombre"),
            ["tabla_legajos_existe"] = GetBool(meta, "tabla_legajos_existe") ?? false,
            ["tiene_legajo_vinculado"] = GetBool(meta, "tiene_legajo_vinculado") ?? false,
            ["id_legajo_pk_usado_como_id_operario"] = GetInt(meta, "id_legajo_pk_usado_como_id_operario"),
            ["legajo_nro"] = GetString(meta, "legajo_nro"),
            ["legajo_apellido"] = GetString(meta, "legajo_apellido"),
            ["legajo_nombre"] = GetString(meta, "legajo_nombre"),
            ["tabla_partes_operario_existe"] = GetBool(meta, "tabla_partes_operario_existe") ?? false,
            ["tablas_planificado_existen"] = GetBool(meta, "tablas_planificado_existen") ?? false,
            ["filtros_solicitados"] = new Dictionary<string, object?>
            {
                ["fecha_desde"] = FormatDateOnly(meta.TryGetValue("filter_fecha_desde", out var fd) ? fd : null),
                ["fecha_hasta"] = FormatDateOnly(meta.TryGetValue("filter_fecha_hasta", out var fh) ? fh : null),
                ["id_turno"] = GetInt(meta, "filter_id_turno"),
                ["estado_parte"] = GetInt(meta, "filter_estado")
            }
        };

    private static Dictionary<string, object?> MapMisPartesCabecera(Dictionary<string, object?> keyed) =>
        new()
        {
            ["id"] = GetInt(keyed, "id") ?? 0,
            ["fecha_parte"] = FormatDateOnly(keyed.TryGetValue("fecha_parte", out var fechaParte) ? fechaParte : null),
            ["id_turno"] = GetInt(keyed, "id_turno"),
            ["turno_codigo"] = GetString(keyed, "turno_codigo"),
            ["turno_nombre"] = GetString(keyed, "turno_nombre"),
            ["estado"] = GetInt(keyed, "estado") ?? 0,
            ["fecha_apertura"] = FormatDateTime(keyed.TryGetValue("fecha_apertura", out var fa) ? fa : null),
            ["fecha_cierre"] = FormatDateTime(keyed.TryGetValue("fecha_cierre", out var fc) ? fc : null),
            ["observaciones"] = GetString(keyed, "observaciones")
        };

    private static Dictionary<string, object?> MapMisPartesPlanificadoItem(Dictionary<string, object?> keyed)
    {
        var esGenerada = GetBool(keyed, "es_generada_usuario") ?? false;
        var idAsignacionItem = GetInt(keyed, "id_asignacion_item");
        var idParteEntrada = GetInt(keyed, "id_parte_entrada");
        var rowKey = GetString(keyed, "row_key");
        if (string.IsNullOrWhiteSpace(rowKey))
        {
            if (esGenerada && idParteEntrada is > 0)
            {
                rowKey = "libre-" + idParteEntrada.Value;
            }
            else if (idAsignacionItem is > 0)
            {
                rowKey = "plan-" + idAsignacionItem.Value;
            }
        }

        return new Dictionary<string, object?>
        {
            ["id_asignacion_item"] = idAsignacionItem,
            ["id_asignacion"] = GetInt(keyed, "id_asignacion"),
            ["id_parte_operario"] = GetInt(keyed, "id_parte_operario"),
            ["id_parte_entrada"] = idParteEntrada,
            ["id_orden_trabajo"] = GetInt(keyed, "id_orden_trabajo"),
            ["id_operacion"] = GetInt(keyed, "id_operacion"),
            ["id_tipo_tarea"] = GetInt(keyed, "id_tipo_tarea"),
            ["id_maquina"] = GetInt(keyed, "id_maquina"),
            ["fecha_asignacion"] = FormatDateOnly(keyed.TryGetValue("fecha_asignacion", out var fechaAsig) ? fechaAsig : null),
            ["id_turno"] = GetInt(keyed, "id_turno"),
            ["turno_codigo"] = GetString(keyed, "turno_codigo"),
            ["turno_nombre"] = GetString(keyed, "turno_nombre"),
            ["codigo_ot"] = GetString(keyed, "codigo_ot"),
            ["nro_orden"] = GetInt(keyed, "nro_orden"),
            ["operacion_etiqueta"] = GetString(keyed, "operacion_etiqueta"),
            ["tipo_tarea_etiqueta"] = GetString(keyed, "tipo_tarea_etiqueta"),
            ["supervisor_id"] = GetInt(keyed, "supervisor_id"),
            ["supervisor_codigo"] = GetString(keyed, "supervisor_codigo"),
            ["supervisor_nombre"] = GetString(keyed, "supervisor_nombre"),
            ["supervisor_label"] = GetString(keyed, "supervisor_label"),
            ["maquina_etiqueta"] = GetString(keyed, "maquina_etiqueta"),
            ["minutos_plan"] = GetInt(keyed, "minutos_plan"),
            ["unidades_plan"] = GetDecimal(keyed, "unidades_plan") is { } up ? (double)up : null,
            ["unidades_hora_std"] = GetDecimal(keyed, "unidades_hora_std") is { } uh ? (double)uh : null,
            ["notas_plan"] = GetString(keyed, "notas_plan"),
            ["registrado_en_parte"] = GetBool(keyed, "registrado_en_parte") ?? false,
            ["es_generada_usuario"] = esGenerada,
            ["row_key"] = rowKey
        };
    }

    private static object BuildParteOperarioPartesRevisarList(
        IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets)
    {
        var metaRow = resultSets.ElementAtOrDefault(0)?.FirstOrDefault();
        var meta = metaRow is null
            ? new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            : Keyed(metaRow);
        var filas = resultSets.ElementAtOrDefault(1) ?? Array.Empty<Dictionary<string, object?>>();
        var items = filas.Select(row => MapPartesRevisarItem(Keyed(row))).ToList();

        return new Dictionary<string, object?>
        {
            ["items"] = items,
            ["page"] = GetInt(meta, "page") ?? 1,
            ["page_size"] = GetInt(meta, "page_size") ?? 50,
            ["total"] = GetInt(meta, "total") ?? items.Count,
            ["total_pages"] = GetInt(meta, "total_pages") ?? 0
        };
    }

    private static Dictionary<string, object?> MapPartesRevisarItem(Dictionary<string, object?> keyed)
    {
        var idOperario = GetInt(keyed, "id_operario");
        var operarioNombre = GetString(keyed, "operario_nombre");
        if (string.IsNullOrWhiteSpace(operarioNombre))
        {
            operarioNombre = "Op." + (idOperario ?? 0);
        }

        return new Dictionary<string, object?>
        {
            ["id"] = GetInt(keyed, "id") ?? 0,
            ["id_operario"] = idOperario,
            ["operario_nombre"] = operarioNombre,
            ["fecha_parte"] = FormatDateOnly(keyed.TryGetValue("fecha_parte", out var fechaParte) ? fechaParte : null),
            ["id_turno"] = GetInt(keyed, "id_turno"),
            ["turno_codigo"] = GetString(keyed, "turno_codigo"),
            ["turno_nombre"] = GetString(keyed, "turno_nombre"),
            ["estado"] = GetInt(keyed, "estado") ?? 0,
            ["fecha_envio"] = FormatDateTime(keyed.TryGetValue("fecha_envio", out var fechaEnvio) ? fechaEnvio : null),
            ["cantidad_entradas"] = GetInt(keyed, "cantidad_entradas") ?? 0
        };
    }

    private static PartesOutcome BuildParteOperarioPartesRevisarGet(
        IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets)
    {
        var headerRow = resultSets.ElementAtOrDefault(0)?.FirstOrDefault();
        if (headerRow is null)
        {
            return Fail("NOT_FOUND", "Parte no encontrado");
        }

        var header = Keyed(headerRow);
        var id = GetInt(header, "id") ?? 0;
        if (id <= 0)
        {
            return Fail("NOT_FOUND", "Parte no encontrado");
        }

        var idOperario = GetInt(header, "id_operario");
        var operarioNombre = GetString(header, "operario_nombre");
        if (string.IsNullOrWhiteSpace(operarioNombre))
        {
            operarioNombre = "Op." + (idOperario ?? 0);
        }

        var entradasRows = resultSets.ElementAtOrDefault(1) ?? Array.Empty<Dictionary<string, object?>>();
        var entradas = entradasRows.Select(row => MapPartesRevisarEntrada(Keyed(row))).ToList();

        return Ok(new Dictionary<string, object?>
        {
            ["id"] = id,
            ["id_operario"] = idOperario,
            ["operario_nombre"] = operarioNombre,
            ["fecha_parte"] = FormatDateOnly(header.TryGetValue("fecha_parte", out var fechaParte) ? fechaParte : null),
            ["id_turno"] = GetInt(header, "id_turno"),
            ["turno_nombre"] = GetString(header, "turno_nombre"),
            ["estado"] = GetInt(header, "estado") ?? 0,
            ["observaciones"] = GetString(header, "observaciones"),
            ["entradas"] = entradas
        });
    }

    private static Dictionary<string, object?> MapPartesRevisarEntrada(Dictionary<string, object?> keyed) =>
        new()
        {
            ["id"] = GetInt(keyed, "id") ?? 0,
            ["id_concepto_tiempo"] = GetInt(keyed, "id_concepto_tiempo"),
            ["concepto_nombre"] = GetString(keyed, "concepto_nombre"),
            ["minutos"] = GetInt(keyed, "minutos"),
            ["unidades_hechas"] = GetDecimal(keyed, "unidades_hechas") is { } uh ? (double)uh : null,
            ["notas"] = GetString(keyed, "notas"),
            ["notas_revision"] = GetString(keyed, "notas_revision")
        };

    private static PartesOutcome BuildParteOperarioEntradasList(
        IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets)
    {
        var metaRow = resultSets.ElementAtOrDefault(0)?.FirstOrDefault();
        if (metaRow is not null)
        {
            var meta = Keyed(metaRow);
            if ((GetInt(meta, "no_legajo") ?? 0) == 1)
            {
                return Fail("NO_LEGAJO", "Usuario sin legajo");
            }

            if ((GetInt(meta, "not_found") ?? 0) == 1)
            {
                return Fail("NOT_FOUND", PartesEntradasRunnerHelpers.NotFoundEditableMessage);
            }
        }

        var filas = resultSets.ElementAtOrDefault(1) ?? Array.Empty<Dictionary<string, object?>>();
        var items = filas.Select(row => MapParteOperarioEntradaItem(Keyed(row))).ToList();
        return Ok(new Dictionary<string, object?> { ["items"] = items });
    }

    private static Dictionary<string, object?> MapParteOperarioEntradaItem(Dictionary<string, object?> keyed)
    {
        var row = new Dictionary<string, object?>
        {
            ["id"] = GetInt(keyed, "id") ?? 0,
            ["id_asignacion_item"] = GetInt(keyed, "id_asignacion_item"),
            ["id_orden_trabajo"] = GetInt(keyed, "id_orden_trabajo"),
            ["codigo_ot"] = GetString(keyed, "codigo_ot"),
            ["origen_carga"] = GetString(keyed, "origen_carga"),
            ["id_maquina"] = GetInt(keyed, "id_maquina"),
            ["maquina_etiqueta"] = GetString(keyed, "maquina_etiqueta"),
            ["id_concepto_tiempo"] = GetInt(keyed, "id_concepto_tiempo"),
            ["concepto_nombre"] = GetString(keyed, "concepto_nombre"),
            ["es_productivo"] = GetBool(keyed, "es_productivo"),
            ["minutos"] = GetInt(keyed, "minutos"),
            ["fecha_hora_desde"] = FormatDateTime(keyed.TryGetValue("fecha_hora_desde", out var desde) ? desde : null),
            ["fecha_hora_hasta"] = FormatDateTime(keyed.TryGetValue("fecha_hora_hasta", out var hasta) ? hasta : null),
            ["unidades_hechas"] = GetDecimal(keyed, "unidades_hechas") is { } uh ? (double)uh : null,
            ["unidades_merma"] = GetDecimal(keyed, "unidades_merma") is { } um ? (double)um : null,
            ["unidades_retrabajo"] = GetDecimal(keyed, "unidades_retrabajo") is { } ur ? (double)ur : null,
            ["notas"] = GetString(keyed, "notas")
        };

        if (keyed.ContainsKey("nro_orden_operacion"))
        {
            row["nro_orden_operacion"] = GetInt(keyed, "nro_orden_operacion");
        }

        return row;
    }


    private static Dictionary<string, object?> MapAsignacionItem(Dictionary<string, object?> keyed) =>
        new()
        {
            ["id"] = GetInt(keyed, "id") ?? 0,
            ["id_asignacion"] = GetInt(keyed, "id_asignacion"),
            ["id_orden_trabajo"] = GetInt(keyed, "id_orden_trabajo"),
            ["orden_trabajo_codigo"] = GetString(keyed, "orden_trabajo_codigo"),
            ["id_articulo"] = GetInt(keyed, "id_articulo"),
            ["articulo_codigo"] = GetString(keyed, "articulo_codigo"),
            ["id_operacion"] = GetInt(keyed, "id_operacion"),
            ["operacion_codigo"] = GetString(keyed, "operacion_codigo"),
            ["operacion_nombre"] = GetString(keyed, "operacion_nombre"),
            ["id_tipo_tarea"] = GetInt(keyed, "id_tipo_tarea"),
            ["tipo_tarea_codigo"] = GetString(keyed, "tipo_tarea_codigo"),
            ["tipo_tarea_nombre"] = GetString(keyed, "tipo_tarea_nombre"),
            ["id_maquina"] = GetInt(keyed, "id_maquina"),
            ["maquina_codigo"] = GetString(keyed, "maquina_codigo"),
            ["maquina_nombre"] = GetString(keyed, "maquina_nombre"),
            ["unidades_hora_std"] = GetDecimal(keyed, "unidades_hora_std") is { } uh ? (double)uh : null,
            ["unidades_plan"] = GetDecimal(keyed, "unidades_plan") is { } up ? (double)up : null,
            ["minutos_plan"] = GetInt(keyed, "minutos_plan"),
            ["notas_plan"] = GetString(keyed, "notas_plan"),
            ["prioridad"] = GetInt(keyed, "prioridad"),
            ["hora_inicio_plan"] = GetString(keyed, "hora_inicio_plan"),
            ["hora_fin_plan"] = GetString(keyed, "hora_fin_plan"),
            ["nro_orden_operacion"] = GetInt(keyed, "nro_orden_operacion")
        };

    private static bool TryValidateAsignacionListDates(
        IReadOnlyDictionary<string, object?> spParams,
        out string errorMessage)
    {
        errorMessage = string.Empty;
        if (!TryParseOptionalDate(spParams, "FilterFechaDesde", out var desde)
            || !TryParseOptionalDate(spParams, "FilterFechaHasta", out var hasta))
        {
            errorMessage = "filter_fecha_desde / filter_fecha_hasta deben ser fechas válidas.";
            return false;
        }

        if (desde is not null && hasta is not null && desde > hasta)
        {
            errorMessage = "La fecha desde no debe ser posterior a la fecha hasta.";
            return false;
        }

        return true;
    }

    private static bool TryParseRequiredDate(
        Dictionary<string, object?> spParams,
        string spKey,
        string jobParamName,
        out string errorMessage)
    {
        errorMessage = string.Empty;
        if (!TryParseOptionalDate(spParams, spKey, out var value) || value is null)
        {
            errorMessage = spParams.TryGetValue(spKey, out var raw) && raw is not null && raw is not DBNull
                ? $"El parametro {jobParamName} debe ser fecha AAAA-MM-DD."
                : $"El parametro {jobParamName} es obligatorio.";
            return false;
        }

        spParams[spKey] = value.Value;
        return true;
    }

    private static bool TryParseOptionalDate(
        IReadOnlyDictionary<string, object?> parameters,
        string key,
        out DateOnly? value)
    {
        value = null;
        if (!parameters.TryGetValue(key, out var raw) || raw is null || raw is DBNull)
        {
            return true;
        }

        if (raw is DateOnly d)
        {
            value = d;
            return true;
        }

        if (raw is DateTime dt)
        {
            value = DateOnly.FromDateTime(dt);
            return true;
        }

        var text = raw.ToString();
        if (string.IsNullOrWhiteSpace(text))
        {
            return true;
        }

        if (DateOnly.TryParse(text, out var parsedOnly))
        {
            value = parsedOnly;
            return true;
        }

        if (DateTime.TryParse(text, out var parsedDt))
        {
            value = DateOnly.FromDateTime(parsedDt);
            return true;
        }

        return false;
    }

    private static Dictionary<string, object?> MapOrdenTrabajoListItem(Dictionary<string, object?> keyed, bool agrupado)
    {
        var estado = GetInt(keyed, "estado") ?? 0;
        var estadoLabel = GetString(keyed, "estado_label")
            ?? estado switch
            {
                0 => "Borrador",
                1 => "Abierta",
                2 => "Cerrada",
                _ => "Desconocido"
            };

        var item = new Dictionary<string, object?>
        {
            ["id"] = GetInt(keyed, "id") ?? 0,
            ["codigo"] = GetString(keyed, "codigo"),
            ["tipo_ref_externa"] = GetString(keyed, "tipo_ref_externa"),
            ["id_ref_externa"] = keyed.TryGetValue("id_ref_externa", out var idRef) && idRef is not null && idRef is not DBNull
                ? idRef
                : null,
            ["descripcion"] = GetString(keyed, "descripcion"),
            ["id_articulo"] = GetInt(keyed, "id_articulo"),
            ["articulo_codigo"] = GetString(keyed, "articulo_codigo"),
            ["articulo_label"] = GetString(keyed, "articulo_label"),
            ["cantidad_a_producir"] = GetInt(keyed, "cantidad_a_producir") ?? 0,
            ["fecha_inicio_plan"] = FormatDateOnly(keyed.TryGetValue("fecha_inicio_plan", out var fi) ? fi : null),
            ["fecha_fin_plan"] = FormatDateOnly(keyed.TryGetValue("fecha_fin_plan", out var ff) ? ff : null),
            ["estado"] = estado,
            ["estado_label"] = estadoLabel,
            ["observaciones"] = GetString(keyed, "observaciones"),
            ["fecha_alta"] = FormatDateTime(keyed.TryGetValue("fecha_alta", out var fa) ? fa : null)
        };

        if (agrupado)
        {
            item["id_orden_trabajo_representativo"] = GetInt(keyed, "id_orden_trabajo_representativo")
                ?? GetInt(keyed, "id")
                ?? 0;
        }
        else
        {
            item["id_operacion"] = GetInt(keyed, "id_operacion");
            item["operacion_codigo"] = GetString(keyed, "operacion_codigo");
            item["operacion_nombre"] = GetString(keyed, "operacion_nombre");
            item["operacion_label"] = GetString(keyed, "operacion_label");
            if (keyed.ContainsKey("nro_orden") && GetInt(keyed, "nro_orden") is int nroOrden)
            {
                item["nro_orden"] = nroOrden;
            }
        }

        return item;
    }

    private static PartesOutcome BuildOrdenTrabajoGet(
        IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets,
        bool byId)
    {
        var filas = resultSets.ElementAtOrDefault(0) ?? Array.Empty<Dictionary<string, object?>>();
        if (filas.Count == 0)
        {
            return Fail("NOT_FOUND", "Orden de trabajo no encontrada.");
        }

        if (byId)
        {
            var keyed = Keyed(filas[0]);
            return Ok(MapOrdenTrabajoListItem(keyed, agrupado: false));
        }

        var items = filas.Select(row => MapOrdenTrabajoListItem(Keyed(row), agrupado: false)).ToList();
        var rep = items[0];
        var estadoRep = items.Max(item => GetInt(item, "estado") ?? 0);
        var estadoLabel = estadoRep switch
        {
            0 => "Borrador",
            1 => "Abierta",
            2 => "Cerrada",
            _ => "Desconocido"
        };

        var operacionesIncluidas = items.Select(item => new Dictionary<string, object?>
        {
            ["id_operacion"] = GetInt(item, "id_operacion") ?? 0,
            ["nro_orden"] = GetInt(item, "nro_orden") ?? 0,
            ["operacion_label"] = GetString(item, "operacion_label"),
            ["id_orden_trabajo"] = GetInt(item, "id") ?? 0
        }).ToList();

        return Ok(new Dictionary<string, object?>
        {
            ["codigo"] = GetString(rep, "codigo"),
            ["descripcion"] = GetString(rep, "descripcion"),
            ["id_articulo"] = GetInt(rep, "id_articulo"),
            ["articulo_label"] = GetString(rep, "articulo_label"),
            ["cantidad_a_producir"] = GetInt(rep, "cantidad_a_producir"),
            ["fecha_inicio_plan"] = GetString(rep, "fecha_inicio_plan"),
            ["fecha_fin_plan"] = GetString(rep, "fecha_fin_plan"),
            ["observaciones"] = GetString(rep, "observaciones"),
            ["estado"] = estadoRep,
            ["estado_label"] = estadoLabel,
            ["modo_individual"] = items.Count == 1,
            ["operaciones_incluidas"] = operacionesIncluidas,
            ["id_orden_trabajo_representativo"] = GetInt(rep, "id") ?? 0
        });
    }

    private static string? FormatDateOnly(object? value)
    {
        if (value is null || value is DBNull)
        {
            return null;
        }

        if (value is DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd");
        }

        if (value is DateOnly d)
        {
            return d.ToString("yyyy-MM-dd");
        }

        var text = value.ToString();
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        if (DateTime.TryParse(text, out var parsed))
        {
            return parsed.ToString("yyyy-MM-dd");
        }

        return text.Length >= 10 ? text[..10] : text;
    }

    private static string? FormatDateTime(object? value)
    {
        if (value is null || value is DBNull)
        {
            return null;
        }

        if (value is DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        var text = value.ToString();
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        if (DateTime.TryParse(text, out var parsed))
        {
            return parsed.ToString("yyyy-MM-dd HH:mm:ss");
        }

        return text;
    }

    private static Dictionary<string, object?> MapConceptoTiempoListItem(Dictionary<string, object?> keyed)
    {
        var payload = new Dictionary<string, object?>
        {
            ["id"] = GetInt(keyed, "id") ?? 0,
            ["codigo"] = GetString(keyed, "codigo") ?? string.Empty,
            ["nombre"] = GetString(keyed, "nombre") ?? string.Empty,
            ["es_productivo"] = GetBool(keyed, "es_productivo") ?? false,
            ["activo"] = GetBool(keyed, "activo") ?? false
        };

        var hasTipoTareaFields = keyed.ContainsKey("habitual")
            || keyed.ContainsKey("id_tipo_tarea")
            || keyed.ContainsKey("tipo_tarea_codigo");

        if (!hasTipoTareaFields)
        {
            return payload;
        }

        payload["habitual"] = GetBool(keyed, "habitual") ?? false;
        payload["id_tipo_tarea"] = GetInt(keyed, "id_tipo_tarea");
        payload["tipo_tarea_codigo"] = GetString(keyed, "tipo_tarea_codigo");
        payload["tipo_tarea_nombre"] = GetString(keyed, "tipo_tarea_nombre");
        payload["tipo_tarea_label"] = GetString(keyed, "tipo_tarea_label");
        return payload;
    }

    private static PartesOutcome BuildMaquinaGet(IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets)
    {
        var row = resultSets.ElementAtOrDefault(0)?.FirstOrDefault();
        if (row is null)
        {
            return Fail("NOT_FOUND", "Maquina no encontrada.");
        }

        var keyed = Keyed(row);
        return Ok(MapMaquinaPayload(keyed));
    }

    private static PartesOutcome BuildMaquinaMutation(
        IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets,
        string accion)
    {
        var row = resultSets.ElementAtOrDefault(0)?.FirstOrDefault();
        if (row is null)
        {
            return Fail("SQL_ERROR", $"Respuesta vacia del SP Maquinas ({accion}).");
        }

        var keyed = Keyed(row);
        var resultCode = GetString(keyed, "resultCode") ?? string.Empty;
        if (!string.Equals(resultCode, "OK", StringComparison.OrdinalIgnoreCase))
        {
            return resultCode switch
            {
                "NOT_FOUND" => Fail("NOT_FOUND", "Maquina no encontrada."),
                "DUPLICATE_CODE" => Fail("DUPLICATE_CODE", "El codigo de maquina ya existe."),
                "INVALID_PARAMETERS" => Fail("INVALID_PARAMETERS", $"Parametros invalidos para {accion} maquina."),
                "REFERENCED" => Fail("REFERENCED", "No se pudo eliminar la maquina por referencias relacionadas."),
                "tablaNoExiste" => Fail("SQL_ERROR", "Tabla PQ_PRD_MAQUINAS no disponible."),
                _ => Fail(string.IsNullOrWhiteSpace(resultCode) ? "SQL_ERROR" : resultCode,
                    $"Error al {accion} maquina ({resultCode}).")
            };
        }

        return Ok(MapMaquinaPayload(keyed));
    }

    private static PartesOutcome BuildMaquinaDelete(IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets)
    {
        var row = resultSets.ElementAtOrDefault(0)?.FirstOrDefault();
        if (row is null)
        {
            return Fail("SQL_ERROR", "Respuesta vacia del SP MaquinasDelete.");
        }

        var keyed = Keyed(row);
        var resultCode = GetString(keyed, "resultCode") ?? string.Empty;
        if (!string.Equals(resultCode, "OK", StringComparison.OrdinalIgnoreCase))
        {
            return resultCode switch
            {
                "NOT_FOUND" => Fail("NOT_FOUND", "Maquina no encontrada."),
                "INVALID_PARAMETERS" => Fail("INVALID_PARAMETERS", "El parametro id_maquina es obligatorio y debe ser > 0."),
                "REFERENCED" => Fail("REFERENCED", "No se pudo eliminar la maquina por referencias relacionadas."),
                "tablaNoExiste" => Fail("SQL_ERROR", "Tabla PQ_PRD_MAQUINAS no disponible."),
                _ => Fail(string.IsNullOrWhiteSpace(resultCode) ? "SQL_ERROR" : resultCode,
                    $"Error al eliminar maquina ({resultCode}).")
            };
        }

        return Ok(new Dictionary<string, object?>
        {
            ["id"] = GetInt(keyed, "id") ?? 0,
            ["eliminado"] = GetBool(keyed, "eliminado") ?? true
        });
    }

    private static Dictionary<string, object?> MapMaquinaPayload(Dictionary<string, object?> keyed) =>
        new()
        {
            ["id"] = GetInt(keyed, "id") ?? 0,
            ["codigo"] = GetString(keyed, "codigo") ?? string.Empty,
            ["nombre"] = GetString(keyed, "nombre") ?? string.Empty,
            ["activa"] = GetBool(keyed, "activa") ?? false
        };

    private static PartesOutcome BuildOperacionGet(IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets)
    {
        var row = resultSets.ElementAtOrDefault(0)?.FirstOrDefault();
        if (row is null)
        {
            return Fail("NOT_FOUND", "Operacion no encontrada.");
        }

        var keyed = Keyed(row);
        return Ok(MapMaquinaPayload(keyed));
    }

    private static PartesOutcome BuildOperacionMutation(
        IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets,
        string accion)
    {
        var row = resultSets.ElementAtOrDefault(0)?.FirstOrDefault();
        if (row is null)
        {
            return Fail("SQL_ERROR", $"Respuesta vacia del SP Operaciones ({accion}).");
        }

        var keyed = Keyed(row);
        var resultCode = GetString(keyed, "resultCode") ?? string.Empty;
        if (!string.Equals(resultCode, "OK", StringComparison.OrdinalIgnoreCase))
        {
            return resultCode switch
            {
                "NOT_FOUND" => Fail("NOT_FOUND", "Operacion no encontrada."),
                "DUPLICATE_CODE" => Fail("DUPLICATE_CODE", "El codigo de operacion ya existe."),
                "INVALID_PARAMETERS" => Fail("INVALID_PARAMETERS", $"Parametros invalidos para {accion} operacion."),
                "REFERENCED" => Fail("REFERENCED", "No se pudo eliminar la operacion por referencias relacionadas."),
                "tablaNoExiste" => Fail("SQL_ERROR", "Tabla PQ_PRD_OPERACIONES no disponible."),
                _ => Fail(string.IsNullOrWhiteSpace(resultCode) ? "SQL_ERROR" : resultCode,
                    $"Error al {accion} operacion ({resultCode}).")
            };
        }

        return Ok(MapMaquinaPayload(keyed));
    }

    private static PartesOutcome BuildOperacionDelete(IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets)
    {
        var row = resultSets.ElementAtOrDefault(0)?.FirstOrDefault();
        if (row is null)
        {
            return Fail("SQL_ERROR", "Respuesta vacia del SP OperacionesDelete.");
        }

        var keyed = Keyed(row);
        var resultCode = GetString(keyed, "resultCode") ?? string.Empty;
        if (!string.Equals(resultCode, "OK", StringComparison.OrdinalIgnoreCase))
        {
            return resultCode switch
            {
                "NOT_FOUND" => Fail("NOT_FOUND", "Operacion no encontrada."),
                "INVALID_PARAMETERS" => Fail("INVALID_PARAMETERS", "El parametro id_operacion es obligatorio y debe ser > 0."),
                "REFERENCED" => Fail("REFERENCED", "No se pudo eliminar la operacion por referencias relacionadas."),
                "tablaNoExiste" => Fail("SQL_ERROR", "Tabla PQ_PRD_OPERACIONES no disponible."),
                _ => Fail(string.IsNullOrWhiteSpace(resultCode) ? "SQL_ERROR" : resultCode,
                    $"Error al eliminar operacion ({resultCode}).")
            };
        }

        return Ok(new Dictionary<string, object?>
        {
            ["id"] = GetInt(keyed, "id") ?? 0,
            ["eliminado"] = GetBool(keyed, "eliminado") ?? true
        });
    }

    private static PartesOutcome BuildTurnoGet(IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets)
    {
        var row = resultSets.ElementAtOrDefault(0)?.FirstOrDefault();
        if (row is null)
        {
            return Fail("NOT_FOUND", "Turno no encontrado.");
        }

        var keyed = Keyed(row);
        return Ok(MapTurnoPayload(keyed));
    }

    private static PartesOutcome BuildTurnoMutation(
        IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets,
        string accion)
    {
        var row = resultSets.ElementAtOrDefault(0)?.FirstOrDefault();
        if (row is null)
        {
            return Fail("SQL_ERROR", $"Respuesta vacia del SP Turnos ({accion}).");
        }

        var keyed = Keyed(row);
        var resultCode = GetString(keyed, "resultCode") ?? string.Empty;
        if (!string.Equals(resultCode, "OK", StringComparison.OrdinalIgnoreCase))
        {
            return resultCode switch
            {
                "NOT_FOUND" => Fail("NOT_FOUND", "Turno no encontrado."),
                "DUPLICATE_CODE" => Fail("DUPLICATE_CODE", "El codigo de turno ya existe."),
                "INVALID_PARAMETERS" => Fail("INVALID_PARAMETERS", $"Parametros invalidos para {accion} turno."),
                "INVALID_SCHEDULE" => Fail("INVALID_SCHEDULE", "hora_fin debe ser posterior a hora_inicio."),
                "REFERENCED" => Fail("REFERENCED", "No se pudo eliminar el turno por referencias relacionadas."),
                "tablaNoExiste" => Fail("SQL_ERROR", "Tabla PQ_PRD_TURNOS no disponible."),
                _ => Fail(string.IsNullOrWhiteSpace(resultCode) ? "SQL_ERROR" : resultCode,
                    $"Error al {accion} turno ({resultCode}).")
            };
        }

        return Ok(MapTurnoPayload(keyed));
    }

    private static PartesOutcome BuildTurnoDelete(IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets)
    {
        var row = resultSets.ElementAtOrDefault(0)?.FirstOrDefault();
        if (row is null)
        {
            return Fail("SQL_ERROR", "Respuesta vacia del SP TurnosDelete.");
        }

        var keyed = Keyed(row);
        var resultCode = GetString(keyed, "resultCode") ?? string.Empty;
        if (!string.Equals(resultCode, "OK", StringComparison.OrdinalIgnoreCase))
        {
            return resultCode switch
            {
                "NOT_FOUND" => Fail("NOT_FOUND", "Turno no encontrado."),
                "INVALID_PARAMETERS" => Fail("INVALID_PARAMETERS", "El parametro id_turno es obligatorio y debe ser > 0."),
                "REFERENCED" => Fail("REFERENCED", "No se pudo eliminar el turno por referencias relacionadas."),
                "tablaNoExiste" => Fail("SQL_ERROR", "Tabla PQ_PRD_TURNOS no disponible."),
                _ => Fail(string.IsNullOrWhiteSpace(resultCode) ? "SQL_ERROR" : resultCode,
                    $"Error al eliminar turno ({resultCode}).")
            };
        }

        return Ok(new Dictionary<string, object?>
        {
            ["id"] = GetInt(keyed, "id") ?? 0,
            ["eliminado"] = GetBool(keyed, "eliminado") ?? true
        });
    }

    private static Dictionary<string, object?> MapTurnoPayload(Dictionary<string, object?> keyed) =>
        new()
        {
            ["id"] = GetInt(keyed, "id") ?? 0,
            ["codigo"] = GetString(keyed, "codigo") ?? string.Empty,
            ["nombre"] = GetString(keyed, "nombre") ?? string.Empty,
            ["hora_inicio"] = GetString(keyed, "hora_inicio"),
            ["hora_fin"] = GetString(keyed, "hora_fin"),
            ["activo"] = GetBool(keyed, "activo") ?? false
        };

    private static PartesOutcome BuildConceptoTiempoGet(IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets)
    {
        var row = resultSets.ElementAtOrDefault(0)?.FirstOrDefault();
        if (row is null)
        {
            return Fail("NOT_FOUND", "Concepto de tiempo no encontrado.");
        }

        var keyed = Keyed(row);
        return Ok(MapConceptoTiempoPayload(keyed, includeAviso: false));
    }

    private static PartesOutcome BuildConceptoTiempoMutation(
        IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets,
        string accion)
    {
        var row = resultSets.ElementAtOrDefault(0)?.FirstOrDefault();
        if (row is null)
        {
            return Fail("SQL_ERROR", $"Respuesta vacia del SP ConceptosTiempo ({accion}).");
        }

        var keyed = Keyed(row);
        var resultCode = GetString(keyed, "resultCode") ?? string.Empty;
        if (!string.Equals(resultCode, "OK", StringComparison.OrdinalIgnoreCase))
        {
            return resultCode switch
            {
                "NOT_FOUND" => Fail("NOT_FOUND", "Concepto de tiempo no encontrado."),
                "DUPLICATE_CODE" => Fail("DUPLICATE_CODE", "El codigo de concepto de tiempo ya existe."),
                "INVALID_PARAMETERS" => Fail("INVALID_PARAMETERS", $"Parametros invalidos para {accion} concepto de tiempo."),
                "TIPO_TAREA_NOT_FOUND" => Fail("TIPO_TAREA_NOT_FOUND", "El tipo de tarea indicado no existe o no esta activo."),
                "HABITUAL_SIN_TIPO" => Fail("HABITUAL_SIN_TIPO", "Debe indicar un tipo de tarea para marcar el concepto como habitual."),
                "REFERENCED" => Fail("REFERENCED", "No se pudo eliminar el concepto de tiempo por referencias relacionadas."),
                "tablaNoExiste" => Fail("SQL_ERROR", "Tabla PQ_PRD_CONCEPTOS_TIEMPO no disponible."),
                "schemaIncompleto" => Fail("SQL_ERROR", "Schema PQ_PRD_CONCEPTOS_TIEMPO incompleto (HABITUAL/ID_TIPO_TAREA)."),
                _ => Fail(string.IsNullOrWhiteSpace(resultCode) ? "SQL_ERROR" : resultCode,
                    $"Error al {accion} concepto de tiempo ({resultCode}).")
            };
        }

        return Ok(MapConceptoTiempoPayload(keyed, includeAviso: true));
    }

    private static PartesOutcome BuildConceptoTiempoDelete(IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets)
    {
        var row = resultSets.ElementAtOrDefault(0)?.FirstOrDefault();
        if (row is null)
        {
            return Fail("SQL_ERROR", "Respuesta vacia del SP ConceptosTiempoDelete.");
        }

        var keyed = Keyed(row);
        var resultCode = GetString(keyed, "resultCode") ?? string.Empty;
        if (!string.Equals(resultCode, "OK", StringComparison.OrdinalIgnoreCase))
        {
            return resultCode switch
            {
                "NOT_FOUND" => Fail("NOT_FOUND", "Concepto de tiempo no encontrado."),
                "INVALID_PARAMETERS" => Fail("INVALID_PARAMETERS", "El parametro id_concepto_tiempo es obligatorio y debe ser > 0."),
                "REFERENCED" => Fail("REFERENCED", "No se pudo eliminar el concepto de tiempo por referencias relacionadas."),
                "tablaNoExiste" => Fail("SQL_ERROR", "Tabla PQ_PRD_CONCEPTOS_TIEMPO no disponible."),
                _ => Fail(string.IsNullOrWhiteSpace(resultCode) ? "SQL_ERROR" : resultCode,
                    $"Error al eliminar concepto de tiempo ({resultCode}).")
            };
        }

        return Ok(new Dictionary<string, object?>
        {
            ["id"] = GetInt(keyed, "id") ?? 0,
            ["eliminado"] = GetBool(keyed, "eliminado") ?? true
        });
    }

    private static Dictionary<string, object?> MapConceptoTiempoPayload(
        Dictionary<string, object?> keyed,
        bool includeAviso)
    {
        var payload = new Dictionary<string, object?>
        {
            ["id"] = GetInt(keyed, "id") ?? 0,
            ["codigo"] = GetString(keyed, "codigo") ?? string.Empty,
            ["nombre"] = GetString(keyed, "nombre") ?? string.Empty,
            ["es_productivo"] = GetBool(keyed, "es_productivo") ?? false,
            ["activo"] = GetBool(keyed, "activo") ?? false,
            ["habitual"] = GetBool(keyed, "habitual") ?? false,
            ["id_tipo_tarea"] = GetInt(keyed, "id_tipo_tarea"),
            ["tipo_tarea_codigo"] = GetString(keyed, "tipo_tarea_codigo"),
            ["tipo_tarea_nombre"] = GetString(keyed, "tipo_tarea_nombre"),
            ["tipo_tarea_label"] = GetString(keyed, "tipo_tarea_label")
        };

        if (includeAviso)
        {
            var aviso = MapAvisoHabitual(keyed);
            if (aviso is not null)
            {
                payload["aviso_habitual"] = aviso;
            }
        }

        return payload;
    }

    private static Dictionary<string, object?>? MapAvisoHabitual(Dictionary<string, object?> keyed)
    {
        var reemplazado = GetBool(keyed, "aviso_reemplazado") ?? false;
        if (!reemplazado)
        {
            return null;
        }

        return new Dictionary<string, object?>
        {
            ["reemplazado"] = true,
            ["concepto_anterior"] = new Dictionary<string, object?>
            {
                ["id"] = GetInt(keyed, "aviso_concepto_id") ?? 0,
                ["codigo"] = GetString(keyed, "aviso_concepto_codigo") ?? string.Empty,
                ["nombre"] = GetString(keyed, "aviso_concepto_nombre") ?? string.Empty
            }
        };
    }

    private static PartesOutcome BuildTipoTareaGet(IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets)
    {
        var row = resultSets.ElementAtOrDefault(0)?.FirstOrDefault();
        if (row is null)
        {
            return Fail("NOT_FOUND", "Tipo de tarea no encontrado.");
        }

        var keyed = Keyed(row);
        return Ok(MapTipoTareaPayload(keyed));
    }

    private static PartesOutcome BuildTipoTareaMutation(
        IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets,
        string accion)
    {
        var row = resultSets.ElementAtOrDefault(0)?.FirstOrDefault();
        if (row is null)
        {
            return Fail("SQL_ERROR", $"Respuesta vacia del SP TiposTarea ({accion}).");
        }

        var keyed = Keyed(row);
        var resultCode = GetString(keyed, "resultCode") ?? string.Empty;
        if (!string.Equals(resultCode, "OK", StringComparison.OrdinalIgnoreCase))
        {
            return resultCode switch
            {
                "NOT_FOUND" => Fail("NOT_FOUND", "Tipo de tarea no encontrado."),
                "DUPLICATE_CODE" => Fail("DUPLICATE_CODE", "El codigo de tipo de tarea ya existe."),
                "INVALID_PARAMETERS" => Fail("INVALID_PARAMETERS", $"Parametros invalidos para {accion} tipo de tarea."),
                "REFERENCED" => Fail("REFERENCED", "No se pudo eliminar el tipo de tarea por referencias relacionadas."),
                "tablaNoExiste" => Fail("SQL_ERROR", "Tabla PQ_PRD_TIPOS_TAREA no disponible."),
                _ => Fail(string.IsNullOrWhiteSpace(resultCode) ? "SQL_ERROR" : resultCode,
                    $"Error al {accion} tipo de tarea ({resultCode}).")
            };
        }

        return Ok(MapTipoTareaPayload(keyed));
    }

    private static PartesOutcome BuildTipoTareaDelete(IReadOnlyList<IReadOnlyList<Dictionary<string, object?>>> resultSets)
    {
        var row = resultSets.ElementAtOrDefault(0)?.FirstOrDefault();
        if (row is null)
        {
            return Fail("SQL_ERROR", "Respuesta vacia del SP TiposTareaDelete.");
        }

        var keyed = Keyed(row);
        var resultCode = GetString(keyed, "resultCode") ?? string.Empty;
        if (!string.Equals(resultCode, "OK", StringComparison.OrdinalIgnoreCase))
        {
            return resultCode switch
            {
                "NOT_FOUND" => Fail("NOT_FOUND", "Tipo de tarea no encontrado."),
                "INVALID_PARAMETERS" => Fail("INVALID_PARAMETERS", "El parametro id_tipo_tarea es obligatorio y debe ser > 0."),
                "REFERENCED" => Fail("REFERENCED", "No se pudo eliminar el tipo de tarea por referencias relacionadas."),
                "tablaNoExiste" => Fail("SQL_ERROR", "Tabla PQ_PRD_TIPOS_TAREA no disponible."),
                _ => Fail(string.IsNullOrWhiteSpace(resultCode) ? "SQL_ERROR" : resultCode,
                    $"Error al eliminar tipo de tarea ({resultCode}).")
            };
        }

        return Ok(new Dictionary<string, object?>
        {
            ["id"] = GetInt(keyed, "id") ?? 0,
            ["eliminado"] = GetBool(keyed, "eliminado") ?? true
        });
    }

    private static Dictionary<string, object?> MapTipoTareaPayload(Dictionary<string, object?> keyed) =>
        new()
        {
            ["id"] = GetInt(keyed, "id") ?? 0,
            ["codigo"] = GetString(keyed, "codigo") ?? string.Empty,
            ["nombre"] = GetString(keyed, "nombre") ?? string.Empty,
            ["activo"] = GetBool(keyed, "activo") ?? false
        };

    private static bool TryNormalizeOrdenTrabajoGetParams(
        Dictionary<string, object?> spParams,
        out bool byId,
        out string errorMessage)
    {
        byId = false;
        errorMessage = string.Empty;

        if (TryGetPositiveInt(spParams, "IdOrdenTrabajo", out var idOrdenTrabajo))
        {
            byId = true;
            spParams["IdOrdenTrabajo"] = idOrdenTrabajo;
            spParams["CodigoOt"] = null;
            return true;
        }

        var codigo = spParams.TryGetValue("CodigoOt", out var codigoRaw) && codigoRaw is not null
            ? codigoRaw.ToString()?.Trim()
            : null;

        if (!string.IsNullOrWhiteSpace(codigo))
        {
            byId = false;
            spParams["IdOrdenTrabajo"] = 0;
            spParams["CodigoOt"] = codigo;
            return true;
        }

        errorMessage = "Debe indicar id_orden_trabajo (> 0) o codigo_ot.";
        return false;
    }

    private static bool TryNormalizeMaquinaCreateParams(
        Dictionary<string, object?> spParams,
        out string errorMessage)
    {
        errorMessage = string.Empty;

        var codigo = NormalizeRequiredString(spParams, "Codigo", maxLen: 20);
        if (codigo is null)
        {
            errorMessage = "El parametro codigo es obligatorio (max 20).";
            return false;
        }

        var nombre = NormalizeRequiredString(spParams, "Nombre", maxLen: 100);
        if (nombre is null)
        {
            errorMessage = "El parametro nombre es obligatorio (max 100).";
            return false;
        }

        spParams["Codigo"] = codigo;
        spParams["Nombre"] = nombre;

        if (!TryNormalizeOptionalBool(spParams, "Activa", "activa", requirePresent: false, out errorMessage))
        {
            return false;
        }

        if (!spParams.ContainsKey("Activa"))
        {
            spParams["Activa"] = true;
        }

        return true;
    }

    private static bool TryNormalizeMaquinaUpdateParams(
        Dictionary<string, object?> spParams,
        out string errorMessage)
    {
        errorMessage = string.Empty;

        if (!TryGetPositiveInt(spParams, "IdMaquina", out var idMaquina))
        {
            errorMessage = "El parametro id_maquina es obligatorio y debe ser > 0.";
            return false;
        }

        spParams["IdMaquina"] = idMaquina;

        var hasNombre = spParams.TryGetValue("Nombre", out var nombreRaw) && nombreRaw is not null;
        var hasActiva = spParams.TryGetValue("Activa", out var _) && spParams["Activa"] is not null;

        if (!hasNombre && !hasActiva)
        {
            errorMessage = "Debe enviar al menos nombre o activa.";
            return false;
        }

        if (hasNombre)
        {
            var nombre = NormalizeRequiredString(spParams, "Nombre", maxLen: 100);
            if (nombre is null)
            {
                errorMessage = "El parametro nombre no puede ser vacio (max 100).";
                return false;
            }

            spParams["Nombre"] = nombre;
        }

        if (hasActiva && !TryNormalizeOptionalBool(spParams, "Activa", "activa", requirePresent: true, out errorMessage))
        {
            return false;
        }

        return true;
    }

    private static bool TryNormalizeOperacionUpdateParams(
        Dictionary<string, object?> spParams,
        out string errorMessage)
    {
        errorMessage = string.Empty;

        if (!TryGetPositiveInt(spParams, "IdOperacion", out var idOperacion))
        {
            errorMessage = "El parametro id_operacion es obligatorio y debe ser > 0.";
            return false;
        }

        spParams["IdOperacion"] = idOperacion;

        var hasNombre = spParams.TryGetValue("Nombre", out var nombreRaw) && nombreRaw is not null;
        var hasActiva = spParams.TryGetValue("Activa", out var _) && spParams["Activa"] is not null;

        if (!hasNombre && !hasActiva)
        {
            errorMessage = "Debe enviar al menos nombre o activa.";
            return false;
        }

        if (hasNombre)
        {
            var nombre = NormalizeRequiredString(spParams, "Nombre", maxLen: 100);
            if (nombre is null)
            {
                errorMessage = "El parametro nombre no puede ser vacio (max 100).";
                return false;
            }

            spParams["Nombre"] = nombre;
        }

        if (hasActiva && !TryNormalizeOptionalBool(spParams, "Activa", "activa", requirePresent: true, out errorMessage))
        {
            return false;
        }

        return true;
    }

    private static bool TryNormalizeTipoTareaCreateParams(
        Dictionary<string, object?> spParams,
        out string errorMessage)
    {
        errorMessage = string.Empty;

        var codigo = NormalizeRequiredString(spParams, "Codigo", maxLen: 20);
        if (codigo is null)
        {
            errorMessage = "El parametro codigo es obligatorio (max 20).";
            return false;
        }

        var nombre = NormalizeRequiredString(spParams, "Nombre", maxLen: 100);
        if (nombre is null)
        {
            errorMessage = "El parametro nombre es obligatorio (max 100).";
            return false;
        }

        spParams["Codigo"] = codigo;
        spParams["Nombre"] = nombre;

        if (!TryNormalizeOptionalBool(spParams, "Activo", "activo", requirePresent: false, out errorMessage))
        {
            return false;
        }

        if (!spParams.ContainsKey("Activo"))
        {
            spParams["Activo"] = true;
        }

        return true;
    }

    private static bool TryNormalizeTipoTareaUpdateParams(
        Dictionary<string, object?> spParams,
        out string errorMessage)
    {
        errorMessage = string.Empty;

        if (!TryGetPositiveInt(spParams, "IdTipoTarea", out var idTipoTarea))
        {
            errorMessage = "El parametro id_tipo_tarea es obligatorio y debe ser > 0.";
            return false;
        }

        spParams["IdTipoTarea"] = idTipoTarea;

        var hasNombre = spParams.TryGetValue("Nombre", out var nombreRaw) && nombreRaw is not null;
        var hasActivo = spParams.TryGetValue("Activo", out var _) && spParams["Activo"] is not null;

        if (!hasNombre && !hasActivo)
        {
            errorMessage = "Debe enviar al menos nombre o activo.";
            return false;
        }

        if (hasNombre)
        {
            var nombre = NormalizeRequiredString(spParams, "Nombre", maxLen: 100);
            if (nombre is null)
            {
                errorMessage = "El parametro nombre no puede ser vacio (max 100).";
                return false;
            }

            spParams["Nombre"] = nombre;
        }

        if (hasActivo && !TryNormalizeOptionalBool(spParams, "Activo", "activo", requirePresent: true, out errorMessage))
        {
            return false;
        }

        return true;
    }

    private static bool TryNormalizeConceptoTiempoCreateParams(
        Dictionary<string, object?> spParams,
        out string errorCode,
        out string errorMessage)
    {
        errorCode = "INVALID_PARAMETERS";
        errorMessage = string.Empty;

        var codigo = NormalizeRequiredString(spParams, "Codigo", maxLen: 20);
        if (codigo is null)
        {
            errorMessage = "El parametro codigo es obligatorio (max 20).";
            return false;
        }

        var nombre = NormalizeRequiredString(spParams, "Nombre", maxLen: 120);
        if (nombre is null)
        {
            errorMessage = "El parametro nombre es obligatorio (max 120).";
            return false;
        }

        spParams["Codigo"] = codigo;
        spParams["Nombre"] = nombre;

        if (!TryNormalizeOptionalBool(spParams, "EsProductivo", "es_productivo", requirePresent: false, out errorMessage))
        {
            return false;
        }

        if (!spParams.ContainsKey("EsProductivo") || spParams["EsProductivo"] is null)
        {
            spParams["EsProductivo"] = true;
        }

        if (!TryNormalizeOptionalBool(spParams, "Activo", "activo", requirePresent: false, out errorMessage))
        {
            return false;
        }

        if (!spParams.ContainsKey("Activo") || spParams["Activo"] is null)
        {
            spParams["Activo"] = true;
        }

        if (!TryNormalizeOptionalBool(spParams, "Habitual", "habitual", requirePresent: false, out errorMessage))
        {
            return false;
        }

        if (!spParams.ContainsKey("Habitual") || spParams["Habitual"] is null)
        {
            spParams["Habitual"] = false;
        }

        if (!TryGetPositiveInt(spParams, "IdTipoTarea", out var idTipoTarea))
        {
            var habitual = spParams["Habitual"] is true;
            if (habitual)
            {
                errorCode = "HABITUAL_SIN_TIPO";
                errorMessage = "Debe indicar un tipo de tarea para marcar el concepto como habitual.";
                return false;
            }

            errorMessage = "El parametro id_tipo_tarea es obligatorio y debe ser > 0.";
            return false;
        }

        spParams["IdTipoTarea"] = idTipoTarea;
        return true;
    }

    private static bool TryNormalizeConceptoTiempoUpdateParams(
        Dictionary<string, object?> spParams,
        IReadOnlyDictionary<string, object?> jobParameters,
        out string errorCode,
        out string errorMessage)
    {
        errorCode = "INVALID_PARAMETERS";
        errorMessage = string.Empty;

        if (!TryGetPositiveInt(spParams, "IdConceptoTiempo", out var idConceptoTiempo))
        {
            errorMessage = "El parametro id_concepto_tiempo es obligatorio y debe ser > 0.";
            return false;
        }

        spParams["IdConceptoTiempo"] = idConceptoTiempo;

        var hasNombre = spParams.TryGetValue("Nombre", out var nombreRaw) && nombreRaw is not null;
        var hasEsProductivo = spParams.TryGetValue("EsProductivo", out var _) && spParams["EsProductivo"] is not null;
        var hasActivo = spParams.TryGetValue("Activo", out var _) && spParams["Activo"] is not null;
        var hasHabitual = spParams.TryGetValue("Habitual", out var _) && spParams["Habitual"] is not null;
        var setIdTipoTarea = HasJobParameter(jobParameters, "id_tipo_tarea");

        if (!hasNombre && !hasEsProductivo && !hasActivo && !hasHabitual && !setIdTipoTarea)
        {
            errorMessage = "Debe enviar al menos nombre, es_productivo, activo, habitual o id_tipo_tarea.";
            return false;
        }

        if (hasNombre)
        {
            var nombre = NormalizeRequiredString(spParams, "Nombre", maxLen: 120);
            if (nombre is null)
            {
                errorMessage = "El parametro nombre no puede ser vacio (max 120).";
                return false;
            }

            spParams["Nombre"] = nombre;
        }

        if (hasEsProductivo
            && !TryNormalizeOptionalBool(spParams, "EsProductivo", "es_productivo", requirePresent: true, out errorMessage))
        {
            return false;
        }

        if (hasActivo && !TryNormalizeOptionalBool(spParams, "Activo", "activo", requirePresent: true, out errorMessage))
        {
            return false;
        }

        if (hasHabitual && !TryNormalizeOptionalBool(spParams, "Habitual", "habitual", requirePresent: true, out errorMessage))
        {
            return false;
        }

        spParams["SetIdTipoTarea"] = setIdTipoTarea;
        if (setIdTipoTarea)
        {
            if (!TryGetPositiveInt(spParams, "IdTipoTarea", out var idTipoTarea))
            {
                errorMessage = "El parametro id_tipo_tarea debe ser > 0.";
                return false;
            }

            spParams["IdTipoTarea"] = idTipoTarea;
        }
        else
        {
            spParams.Remove("IdTipoTarea");
        }

        return true;
    }

    private static bool TryNormalizeTurnoCreateParams(
        Dictionary<string, object?> spParams,
        out string errorCode,
        out string errorMessage)
    {
        errorCode = "INVALID_PARAMETERS";
        errorMessage = string.Empty;

        var codigo = NormalizeRequiredString(spParams, "Codigo", maxLen: 20);
        if (codigo is null)
        {
            errorMessage = "El parametro codigo es obligatorio (max 20).";
            return false;
        }

        var nombre = NormalizeRequiredString(spParams, "Nombre", maxLen: 50);
        if (nombre is null)
        {
            errorMessage = "El parametro nombre es obligatorio (max 50).";
            return false;
        }

        spParams["Codigo"] = codigo;
        spParams["Nombre"] = nombre;

        if (!TryNormalizeOptionalBool(spParams, "Activo", "activo", requirePresent: false, out errorMessage))
        {
            return false;
        }

        if (!spParams.ContainsKey("Activo"))
        {
            spParams["Activo"] = true;
        }

        if (!TryNormalizeTurnoHora(spParams, "HoraInicio", "hora_inicio", allowEndOfDay: false, out errorMessage))
        {
            return false;
        }

        if (!TryNormalizeTurnoHora(spParams, "HoraFin", "hora_fin", allowEndOfDay: true, out errorMessage))
        {
            return false;
        }

        if (!TryValidateTurnoSchedule(spParams, out errorCode, out errorMessage))
        {
            return false;
        }

        return true;
    }

    private static bool TryNormalizeTurnoUpdateParams(
        Dictionary<string, object?> spParams,
        IReadOnlyDictionary<string, object?> jobParameters,
        out string errorCode,
        out string errorMessage)
    {
        errorCode = "INVALID_PARAMETERS";
        errorMessage = string.Empty;

        if (!TryGetPositiveInt(spParams, "IdTurno", out var idTurno))
        {
            errorMessage = "El parametro id_turno es obligatorio y debe ser > 0.";
            return false;
        }

        spParams["IdTurno"] = idTurno;

        var hasNombre = spParams.TryGetValue("Nombre", out var nombreRaw) && nombreRaw is not null;
        var hasActivo = spParams.TryGetValue("Activo", out var _) && spParams["Activo"] is not null;
        var setHoraInicio = HasJobParameter(jobParameters, "hora_inicio");
        var setHoraFin = HasJobParameter(jobParameters, "hora_fin");

        if (!hasNombre && !hasActivo && !setHoraInicio && !setHoraFin)
        {
            errorMessage = "Debe enviar al menos nombre, activo, hora_inicio o hora_fin.";
            return false;
        }

        if (hasNombre)
        {
            var nombre = NormalizeRequiredString(spParams, "Nombre", maxLen: 50);
            if (nombre is null)
            {
                errorMessage = "El parametro nombre no puede ser vacio (max 50).";
                return false;
            }

            spParams["Nombre"] = nombre;
        }

        if (hasActivo && !TryNormalizeOptionalBool(spParams, "Activo", "activo", requirePresent: true, out errorMessage))
        {
            return false;
        }

        spParams["SetHoraInicio"] = setHoraInicio;
        spParams["SetHoraFin"] = setHoraFin;

        if (setHoraInicio)
        {
            if (!TryNormalizeTurnoHora(spParams, "HoraInicio", "hora_inicio", allowEndOfDay: false, out errorMessage))
            {
                return false;
            }
        }
        else
        {
            spParams.Remove("HoraInicio");
        }

        if (setHoraFin)
        {
            if (!TryNormalizeTurnoHora(spParams, "HoraFin", "hora_fin", allowEndOfDay: true, out errorMessage))
            {
                return false;
            }
        }
        else
        {
            spParams.Remove("HoraFin");
        }

        if (setHoraInicio && setHoraFin
            && !TryValidateTurnoSchedule(spParams, out errorCode, out errorMessage))
        {
            return false;
        }

        return true;
    }

    private static bool HasJobParameter(IReadOnlyDictionary<string, object?> parameters, string key)
    {
        foreach (var pair in parameters)
        {
            if (string.Equals(pair.Key, key, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryNormalizeTurnoHora(
        Dictionary<string, object?> spParams,
        string spKey,
        string jobParamName,
        bool allowEndOfDay,
        out string errorMessage)
    {
        errorMessage = string.Empty;

        if (!spParams.TryGetValue(spKey, out var raw) || raw is null)
        {
            spParams[spKey] = null;
            return true;
        }

        var rawText = raw.ToString()?.Trim();
        if (string.IsNullOrWhiteSpace(rawText))
        {
            spParams[spKey] = null;
            return true;
        }

        var normalized = ParseTurnoTime(rawText, allowEndOfDay);
        if (normalized is null)
        {
            errorMessage = allowEndOfDay
                ? $"El parametro {jobParamName} debe ser HH:mm[:ss] (00:00-23:59) o 24:00."
                : $"El parametro {jobParamName} debe ser HH:mm[:ss] (00:00-23:59; 24:00 no aplica).";
            return false;
        }

        spParams[spKey] = normalized;
        return true;
    }

    private static string? ParseTurnoTime(string value, bool allowEndOfDay)
    {
        var v = value.Trim();
        if (System.Text.RegularExpressions.Regex.IsMatch(v, @"^24:00(:00)?$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
        {
            return allowEndOfDay ? "23:59:59" : null;
        }

        var match = System.Text.RegularExpressions.Regex.Match(v, @"^(\d{1,2}):(\d{2})(?::(\d{2}))?$");
        if (!match.Success)
        {
            return null;
        }

        var h = int.Parse(match.Groups[1].Value);
        var m = int.Parse(match.Groups[2].Value);
        var s = match.Groups[3].Success ? int.Parse(match.Groups[3].Value) : 0;
        if (h < 0 || h > 23 || m < 0 || m > 59 || s < 0 || s > 59)
        {
            return null;
        }

        return $"{h:D2}:{m:D2}:{s:D2}";
    }

    private static bool TryValidateTurnoSchedule(
        IReadOnlyDictionary<string, object?> spParams,
        out string errorCode,
        out string errorMessage)
    {
        errorCode = "INVALID_PARAMETERS";
        errorMessage = string.Empty;

        var horaInicio = spParams.TryGetValue("HoraInicio", out var inicioRaw) ? inicioRaw as string : null;
        var horaFin = spParams.TryGetValue("HoraFin", out var finRaw) ? finRaw as string : null;
        if (string.IsNullOrWhiteSpace(horaInicio) || string.IsNullOrWhiteSpace(horaFin))
        {
            return true;
        }

        if (HoraFinToMinutes(horaFin) <= HoraInicioToMinutes(horaInicio))
        {
            errorCode = "INVALID_SCHEDULE";
            errorMessage = "hora_fin debe ser posterior a hora_inicio.";
            return false;
        }

        return true;
    }

    private static int HoraInicioToMinutes(string hms)
    {
        var parts = hms.Split(':');
        return int.Parse(parts[0]) * 60 + int.Parse(parts[1]);
    }

    private static int HoraFinToMinutes(string hms)
    {
        if (System.Text.RegularExpressions.Regex.IsMatch(hms, @"^23:59:59(\.\d+)?$"))
        {
            return 1440;
        }

        var parts = hms.Split(':');
        return int.Parse(parts[0]) * 60 + int.Parse(parts[1]);
    }

    private static bool TryNormalizeOptionalBool(
        Dictionary<string, object?> spParams,
        string spKey,
        string jobParamName,
        bool requirePresent,
        out string errorMessage)
    {
        errorMessage = string.Empty;

        if (!spParams.TryGetValue(spKey, out var raw) || raw is null)
        {
            if (requirePresent)
            {
                errorMessage = $"El parametro {jobParamName} debe ser booleano.";
                return false;
            }

            return true;
        }

        if (raw is bool)
        {
            return true;
        }

        if (raw is int i)
        {
            spParams[spKey] = i != 0;
            return true;
        }

        if (raw is string s && bool.TryParse(s, out var parsed))
        {
            spParams[spKey] = parsed;
            return true;
        }

        if (raw is string s2 && int.TryParse(s2, out var asInt))
        {
            spParams[spKey] = asInt != 0;
            return true;
        }

        errorMessage = $"El parametro {jobParamName} debe ser booleano.";
        return false;
    }

    private static string? NormalizeRequiredString(
        IReadOnlyDictionary<string, object?> parameters,
        string key,
        int maxLen)
    {
        if (!parameters.TryGetValue(key, out var raw) || raw is null)
        {
            return null;
        }

        var value = raw.ToString()?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(value) || value.Length > maxLen)
        {
            return null;
        }

        return value;
    }

    private static Dictionary<string, object?> MapParametro(Dictionary<string, object?> row)
    {
        var keyed = Keyed(row);
        return new Dictionary<string, object?>
        {
            ["Programa"] = GetString(keyed, "Programa") ?? string.Empty,
            ["Clave"] = GetString(keyed, "Clave") ?? string.Empty,
            ["TipoValor"] = GetString(keyed, "Tipo_Valor") ?? GetString(keyed, "TipoValor"),
            ["ValorBool"] = GetBool(keyed, "Valor_Bool") ?? GetBool(keyed, "ValorBool"),
            ["ValorInt"] = GetInt(keyed, "Valor_Int") ?? GetInt(keyed, "ValorInt"),
            ["ValorDecimal"] = GetDecimal(keyed, "Valor_Decimal") ?? GetDecimal(keyed, "ValorDecimal"),
            ["ValorString"] = GetString(keyed, "Valor_String") ?? GetString(keyed, "ValorString"),
            ["ValorDateTime"] = GetDateTime(keyed, "Valor_DateTime") ?? GetDateTime(keyed, "ValorDateTime"),
            ["ValorText"] = GetString(keyed, "Valor_Text") ?? GetString(keyed, "ValorText")
        };
    }

    private static Dictionary<string, object?> MapItem(Dictionary<string, object?> row)
    {
        var keyed = Keyed(row);
        return new Dictionary<string, object?>
        {
            ["id_parte_entrada"] = GetInt(keyed, "id_parte_entrada"),
            ["fecha"] = GetDateTime(keyed, "fecha_parte"),
            ["id_turno"] = GetInt(keyed, "id_turno"),
            ["turno_codigo"] = GetString(keyed, "turno_codigo"),
            ["id_operario"] = GetInt(keyed, "id_operario"),
            ["operario_nombre"] = GetString(keyed, "operario_nombre"),
            ["id_orden_trabajo"] = GetInt(keyed, "id_orden_trabajo"),
            ["codigo_ot"] = GetString(keyed, "codigo_ot"),
            ["id_articulo"] = GetInt(keyed, "id_articulo"),
            ["id_operacion"] = GetInt(keyed, "id_operacion"),
            ["operacion_codigo"] = GetString(keyed, "operacion_codigo"),
            ["id_maquina"] = GetInt(keyed, "id_maquina"),
            ["maquina_codigo"] = GetString(keyed, "maquina_codigo"),
            ["unidad_negocio"] = GetString(keyed, "unidad_negocio"),
            ["id_tipo_tarea"] = GetInt(keyed, "id_tipo_tarea"),
            ["id_concepto_tiempo"] = GetInt(keyed, "id_concepto_tiempo"),
            ["concepto_codigo"] = GetString(keyed, "concepto_codigo"),
            ["productive_minutes"] = GetInt(keyed, "productive_minutes") ?? 0,
            ["non_productive_minutes"] = GetInt(keyed, "non_productive_minutes") ?? 0,
            ["units_done"] = GetDecimal(keyed, "units_done"),
            ["std_units_per_hour"] = GetDecimal(keyed, "std_units_per_hour"),
            ["theoretical_units"] = GetDecimal(keyed, "theoretical_units"),
            ["efficiency_pct"] = GetDecimal(keyed, "efficiency_pct")
        };
    }

    private static Dictionary<string, object?> MapResumen(Dictionary<string, object?>? row)
    {
        if (row is null)
        {
            return new Dictionary<string, object?>
            {
                ["productive_minutes"] = 0,
                ["non_productive_minutes"] = 0,
                ["theoretical_units"] = 0m,
                ["efficiency_pct"] = null
            };
        }

        var keyed = Keyed(row);
        var productive = GetInt(keyed, "total_productive_minutes") ?? 0;
        var nonProductive = GetInt(keyed, "total_non_productive_minutes") ?? 0;
        var theoretical = GetDecimal(keyed, "total_theoretical_units") ?? 0m;
        var unitsDone = GetDecimal(keyed, "total_units_done");

        decimal? efficiencyPct = null;
        if (theoretical > 0m && unitsDone is not null)
        {
            efficiencyPct = Math.Round(unitsDone.Value / theoretical * 100m, 2);
        }

        return new Dictionary<string, object?>
        {
            ["productive_minutes"] = productive,
            ["non_productive_minutes"] = nonProductive,
            ["theoretical_units"] = theoretical,
            ["efficiency_pct"] = efficiencyPct
        };
    }

    private static bool TryGetPositiveInt(
        IReadOnlyDictionary<string, object?> parameters,
        string key,
        out int value)
    {
        value = 0;
        if (!parameters.TryGetValue(key, out var raw) || raw is null)
        {
            return false;
        }

        value = raw switch
        {
            int i => i,
            long l => (int)l,
            string s when int.TryParse(s, out var p) => p,
            _ => 0
        };

        return value > 0;
    }

    private static Dictionary<string, object?> MapSpParameters(
        PartesOperationDefinition definition,
        IReadOnlyDictionary<string, object?> parameters)
    {
        var result = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        foreach (var jobName in definition.JobParameters)
        {
            if (!parameters.TryGetValue(jobName, out var raw))
            {
                continue;
            }

            var converted = ConvertValue(raw);
            if (converted is string s && string.IsNullOrWhiteSpace(s))
            {
                converted = null;
            }

            var spName = definition.JobToSpParameterMap.TryGetValue(jobName, out var mapped)
                ? mapped
                : jobName;
            result[spName] = converted;
        }

        return result;
    }

    private static object? ConvertValue(object? value)
    {
        if (value is null)
        {
            return null;
        }

        if (value is JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString(),
                JsonValueKind.Number when element.TryGetInt32(out var i) => i,
                JsonValueKind.Number when element.TryGetInt64(out var l) => l,
                JsonValueKind.Number => element.GetDecimal(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null,
                _ => element.ToString()
            };
        }

        return value;
    }

    private static string? ExtractString(IReadOnlyDictionary<string, object?> parameters, string key)
    {
        if (!parameters.TryGetValue(key, out var raw) || raw is null)
        {
            return null;
        }

        return raw switch
        {
            string s => s,
            JsonElement je when je.ValueKind == JsonValueKind.String => je.GetString(),
            _ => raw.ToString()
        };
    }

    private static Dictionary<string, object?> Keyed(Dictionary<string, object?> row) =>
        new(row, StringComparer.OrdinalIgnoreCase);

    private static string? GetString(Dictionary<string, object?> row, string key)
    {
        if (!row.TryGetValue(key, out var value) || value is null || value is DBNull)
        {
            return null;
        }

        return value.ToString();
    }

    private static bool? GetBool(Dictionary<string, object?> row, string key)
    {
        if (!row.TryGetValue(key, out var value) || value is null || value is DBNull)
        {
            return null;
        }

        if (value is bool b)
        {
            return b;
        }

        if (bool.TryParse(value.ToString(), out var parsed))
        {
            return parsed;
        }

        if (int.TryParse(value.ToString(), out var asInt))
        {
            return asInt != 0;
        }

        return null;
    }

    private static int? GetInt(Dictionary<string, object?> row, string key)
    {
        if (!row.TryGetValue(key, out var value) || value is null || value is DBNull)
        {
            return null;
        }

        return value switch
        {
            int i => i,
            long l => (int)l,
            _ => int.TryParse(value.ToString(), out var parsed) ? parsed : null
        };
    }

    private static decimal? GetDecimal(Dictionary<string, object?> row, string key)
    {
        if (!row.TryGetValue(key, out var value) || value is null || value is DBNull)
        {
            return null;
        }

        return value switch
        {
            decimal d => d,
            double dbl => (decimal)dbl,
            float f => (decimal)f,
            _ => decimal.TryParse(value.ToString(), out var parsed) ? parsed : null
        };
    }

    private static DateTime? GetDateTime(Dictionary<string, object?> row, string key)
    {
        if (!row.TryGetValue(key, out var value) || value is null || value is DBNull)
        {
            return null;
        }

        if (value is DateTime dt)
        {
            return dt;
        }

        return DateTime.TryParse(value.ToString(), out var parsed) ? parsed : null;
    }

    private static PartesOutcome Ok(object? data) =>
        new()
        {
            Status = JobStatuses.Success,
            Data = data
        };

    private static PartesOutcome Fail(string code, string message) =>
        new()
        {
            Status = JobStatuses.Failed,
            ErrorCode = code,
            ErrorMessage = message
        };
}
