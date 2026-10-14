# Instalación del agente (cliente)

| Campo | Valor |
|-------|--------|
| Estado | Instructivo **final** (HU-008 / TR-009) |
| Público objetivo | Administrador del servidor SQL del cliente |
| Descarga | `https://descarga.paqsystems.com/agentes/` — [urls-deploy.md](urls-deploy.md) (D9; Q-D9-1 cerrada) |
| Empaquetado (build) | [empaquetado-instalador.md](empaquetado-instalador.md) |
| Gateway (operador) | [deploy-gateway-aws.md](deploy-gateway-aws.md) |
| Ambiente DEV | §9 abajo · ficha única [ficha-gateway-dev.md](ficha-gateway-dev.md) |
| Lab desarrollo | [lab-local.md](lab-local.md) |
| Alta / token | [MANUAL-DEL-PROGRAMADOR](../00-contexto/MANUAL-DEL-PROGRAMADOR.md) § 4.a (resumen al pie, §8) |

**Prohibido:** Tailscale; IP pública del SQL hacia Internet; abrir 1433 a Internet; `dev-agent-token`; editar `appsettings.local.json` a mano en producción; fallback SQL modo agente.

---

## Checklist de alta de cliente (10 pasos)

Copiable para el operador PaqSystems + admin del servidor:

1. Alta modo agente en Laravel (TANGO) → obtener **AgentId**, **ClientId**, **AgentToken** (el token se muestra **una sola vez**). Cómo: **§8** y fuente [MANUAL § 4.a](../00-contexto/MANUAL-DEL-PROGRAMADOR.md).
2. Confirmar Gateway de producción reachable: `https://gateway.paqsystems.com/agent-hub`.
3. Entregar al admin del servidor: los tres valores + esta guía + URL de descarga.
4. En el servidor Windows del cliente: instalar **.NET 8 Desktop Runtime x64** si el instalador lo pide (o ya está).
5. Descargar **`PaqAgentSetup.exe`** desde `https://descarga.paqsystems.com/agentes/`, verificar **SHA256**, ejecutar (no descomprimir).
6. Ejecutar `PaqAgentSetup.exe` **como Administrador**.
7. Completar el asistente: runtime → credenciales (identidad + SQL + Gateway) → Probar SQL → Probar Gateway → Instalar.
8. Verificar servicio Windows **`PaqAgent`** en estado **Running** (inicio automático).
9. Confirmar en PaqSuite que el agente aparece **online** (heartbeat; TTL ~90 s).
10. Guardar evidencias (captura servicio / online) y archivar el token fuera del ticket público.

---

## 1. Dónde descargar

| Ítem | Valor |
|------|-------|
| Canal | Landing **pública** en `descarga.paqsystems.com`, sin login (D9) |
| URL canónica | `https://descarga.paqsystems.com/agentes/` (Q-D9-1 cerrada; [urls-deploy.md](urls-deploy.md); alta AWS: [descarga-agentes-aws.md](descarga-agentes-aws.md)) |
| Asset | `PaqAgentSetup.exe` (un solo exe autoejecutable; PaqAgent va embebido) |
| Integridad | SHA256 en la misma página o archivo `PaqAgentSetup.exe.sha256` (D9) |
| Gateway URL por defecto en el instalador | `https://gateway.paqsystems.com/agent-hub` (editable) |
| Carpeta de instalación por defecto | `C:\PaqSystems\PaqAgent` |

Cada servidor nuevo se instala desde esa URL; **no** hace falta clonar el código, Visual Studio ni descomprimir un zip.

Verificar SHA256 (PowerShell):

```powershell
Get-FileHash .\PaqAgentSetup.exe -Algorithm SHA256
# Comparar con el hash publicado en la landing TANGO
```

Si el host o el path de la landing cambian, actualizar solo [urls-deploy.md](urls-deploy.md), D9 y esta sección. El formato del artefacto (un exe) no se reabre.

Justificación (GitHub/zip/SFX): [MANUAL-DEL-PROGRAMADOR](../00-contexto/MANUAL-DEL-PROGRAMADOR.md) § D9.

---

## 2. Prerrequisitos

| Requisito | Notas |
|-----------|--------|
| Windows x64, cuenta **Administrador** | El instalador pide elevación (servicio Windows) |
| .NET 8 **Desktop** Runtime x64 | El paso 0 del asistente lo detecta; descarga Microsoft: https://dotnet.microsoft.com/download/dotnet/8.0 |
| Salida HTTPS/443 hacia el Gateway | DNS/TLS; sin VPN Tailscale |
| SQL Server local alcanzable | Credenciales de solo lo necesario para el diccionario |
| Datos del alta Laravel | AgentId, ClientId, AgentToken |

