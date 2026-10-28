# TR-GEN-34-gateway-installation-ops — Handlers agente (cruzada Framework)

| Campo | Valor |
|-------|--------|
| **Framework** | [TR-GEN-34-gateway-agente](https://github.com/paqsystems/PaqSuite-IA-FRAMEWORK/blob/develop/docs/04-tareas/001-Generalidades/TR-GEN-34-gateway-agente.md) |
| **Repo** | PaqSuite-IA-AgenteCliente-PAQ |
| **Estado** | **Pendiente de Revisión** |

## Ops a implementar en el agente

| Op | Contrato host (laravel-core) |
|----|------------------------------|
| `installation.install` | Payload `step` + `artifact` + `packChecksum` |
| `installation.update` | Idem delta |
| `company.provision` | `mode`, `erpEmpresaId`, … |

## Referencias

- `docs/02-producto/actualizacion-agentes-release-train.md`
- SPEC-AGW-001 (conectividad MVP)

## Config agente (MUST lab/prod)

En `appsettings.local.json`:

```json
"installation": {
  "packRoot": "C:\\\\PaqSuite\\\\release-train\\\\paqsuite-sdk"
}
```

El host valida checksum en su `packageRoot`; el agente debe tener el **mismo árbol relativo** bajo `packRoot`.

## Alcance T3

| Op | Comportamiento |
|----|----------------|
| `installation.install` / `installation.update` | Aplica un paso `kind=sql` en `dictionary` o `company` (filtro `companyIds`); registra en `paq_sp_migrations`. |
| `company.provision` | `initial`: devuelve primera fila `pq_empresa`. `additional`: `PROVISION_ADDITIONAL_NOT_SUPPORTED` (SP alta ERP pendiente). |
