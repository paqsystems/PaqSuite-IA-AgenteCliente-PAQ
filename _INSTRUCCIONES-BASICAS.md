# Cómo instalar agente y gateway xx

Ver `docs\06-operacion\README.md`

en Resumen : 
- Agente : `docs/06-operacion/instalacion-agente.md`
    Descarga del instalador (alta en servidor cliente): `https://descarga.paqsystems.com/agentes/PaqAgentSetup.exe` (landing `https://descarga.paqsystems.com/agentes/`). Build, SHA256 y subida a S3: `docs/06-operacion/empaquetado-instalador.md`.
    El paso a paso para instalar, con obtención de token está en MANUAL-DEL-PROGRAMADOR.md (ver abajo), capítulo `## 4. Instalación de un cliente — orden de ejecución`
- Gateway 
    (corto) :`docs/06-operacion/deploy-gateway-aws.md`
    (exhaustivo) : `docs/06-operacion/deploy/instalacion-exhaustiva-paq-gateway-ia.md`
- Ambiente DEV (pre-prod, un solo agente de test): MANUAL § 4.e · instalacion-agente.md §9
    Ficha de la EC2 (una sola vez): `docs/06-operacion/ficha-gateway-dev.md`

# Reinstalación Agente (manual)

1) Powershell como Administrador

cd C:\Programacion\PaqSuite-IA-AgenteCliente-PAQ

# 1) Compilar el agente nuevo
dotnet publish src\PaqAgent\PaqAgent.csproj -c Release -r win-x64 --self-contained true `
  -p:PublishSingleFile=true -o artifacts\paqagent-update

# 2) Parar el servicio (si el exe queda trabado, matarlo)
Stop-Service PaqAgent -Force
Get-Process PaqAgent -ErrorAction SilentlyContinue | Stop-Process -Force

# 3) Copiar binarios SIN tocar la config
$dest = "C:\PaqSystems\PaqAgent"
Copy-Item artifacts\paqagent-update\* $dest -Recurse -Force -Exclude "appsettings.local.json"

# 4) Arrancar
Start-Service PaqAgent
Get-Service PaqAgent

2) Comprobación de que volvió a conectar

Get-Content C:\PaqSystems\PaqAgent\appsettings.local.json
Get-ChildItem C:\PaqSystems\PaqAgent\logs | Sort-Object LastWriteTime -Descending | Select-Object -First 1 | Get-Content -Tail 40


# Manual del Programador

Ejecución scripts :  

`C:\Programacion\PaqSuite-IA-FRAMEWORK\packages\php\laravel-core\database\sp\pq_sp_grid_layout_core.sql`

`C:\Programacion\PaqSuite-IA-FRAMEWORK\packages\php\laravel-core\database\sp\pq_grid_layouts_unique_q5.sql`


# Ejecuciones SQL 

ver `docs\00-contexto\MANUAL-DEL-PROGRAMADOR.md`

# Cómo hacer pruebas smoke

1) Gateway
cd C:\Programacion\PaqSuite-IA-AgenteCliente-PAQ
dotnet run --project src\PaqGateway --urls http://127.0.0.1:5100
Firefox: http://127.0.0.1:5100 → 404 OK (conecta).

2) Agente
a) Cambiar el appsettings.local.json
 `"gatewayUrl": "https://gateway.paqsystems.com/agent-hub",`
  por
 `"gatewayUrl": "http://127.0.0.1:5100/agent-hub",`
b) Servicio/app del agente en marcha, con el mismo agent_id / token que la fila lenovo en 192.168.41.2 / PAQSYSTEMS.

3) BE Tango (preparacion)
Cambiar en backend/.env (en lab) o en Environment de Forge
a) AGENT_GATEWAY_ENABLED=true
b) cambiar la variable : 
 `AGENT_GATEWAY_URL=http://10.0.1.224:5100`
 por
 `AGENT_GATEWAY_URL=http://127.0.0.1:5100`
c) la BD de tenants/empresas_conexion donde está lenovo con agent_id.

4) FE Tango (Preparación)
En frontend/.env:
VITE_API_URL=http://127.0.0.1:8000/api → npm run dev.
VITE_TENANT_LOCAL=lenovo

5) BE Tango (Ejecución)
cd C:\Programacion\PaqSuite-IA-TANGO\backend
php artisan serve --host=127.0.0.1 --port=8000
Probar: http://127.0.0.1:8000/api/v1/health → debe responder ya (si cuelga, no sigas al login).

6) FE Tango (ejecución)