El paquete del instalador es **self-contained**; igual conviene tener Desktop Runtime por herramientas y por el aviso del paso 0.

---

## 3. De dónde sale cada dato

| Campo en el instalador | Origen |
|-------------------------|--------|
| AgentId | Alta Laravel (HU-001); comando en **§8** |
| ClientId | Alta Laravel; mismo comando |
| AgentToken | Alta Laravel (**una vez**; password-char; sin default). Obtención: **§8** |
| Gateway URL | Default de fábrica prod; **DEV:** `https://gateway-dev.paqsystems.com/agent-hub` (§9); lab: `http://127.0.0.1:5100/agent-hub` |
| Servidor / base / usuario / contraseña SQL | Admin del servidor (SQL local del cliente) |
| Puerto SQL | Opcional (vacío = 1433); si el servidor es instancia con `\`, no hace falta puerto |
| encrypt / trustServerCertificate | **Copiar SSMS** (ver §3.1). No usar el default del asistente a ciegas |
| Dir. instalación | Default `C:\PaqSystems\PaqAgent` |

### 3.1 encrypt y trustServerCertificate (según SSMS)

Los dos tildes del instalador **no se inventan**. Se copian de la conexión que ya funciona en SSMS (o Azure Data Studio) contra **ese** SQL:

| En SSMS | En el instalador |
|---------|------------------|
| Cifrar = **Opcional** (o deshabilitado) | **encrypt** desmarcado |
| Cifrar = **Obligatorio** / Strict | **encrypt** marcado |
| Certificado de servidor de confianza = **sin tilde** | **trustServerCertificate** desmarcado |
| Certificado de servidor de confianza = **con tilde** | **trustServerCertificate** marcado |

Si “Probar SQL” falla con handshake SSL / `SQL_UNREACHABLE` y SSMS entra con Cifrar **Opcional**: desmarcar encrypt. `trustServerCertificate` solo importa cuando encrypt está marcado (certificado de fábrica / autofirmado).

En la base del instalador poner el **nombre exacto del diccionario**, no “predeterminado”.

---

## 4. Pasos del asistente

1. **Runtime** — Si falta Desktop 8, aviso claro (+ posible reinicio) y opción de descargar; con runtime OK, continuar.
2. **Credenciales** — Completar identidad, SQL y Gateway; token obligatorio.
3. **Pruebas** — **Probar SQL** (obligatorio OK). **Probar Gateway** (obligatorio OK, salvo override avanzado desmarcado por defecto).
4. **Instalar** — Copia binarios, escribe `appsettings.local.json`, crea servicio **`PaqAgent`** (`start=auto`) solo si las pruebas pasaron.
5. **Resultado** — Debe indicar servicio **Running** y “esperando online en PaqSuite”.

---

## 5. Cómo verificar

```powershell
Get-Service PaqAgent
# Status = Running, StartType = Automatic

Get-Content C:\PaqSystems\PaqAgent\appsettings.local.json
# Debe contener agentId / clientId / gatewayUrl (no compartir el token)

Get-WinEvent -LogName Application -MaxEvents 20 |
  Where-Object { $_.ProviderName -like '*Paq*' -or $_.Message -like '*PaqAgent*' }
