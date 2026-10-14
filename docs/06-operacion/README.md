# 06 — Operación

Runbooks y lab. No sustituyen el SPEC.

| Archivo | Para qué |
|---------|----------|
| [lab-local.md](lab-local.md) | Verificar Agente ↔ Gateway **por tramos** sin Tailscale, AWS ni instalador |
| [urls-deploy.md](urls-deploy.md) | Hub lab + prod + landing instalador (`descarga.paqsystems.com/agentes`) |
| [ficha-gateway-dev.md](ficha-gateway-dev.md) | Ambiente DEV (segunda EC2, Forge `*-dev`, lista de variables) |
| [instalacion-agente.md](instalacion-agente.md) | Instructivo cliente (HU-008) — un `PaqAgentSetup.exe` público |
| [descarga-agentes-aws.md](descarga-agentes-aws.md) | Plan/runbook S3 + CloudFront + DNS `descarga.paqsystems.com` |
| [deploy-gateway-aws.md](deploy-gateway-aws.md) | Instructivo operador AWS (TR-003/009) |
| [empaquetado-instalador.md](empaquetado-instalador.md) | Publish `PaqAgentSetup.exe` + SHA256 (build) |
| [../00-contexto/MANUAL-DEL-PROGRAMADOR.md](../00-contexto/MANUAL-DEL-PROGRAMADOR.md) | Manual del programador (concepto, código, Gateway EC2, alta cliente, D9; futuro update/SQL) |

Orden práctico: **lab-local** primero; AWS e instalador después (D10).
