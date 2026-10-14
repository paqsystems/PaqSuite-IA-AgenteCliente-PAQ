# Ficha — Gateway DEV (pre-prod)

| Campo | Valor |
|-------|--------|
| Estado | Hub DEV en el aire 2026-10-14 (TLS + systemd + Forge DEV cableado). Falta agente de test + Vercel |
| Cloudsmith | **El mismo** feed y la **misma** versión que va a producción (sin canal prerelease) |
| Sitio Laravel DEV | `tango-paqsystems-dev.on-forge.com` (Forge site 3419611, rama `develop`, server `paq-2021`) |
| Sitio Laravel prod | `tango-paqsystems.on-forge.com` (Forge site 3419610, rama `main`) — **no tocar** para este ambiente |

---

## 1. Instancia vigente `Paq-Gateway-IA-DEV`

```text
Name tag: Paq-Gateway-IA-DEV
EC2: i-064985897a8c74bb2
Estado: running
AMI: ami-0938c21410d9519c6  Amazon Linux 2023 (kernel 6.1 x86_64)
Tipo: t3.micro
Key pair: pq-ia-gateway   → %USERPROFILE%\.ssh\pq-ia-gateway.pem
Usuario SSH: ec2-user
VPC: vpc-0588b88f9c6772017 (misma que Forge / Paq-Gateway-IA)
Subnet: subnet-0b2e94121d57cadd1  AZ us-east-2b
IP privada: 10.0.1.22
EIP: 16.58.164.132  eipalloc-03d35603804315b3a  (nombre EIP: GateWay)  asociada
SG: sg-0e586a31492354a3f (paq-gateway-ia-dev)
  inbound 80  0.0.0.0/0
  inbound 443 0.0.0.0/0
  inbound 22  181.169.114.175/32  (Oficina PAQ)
  inbound 5100 origen sg-012112202a70d9d29 (Forge)
```

Software en esta EC2 (2026-10-14): .NET 8, Nginx, TLS `gateway-dev.paqsystems.com`, `/etc/paqgateway/env`, systemd `paqgateway`. Uso diario: MANUAL § 4.e / instalacion-agente §9. Esta ficha es **una sola vez**.

Vieja (stopped, ya se puede Terminate): `i-0603b50c1270a3b70` `PAQSUITE-IA-GATEWAY-DEV-DISABLE` en `vpc-bf48bed4`.

DNS previsto: `gateway-dev.paqsystems.com` A → `16.58.164.132`  
Hub agentes: `https://gateway-dev.paqsystems.com/agent-hub`  
Jobs Laravel: `http://10.0.1.22:5100`  
Auth Laravel: `LaravelApi__BaseUrl=https://tango-paqsystems-dev.on-forge.com`

`ASPNETCORE_ENVIRONMENT=Production` y `Gateway__UseDevAuthStub=false` también en DEV.

---

## 2. Lista concreta — qué se toca en cada objeto

Sin Cloudsmith distinto. Mismo paquete; DEV lo prueba, prod lo usa después.

### Route 53 (`paqsystems.com`)

- Registro A `gateway-dev` → `16.58.164.132`
- **No** cambiar `gateway`

### Security Group de `16.58.164.132`

- Inbound 443 `0.0.0.0/0`
- Inbound 80 `0.0.0.0/0`
- Inbound 5100 origen = SG Forge `sg-012112202a70d9d29` (hecho en `sg-0e586a31492354a3f`)
- Inbound 22 IP oficina
- Sin 1433

### EC2 Gateway DEV (`/etc/paqgateway/env`) — secretos

- `ASPNETCORE_ENVIRONMENT=Production`
- `ASPNETCORE_URLS=http://0.0.0.0:5100`
- `Gateway__UseDevAuthStub=false`
- `Gateway__InternalApiKey` = **key DEV nueva** (no la de `Paq-Gateway-IA`)
- `LaravelApi__InternalApiKey` = **key DEV nueva**
- `LaravelApi__BaseUrl=https://tango-paqsystems-dev.on-forge.com`