- si ya estaba levantado y hay que bajarlo:
    Get-NetTCPConnection -LocalPort 3000 -ErrorAction SilentlyContinue | ForEach-Object { Stop-Process -Id $_.OwningProcess -Force }

cd C:\Programacion\PaqSuite-IA-TANGO\frontend
npm install            
npm run dev

(npm install solo si aún no instalaste deps.)

7) Login (ventana privada)
http://localhost:3000/?cliente=lenovo
Network: POST a http://127.0.0.1:8000/api/v1/auth/login
Terminal artisan: tiene que aparecer ese POST.

Si algo falla, pará en el primer paso que no cumpla (health / 5100 / POST en artisan) y lo vemos.


# Cómo pasar a versión productiva

1) Gateway
No hay que hacer nada. ya está activo en AWS

2) Agente
Cambiar el appsettings.local.json
 `"gatewayUrl": "http://127.0.0.1:5100/agent-hub",`
 por
 `"gatewayUrl": "https://gateway.paqsystems.com/agent-hub",`
 
 3) Host
 Cambiar en backend/.env (en lab) o en Environment de Forge
 `AGENT_GATEWAY_URL=http://127.0.0.1:5100`
 por
 `AGENT_GATEWAY_URL=http://10.0.1.224:5100`

# Ambiente DEV (pre-prod)

No es lab (`127.0.0.1`). Es AWS real, separado de prod. Mismo Cloudsmith / mismo exe que va a producción.

## Distinciones

| Pieza | Prod | DEV |
|-------|------|-----|
| Forge | `tango-paqsystems.on-forge.com` | `tango-paqsystems-dev.on-forge.com` |
| Hub agente `gatewayUrl` | `https://gateway.paqsystems.com/agent-hub` | `https://gateway-dev.paqsystems.com/agent-hub` |
| Forge `AGENT_GATEWAY_URL` | `http://10.0.1.224:5100` | `http://10.0.1.22:5100` |
| Vercel `VITE_API_URL` | API prod | `https://tango-paqsystems-dev.on-forge.com/api` |
| SSH Gateway | `ec2-user@3.142.236.237` | `ec2-user@16.58.164.132` |

## Token

```bash
cd /home/forge/tango-paqsystems-dev.on-forge.com/current/backend
php8.1 artisan empresas-conexion:alta-agente <slug> "<Nombre visible>"
```

Esos tres valores van al agente de **test**. No copiar `empresas_conexion` desde prod.

## Agente (máquina de test)

En `C:\PaqSystems\PaqAgent\appsettings.local.json`:

```json
"gatewayUrl": "https://gateway-dev.paqsystems.com/agent-hub"
```

Restart-Service PaqAgent. Online se mira en PaqSuite **DEV**.

## Gateway DEV

Ya está en el aire (`Paq-Gateway-IA-DEV`). Recetario de software = el de prod (§3 del MANUAL) con hostname `gateway-dev.paqsystems.com`. Crear/recrear la EC2 es **una sola vez**, una sola instalación: `docs/06-operacion/ficha-gateway-dev.md`.

Keys: `C:\Programacion\KEYS\paq-gateway-ia-dev\` (no las de prod).

```powershell
ssh -i "$env:USERPROFILE\.ssh\pq-ia-gateway.pem" ec2-user@16.58.164.132
```

No cruzar DEV con `10.0.1.224` ni con `gateway.paqsystems.com`.

# Cómo entrar por SSH al gateway AWS (producción)

Desde PowerShell (prod `Paq-Gateway-IA`). DEV: ver tópico Ambiente DEV arriba.

```powershell
ssh -i "$env:USERPROFILE\.ssh\pq-ia-gateway.pem" ec2-user@3.142.236.237
```

# Cómo obtener el valor del agent_token

SSH paso a paso (IP del Forge, `id_ed25519`, prompt `forge@paq-2021`): [MANUAL-DEL-PROGRAMADOR](../00-contexto/MANUAL-DEL-PROGRAMADOR.md), apartado **«Paso a paso (para Legos)»** dentro de § 4.a.

# Cómo limpiar la variable ´sessionStorage'

En Firefox:

## Alternativa 1
- Presiona F12 para abrir las herramientas de desarrollador.
- Ve a Almacenamiento.
- Abre Almacenamiento de sesión → http://127.0.0.1:5173.
- Busca paq_cliente.
- Clic derecho → Eliminar.
- Recarga la página con Ctrl + F5.

## Alternativa 2
También puedes hacerlo desde la consola:
- sessionStorage.removeItem('paq_cliente');
- location.reload();