# Alternativa: revisar logs junto al binario si el agente escribe archivos de log
```

En PaqSuite: el cliente/agente debe figurar **online** tras el heartbeat (TTL de online ~90 s según SPEC).

---

## 6. Qué no configurar

- Tailscale (ni como “requisito de red”).
- IP pública del servidor SQL ni port-forward de **1433** a Internet.
- Token de desarrollo / `dev-agent-token`.
- Editar JSON a mano en producción (solo lab).
- “Fallback” a SQL directo si el agente está offline (modo agente = error claro).

---

## 7. Troubleshooting mínimo

| Síntoma | Qué mirar |
|---------|-----------|
| Instalador no abre / UAC | Ejecutar como Administrador |
| Paso 0: falta runtime | Instalar Desktop 8 x64; reinicio posible; volver a detectar |
| Probar SQL falla | Servidor/base/usuario/clave; firewall local; puerto; **encrypt/trust copiados de SSMS** (§3.1) |
| Probar Gateway falla | DNS, TLS, salida **443**; URL correcta; no usar IP Tailscale |
| No crea el servicio | No avanzar sin SQL OK (y Gateway OK u override); elevación admin |
| Servicio no parte | `Get-Service PaqAgent`; Event Viewer; `appsettings.local.json` junto al exe |
| Agente no online | Gateway URL **del mismo ambiente** que el artisan (prod vs DEV §9); token válido del alta; 443 saliente; esperar TTL heartbeat |

---

## Relación con el alta en Laravel

1. Operador PaqSystems da de alta el tenant modo agente y obtiene AgentId / ClientId / AgentToken (**§8**).
2. Entrega esos datos + link de descarga al administrador del servidor.
3. El administrador descarga, verifica SHA256, instala y deja el servicio corriendo.
4. PaqSuite debe ver el agente **online**.

Sin alta previa no hay token válido para el instalador.

---

## 8. Cómo obtener AgentId, ClientId y AgentToken

Fuente canónica (detalle, cifrado, rotación, troubleshooting de `artisan`): [MANUAL-DEL-PROGRAMADOR](../00-contexto/MANUAL-DEL-PROGRAMADOR.md) **§ 4.a**. Lo de abajo es el recorte operativo para este instructivo. Si hay conflicto, manda el manual.

El comando se corre en **TANGO / Forge** (sitio Laravel), **no** en la EC2 del Gateway ni en el Windows del cliente. Repo `PaqSuite-IA-TANGO`, rama **`FRAMEWORK`** deployada. Con SSH `forge@…` **no uses `sudo`**. Usá **`php8.1 artisan`** (el `php` del PATH es 7.4 y falla). Sitio verificado: **dev**.

```bash
cd /home/forge/tango-paqsystems-dev.on-forge.com/current/backend
php8.1 artisan list --raw | grep alta-agente
php8.1 artisan empresas-conexion:alta-agente <slug> "<Nombre visible>"
```

Ejemplo: `php artisan empresas-conexion:alta-agente tecser "Tec-Ser"`.

Solo dos argumentos: `cliente` (slug = `X-Paq-Cliente` / subdominio) y `nombre` (etiqueta visible). El comando:

1. Normaliza el slug; `client_id` = slug; `agent_id` = `{client_id}-agent-01` (un agente por tenant).
2. Genera un token de 64 hex. **Prohibido** `dev-agent-token`. Un INSERT SQL a mano en claro **no alcanza**.
3. INSERT o UPDATE en `empresas_conexion` (`activo=true`; sin `host`/`port` SQL remoto en modo agente).
4. Imprime **`agent_id`**, **`client_id`** y el **token en claro una sola vez**. En BD queda cifrado.

Anotar los tres valores en un lugar seguro (no en un ticket público). El token no se vuelve a imprimir. Esos son los campos AgentId / ClientId / AgentToken del instalador.

Si ya había agente y hay que rotar (el token anterior deja de servir):

```bash
php8.1 artisan empresas-conexion:alta-agente <slug> "<Nombre visible>" --force-replace
```

Si `grep alta-agente` no lista el comando: esa rama **no está deployada**. Ver el manual § 4.a (CD `FRAMEWORK` y, si hace falta, migrate del catálogo).

SSH paso a paso (IP del Forge, `id_ed25519`, prompt `forge@paq-2021`): mismo manual, apartado **«Paso a paso (para Legos)»** dentro de § 4.a.

---

## 9. Ambiente DEV (pre-prod)

Mismo instalador, mismo exe, mismo Cloudsmith que producción. Cambia el **circuito**: Forge DEV + Gateway DEV + **una** máquina de test. No es lab (`127.0.0.1`).

| Qué | Producción | DEV |
|-----|------------|-----|
| Artisan (`alta-agente`) | `tango-paqsystems.on-forge.com` | `tango-paqsystems-dev.on-forge.com` |
| Gateway URL en el asistente / `appsettings.local.json` | `https://gateway.paqsystems.com/agent-hub` | `https://gateway-dev.paqsystems.com/agent-hub` |
| Laravel jobs | `http://10.0.1.224:5100` | `http://10.0.1.22:5100` |
| Dónde debe verse **online** | PaqSuite prod | PaqSuite DEV (Vercel Preview → API DEV) |

```bash
cd /home/forge/tango-paqsystems-dev.on-forge.com/current/backend
php8.1 artisan empresas-conexion:alta-agente <slug> "<Nombre visible>"
```

En el JSON del agente (si ya estaba instalado, no hace falta re-ejecutar el setup):

```json
"gatewayUrl": "https://gateway-dev.paqsystems.com/agent-hub"
```

`agentId` / `clientId` / `agentToken` = los que imprimió el artisan **en DEV**. Después `Restart-Service PaqAgent`.

No usar el token de prod en DEV ni el hub `gateway.paqsystems.com` en la máquina de test.

Detalle de variables Forge/Vercel/Gateway: [MANUAL § 4.e](../00-contexto/MANUAL-DEL-PROGRAMADOR.md#4e-ambiente-dev-pre-prod).

La EC2 `Paq-Gateway-IA-DEV` se crea **una sola vez** (una instalación). IDs, EIP, SG, recetario: [ficha-gateway-dev.md](ficha-gateway-dev.md).