### Nginx + certbot en esa EC2

- `server_name gateway-dev.paqsystems.com`
- `location /agent-hub` → `127.0.0.1:5100`
- `location /internal` → 404
- Cert `/etc/letsencrypt/live/gateway-dev.paqsystems.com/`

### Binario Gateway

- Mismo publish/Cloudsmith que se va a poner en prod
- Destino `/opt/paqgateway`

### Forge — site **DEV** `tango-paqsystems-dev` (environment)

- `AGENT_GATEWAY_ENABLED=true`
- `AGENT_GATEWAY_URL=http://10.0.1.22:5100`
- `AGENT_GATEWAY_INTERNAL_KEY` = mismo valor que `Gateway__InternalApiKey` del gateway DEV
- Key que Laravel exige en `POST /api/internal/gateway/authenticate` = mismo valor que `LaravelApi__InternalApiKey` del gateway DEV
- `APP_URL` = URL del site DEV
- `APP_KEY` = el de DEV (no copiar prod)
- `CLOUDSMITH_READ_TOKEN` = **el mismo** que prod (lectura del mismo feed)
- Catálogo `empresas_conexion` = **base DEV** (no prod)
- `php artisan empresas-conexion:alta-agente …` **en este site**

### Forge — site **prod** `tango-paqsystems`

- Nada

### Vercel — proyecto / env **Preview o DEV**

- `VITE_API_URL` = `https://tango-paqsystems-dev.on-forge.com/api`
- Tenant de prueba vía host/header (`X-Paq-Cliente` / subdominio), no el de prod
- **No** apuntar Preview a `tango-paqsystems.on-forge.com`

### Vercel — Production

- Nada

### Cloudsmith

- Nada (mismo token, mismo paquete, misma versión)

### S3 / `descarga.paqsystems.com`

- Nada (mismo `PaqAgentSetup.exe` si el binario del agente es el de esa versión)

### Agente Windows de test (una instalación)

- `appsettings.local.json` → `gatewayUrl`: `https://gateway-dev.paqsystems.com/agent-hub`
- `agentId` / `clientId` / `agentToken`: los impresos por artisan **en Forge DEV**
- SQL: diccionario de la máquina de test
- Servicio `PaqAgent` (no convive con hub prod en el mismo JSON)

### Agentes de clientes reales

- Nada (`gateway.paqsystems.com` intacto)

### Gateway prod `Paq-Gateway-IA`

- Nada (`LaravelApi__BaseUrl` sigue `http://10.0.1.147` / site prod)

---

## 3. Pares que tienen que coincidir

| Lado A | Lado B | Valor |
|--------|--------|--------|
| Forge DEV `AGENT_GATEWAY_URL` | `http://10.0.1.22:5100` | jobs |
| Forge DEV `AGENT_GATEWAY_INTERNAL_KEY` | `Gateway__InternalApiKey` | Laravel → Gateway |
| `LaravelApi__InternalApiKey` | key authenticate en Laravel DEV | Gateway → Laravel |
| `LaravelApi__BaseUrl` | `https://tango-paqsystems-dev.on-forge.com` | authenticate |
| Agente `gatewayUrl` | `https://gateway-dev.paqsystems.com/agent-hub` | WSS |
| Agente token | fila `empresas_conexion` del **MySQL DEV** | auth |

---

## 4. Qué no cruzar

- `AGENT_GATEWAY_URL` de DEV → `10.0.1.224`
- Agente de test → `https://gateway.paqsystems.com/agent-hub`
- Copiar fila `empresas_conexion` prod ↔ DEV
- Keys de `/etc/paqgateway/env` de `Paq-Gateway-IA` en la instancia `16.58.164.132`
- `UseDevAuthStub=true` en esta EC2
