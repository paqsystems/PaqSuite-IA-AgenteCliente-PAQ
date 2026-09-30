# Actualización de agentes, release train y flota

| Campo | Valor |
|-------|--------|
| Repo | `PaqSuite-IA-AgenteCliente-PAQ` |
| Estado | Dirección técnica / producto (2026-09-30) |
| Audiencia | Ops PaqSystems, arquitectura, integración Tango–Framework–Agente |
| Producto inicial | **Tango** (modo `agent_id` + `client_id`); extensible a otros `{proyecto}` |

## Relación con el Framework (install / update SQL)

La **orquestación de instalación y actualización** (diccionario, empresas, menú, parámetros, manifiesto de stored procedures, modo directo vs jobs hacia el agente) se documenta en el repo **PaqSuite-IA-FRAMEWORK**:

- [`docs/02-producto/34-instalacion-actualizacion-proyecto.md`](../../../PaqSuite-IA-FRAMEWORK/docs/02-producto/34-instalacion-actualizacion-proyecto.md) — servicio de ciclo de vida, manifiesto (§11), relación con agentes (§14).

Este documento cubre el **otro eje**: binario/servicio del agente, **catálogo de runners/ops**, **actualización masiva en decenas de instalaciones** y **matriz de compatibilidad** con cada release del host (`laravel-core`) y del gateway.

Ambos temas responden a problemáticas distintas pero **van acoplados en cada release** cuando la instalación usa modo gateway:

| Cambio | Framework / host | Agente (este repo) |
|--------|------------------|---------------------|
| Nueva op DualPath (`gridLayouts.*`, `installation.update`, …) | `laravel-core` + host Tango | Runner + despliegue de flota en versión mínima |
| Manifiesto SQL/SP (install/update) | §11 del doc 34 | `SqlMigrationRunner` / jobs `installation.*` en el SQL del cliente |
| Solo bump de UI o lógica sin ops nuevas | Deploy Forge | Puede no exigir agente nuevo; validar matriz publicada |
| Instalación SQL directa (sin agente) | Orquestador directo | No aplica flota; el manifiesto SQL sigue alineado |

Referencias cruzadas Framework: [`33-agente-gateway-canal-dual-path.md`](../../../PaqSuite-IA-FRAMEWORK/docs/02-producto/33-agente-gateway-canal-dual-path.md), override [`07-agente-gateway-canal.md`](../../../PaqSuite-IA-FRAMEWORK/docs/10-overrides-framework/07-agente-gateway-canal.md), GEN-18 [`18-instalacion-y-actualizacion.md`](../../../PaqSuite-IA-FRAMEWORK/docs/02-producto/18-instalacion-y-actualizacion.md).

Documentos previos en **este** repo (no reemplazados; se complementan):

| Documento | Enfoque |
|-----------|---------|
| [plan-ciclo-sql-y-updates.md](agente-gateway/plan-ciclo-sql-y-updates.md) | Tres frentes A/B/C: SQL PQ, deploy Laravel, binario agente |
| [circuito-actualizacion-agente-funcional.md](circuito-actualizacion-agente-funcional.md) | Relato funcional: oleadas, handshake, mismo exe install/update |
| [circuito-objetos-sql-agente-funcional.md](circuito-objetos-sql-agente-funcional.md) | Objetos SQL en el instalador / runner |
| [empaquetado-instalador.md](../06-operacion/empaquetado-instalador.md) | `PaqAgentSetup.exe`, canal release D9 |

---

## 1. Tres problemas, tres dueños (resumen)

| ID | Problema | Ejecutor en modo gateway | Disparador típico |
|----|----------|---------------------------|-------------------|
| **A** | Bootstrap / update **SQL PQ** (DDL, seed, SP, alta empresa operativa) | Agente en el servidor del cliente (`SqlMigrationRunner`, jobs) | Install; `installation.update`; ABM empresa → `company.provision` / `seedEmpresaNueva` |
| **B** | Deploy **app Laravel** (Tango + `laravel-core`) | Forge / CI en AWS | Tag release producto |
| **C** | Nueva versión del **binario** PaqAgent | Canal de release + flota (pull/push) | Oleada ops; no confundir con solo Forge |

**Regla:** con `agent_id` y `client_id` informados, Forge **no** aplica migrate/seed/SP sobre el SQL del cliente. El host publica versión deseada y envía jobs; el agente ejecuta en local. Ver [plan-ciclo-sql-y-updates.md](agente-gateway/plan-ciclo-sql-y-updates.md).

Una actualización de producto en gateway **no se considera cerrada** hasta:

1. Host en versión objetivo.
2. Agente ≥ versión mínima del release (y gateway compatible).
3. Manifiesto SQL/SP de esa versión aplicado en el SQL del cliente (vía agente).

---

## 2. Release train (bundle de compatibilidad)

Cada oleada publicada por PaqSystems debería incluir un **bundle** explícito (catálogo en PAQSYSTEMS o artefacto versionado), por ejemplo:

```text
proyecto: tango
hostSdk: 1.3.9
gatewayMin: …
agentMin: 2.1.0
agentRecommended: 2.1.2
schemaDesired: 2026-09-30
opsCatalogHash: …
```

