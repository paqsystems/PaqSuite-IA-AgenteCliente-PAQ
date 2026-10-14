# URLs de deploy — agente-gateway

| Rol | URL |
|-----|-----|
| Gateway producción (hub SignalR) | https://gateway.paqsystems.com/agent-hub |
| Gateway DEV (hub SignalR) | https://gateway-dev.paqsystems.com/agent-hub |
| Gateway DEV interno (Laravel jobs) | `http://10.0.1.22:5100` (instancia *Paq-Gateway-IA-DEV*) |
| Gateway lab | http://127.0.0.1:5100/agent-hub |
| Gateway interno (Laravel → jobs/status) | `http://10.0.1.224:5100` (instancia *Paq-Gateway-IA*; ver ficha) |
| Descarga instalador (MVP, **público**, D9 / Q-D9-1 **cerrada**) | `https://descarga.paqsystems.com/agentes/` |
| Asset instalador | `https://descarga.paqsystems.com/agentes/PaqAgentSetup.exe` (+ `.sha256` al lado) |

Notas:

- Hostname ops real (*2026-09-05*): **gateway.paqsystems.com** (zona Route 53 `paqsystems.com`). El SPEC histórico decía `gateway.paqsuite.com` (dominio no presente en la cuenta).
- El instalador se descarga de **`https://descarga.paqsystems.com/agentes/`** (S3 + CloudFront, sin login). No clonar código ni descomprimir zip. SHA256 (D9). Empaquetado: [empaquetado-instalador.md](empaquetado-instalador.md). Alta del host: [descarga-agentes-aws.md](descarga-agentes-aws.md). Justificación: [MANUAL-DEL-PROGRAMADOR](../00-contexto/MANUAL-DEL-PROGRAMADOR.md) § D9.
- **Q-D9-1 cerrada 2026-10-12:** host `descarga.paqsystems.com`, path `/agentes`. Un botón futuro en TANGO apunta a **la misma** URL. No reabrir el formato del artefacto (`PaqAgentSetup.exe`).
- GitHub Releases no es el canal de cara al cliente (puede usarse como espejo interno de CI).
- Instructivo cliente: [instalacion-agente.md](instalacion-agente.md).
- Laravel habla al Gateway por URL **interna** VPC (no Tailscale ni el hostname público para `/internal/*`).
- El hub público solo recibe WSS saliente de agentes (443); Nginx no publica `/internal`.
- Runbook AWS: [deploy-gateway-aws.md](deploy-gateway-aws.md). **Instalación exhaustiva (referencia):** [deploy/instalacion-exhaustiva-paq-gateway-ia.md](deploy/instalacion-exhaustiva-paq-gateway-ia.md). Plantillas: [deploy/](deploy/).
- Lab por tramos: [lab-local.md](lab-local.md).
- **No** hay URL Tailscale ni fallback modo agente en este archivo.
