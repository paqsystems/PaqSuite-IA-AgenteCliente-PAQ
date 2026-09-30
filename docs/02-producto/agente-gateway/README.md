# Paquete SDD — Reformulación PaqAgent + PaqGateway

**Esta carpeta ya no es la fuente de verdad del circuito SDD.**

La documentación normativa vive en `docs/`:

| Qué | Dónde |
|-----|--------|
| Contexto + DoD MVP | [docs/00-contexto/](../../00-contexto/) |
| Arquitectura | [docs/01-arquitectura/01-arquitectura-agente-gateway.md](../../01-arquitectura/01-arquitectura-agente-gateway.md) |
| SPEC-AGW-001 | [docs/02-producto/SPEC-AGW-001-producto.md](../SPEC-AGW-001-producto.md) |
| Decisiones | [docs/02-producto/decisiones-tecnicas.md](../decisiones-tecnicas.md) |
| HU | [docs/03-historias-usuario/001-Conectividad/](../../03-historias-usuario/001-Conectividad/) |
| TR | [docs/04-tareas/001-Conectividad/](../../04-tareas/001-Conectividad/) |
| Open SPEC | [docs/05-open-spec/001-Conectividad/](../../05-open-spec/001-Conectividad/) |
| Operación | [docs/06-operacion/](../../06-operacion/) |
| Informe 08 | [docs/08-control/08-informe-revision-ambiguedad.md](../../08-control/08-informe-revision-ambiguedad.md) |
| Manual del programador | [MANUAL-DEL-PROGRAMADOR.md](../../00-contexto/MANUAL-DEL-PROGRAMADOR.md) |
| Prompts de IA | [prompts/](../../../prompts/) |

## Qué queda aquí

Análisis y fase 2 (no alteran el MVP de conectividad):

| Archivo | Para qué |
|---------|----------|
| [../actualizacion-agentes-release-train.md](../actualizacion-agentes-release-train.md) | Release train, flota masiva, enlace Framework doc 34 |
| [plan-ciclo-sql-y-updates.md](plan-ciclo-sql-y-updates.md) | Análisis fase 2 SQL/update |
| [../circuito-actualizacion-agente-funcional.md](../circuito-actualizacion-agente-funcional.md) | Relato funcionales: oleadas, handshake, staff, Forge |
| [../circuito-objetos-sql-agente-funcional.md](../circuito-objetos-sql-agente-funcional.md) | Relato funcionales: objetos SQL en el mismo exe |
| [insumo-spec-agw-002-update-agente.md](insumo-spec-agw-002-update-agente.md) | Insumo Paso A (frente C; especificar junto al A) |
| [insumo-spec-agw-002-objetos-sql.md](insumo-spec-agw-002-objetos-sql.md) | Insumo Paso A (frente A; MULTI/MONO, install/update, alta empresa) |
| [SPEC-AGW-002-ciclo-sql-y-updates.md](SPEC-AGW-002-ciclo-sql-y-updates.md) | Placeholder SPEC fase 2 |
| [codex-definicion.md](codex-definicion.md) | Spec alternativa Codex (referencia) |
| [03-historias-usuario.md](03-historias-usuario.md) | Puntero al árbol HU |
| [04-tareas-mvp.md](04-tareas-mvp.md) | Puntero al árbol TR |

## Repos

| Repo | Qué se construye |
|------|------------------|
| `PaqSuite-IA-AgenteCliente-PAQ` (este) | Agente Windows, Gateway .NET, instalador |
| `PaqSuite-IA-TANGO` | Contrato Laravel: `empresas_conexion`, `AgentGatewayClient`, **sin** SQL directo en modo agente |

El código Laravel **no vive en este repo**.