| Consumidor | Uso |
|------------|-----|
| Orquestador Framework (`34`) | Validar antes de marcar install/update completado |
| Host Tango | Handshake de capacidades; rechazar ops no soportadas |
| Agente | Comparar `appliedSchema` vs `desiredSchema`; aplicar paquetes pendientes |
| Ops | Dashboard % flota en `agentRecommended` |

El manifiesto SQL del Framework (§11 del doc 34) y los scripts embebidos o descargados por el agente deben compartir **misma versión lógica** que `schemaDesired`.

---

## 3. Actualización masiva de agentes (decenas de instalaciones)

Objetivo: no depender de RDP o copia manual instalación por instalación.

### 3.1. Inventario

Fuente de candidatos: filas `EMPRESAS_CONEXION` en `PAQSYSTEMS` con `agent_id` y `client_id` y `proyecto` (hoy `tango`). Evolución recomendada: tabla o extensión **AGENT_INSTANCE** (`agent_id`, `proyecto`, `agent_version`, `schema_applied`, `last_heartbeat`, `update_channel`, `update_state`).

### 3.2. Alternativas

| Enfoque | Mecanismo | Escala | Notas |
|---------|-----------|--------|--------|
| **Pull (recomendado a medio plazo)** | Agente consulta feed HTTPS (`/releases/agent?proyecto=tango&channel=stable`); descarga `PaqAgentSetup` o delta firmado; reinicia servicio; reporta heartbeat | Alta | Requiere salida HTTPS del cliente; canary por `client_id` |
| **Push vía PaqGateway** | Comando ops `agents:rollout` → job `agent.upgrade` por lotes | Alta con batching | Offline → cola/retry; no fallback SQL |
| **Release train central** | Job en PAQSYSTEMS: cohortes (10 %, región, versión actual) + estados `pending` / `applied` / `failed` | Ops | Combina pull o push |
| **RMM / GPO / paquetería privada** | MSI/zip en landing o feed interno; IT del cliente despliega | Variable | Plan B sin pull; menos granular por `client_id` |
| **Solo Forge (host)** | Subir Tango sin agente | — | **Anti-patrón** en gateway: desalineación ops/SP |

MVP histórico (D9): release GitHub + `PaqAgentSetup.exe` + SHA256; update manual. Fase siguiente: pull o push masivo + heartbeat con `agentVersion` (ver [circuito-actualizacion-agente-funcional.md](circuito-actualizacion-agente-funcional.md)).

### 3.3. Política de rollout

- Lotes pequeños y pausa entre oleadas.
- Una instalación fallida no debe marcar toda la cohorte como OK.
- Rollback de binario documentado (versión anterior firmada).
- Compatibilidad: no publicar host que exija `agentMin` superior sin artefacto de agente disponible en el feed.

---

## 4. Flujo integrado (visión)

```text
Release PaqSystems
  ├─ publica bundle (hostSdk, agentMin, schemaDesired)
  ├─ Forge → deploy Tango
  ├─ feed agente → PaqAgentSetup 2.1.2
  └─ manifiesto SQL → paquete para MigrationRunner

Por cada fila gateway (tango, …):
  1. Rollout binario (pull o push) hasta agent >= agentMin
  2. Job installation.update (Framework 34) → agente aplica SQL/SP
  3. Smoke: heartbeat + op de prueba + versión schema
```

Alta de **empresa operativa** MULTI: el ABM del host llama el mismo contrato que la instalación (`company.provision`); el agente ejecuta solo el alcance empresa, sin repetir diccionario. Detalle: doc 34 §8 y [insumo-spec-agw-002-objetos-sql.md](agente-gateway/insumo-spec-agw-002-objetos-sql.md).

---

## 5. Handshake de capacidades

Antes de invocar una op nueva, el host debe saber si el agente de **esa** instalación ya la implementa (oleada publicada). Si no:

- no exponer error SQL opaco;
- señal operativa: agente desactualizado respecto al release;
- ops puede priorizar rollout de ese `agent_id`.

Implementación: versión de agente + `opsCatalogHash` o lista de familias en heartbeat / respuesta a `schema.sync`.

---

## 6. Estado y siguientes pasos

| Pieza | Estado |
|-------|--------|
| Dirección release train + flota | Este documento |
| Análisis SQL vs Forge vs binario | [plan-ciclo-sql-y-updates.md](agente-gateway/plan-ciclo-sql-y-updates.md) |
| SPEC formal fase 2 | [SPEC-AGW-002-ciclo-sql-y-updates.md](agente-gateway/SPEC-AGW-002-ciclo-sql-y-updates.md) (placeholder) |
| Orquestador Framework | [34-instalacion-actualizacion-proyecto.md](../../../PaqSuite-IA-FRAMEWORK/docs/02-producto/34-instalacion-actualizacion-proyecto.md) |

Checklist al cerrar un release Tango gateway:

- [ ] Bundle publicado (`agentMin`, `schemaDesired`, `opsCatalogHash`).
- [ ] Artefacto agente en canal (exe + SHA256 o feed pull).
- [ ] Manifiesto SQL alineado con doc 34 §11.
- [ ] Rollout flota (o plan manual documentado para pilotos).
- [ ] `installation.update` ejecutado / verificado en muestra de clientes.
- [ ] Enlaces cruzados actualizados si cambia la ruta de este doc.
