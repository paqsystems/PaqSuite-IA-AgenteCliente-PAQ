# Plan ops — `descarga.paqsystems.com` (Q-D9-1)

| Campo | Valor |
|-------|--------|
| Estado | **Aplicado 2026-10-12.** Instalador público en `https://descarga.paqsystems.com/agentes/PaqAgentSetup.exe` |
| Fecha | 2026-10-12 |
| Cuenta AWS | `655232113361` |
| Host | `descarga.paqsystems.com` |
| Path | `/agentes` |
| Zona DNS | `ZQ4BX7KZVFO0A` (`paqsystems.com.`) |
| Bucket | `paqsystems-descarga-agentes` (`us-east-2`) |
| OAC | `E1QMOQ4BVIZTJS` |
| ACM | `arn:aws:acm:us-east-1:655232113361:certificate/45f38b88-704a-4d2c-a289-75d81102b08f` (Issued) |
| CloudFront | `EZ8KLNGDS514F` → `d2kzl4vawo1zbh.cloudfront.net` |
| Patrón | S3 privado + CloudFront OAC + ACM `us-east-1` + alias Route 53 |
| Público | Sí, **sin login**. El token del tenant **no** va en la URL |

Este archivo es el **plan y el runbook**. No mezclar con el hub SignalR (`gateway.paqsystems.com`).

---

## 1. URLs canónicas (cerradas)

| Rol | URL |
|-----|-----|
| Landing (sin login) | `https://descarga.paqsystems.com/agentes/` |
| Instalador PaqAgent | `https://descarga.paqsystems.com/agentes/PaqAgentSetup.exe` |
| Hash | `https://descarga.paqsystems.com/agentes/PaqAgentSetup.exe.sha256` |

**Por qué `/agentes` y no `/agentePaqSuite`:** paths HTTP en minúsculas; el exe ya se llama `PaqAgentSetup.exe`; mañana puede vivir `…/agentes/otroProductoSetup.exe` sin camelCase ni un host distinto.

**Por qué no Forge `tango-paqsystems.on-forge.com`:** sirve de prueba interna; no es URL de producto, el exe de 80–150 MB no debe salir por PHP del app server, y el hostname Forge no es estable de cara al cliente.

TANGO (Laravel) **puede** más adelante poner un botón que apunte a **estas mismas** URLs. No es un segundo canal. No es requisito para habilitar el host.

---

## 2. Nombres de recursos (usar exactamente estos)

Ajustar solo si el bucket ya existe con otro nombre (S3 es global).

| Recurso | Nombre propuesto |
|---------|------------------|
| Región del bucket | `us-east-2` (Ohio, misma que la EC2 del Gateway) |
| Bucket S3 | `paqsystems-descarga-agentes` |
| Prefijo / “carpeta” | `agentes/` |
| Certificado ACM | `descarga.paqsystems.com` **obligatorio en `us-east-1`** (CloudFront no acepta ACM de otra región) |
| Distribución CloudFront | comentario/tag `paqsystems-descarga-agentes` |
| Registro DNS | A/AAAA alias → distribución CloudFront, nombre `descarga.paqsystems.com` |
| Default root object | `index.html` (opcional; la landing útil es `/agentes/` → objeto `agentes/index.html`) |

Claves S3:

```text
agentes/index.html
agentes/PaqAgentSetup.exe
agentes/PaqAgentSetup.exe.sha256
```

---

## 3. Prerrequisitos

- [x] Cuenta AWS la misma donde ya está la zona `paqsystems.com` (`655232113361`).
- [x] Permisos S3, CloudFront, ACM, Route 53 (aplicado como root).
- [x] MCP AWS autenticado.
- [x] Artefacto publicado: `PaqAgentSetup.exe` (~196 MiB) + `.sha256` (`b039cd6b37658498a77fd0afe9d65a9aea88347c13d27135b3af409db328d9f9`).

---

## 4. Pasos AWS (orden)

### 4.1 Bucket S3 (privado)

1. S3 → Create bucket.
2. Name: `paqsystems-descarga-agentes`.
3. Region: `us-east-2`.
4. **Block all public access: ON** (todo ON). El público entra solo por CloudFront.
5. Versioning: ON (poder volver un exe anterior).
6. Encryption: SSE-S3 (AES-256) alcanza para el MVP.
7. Crear prefijo subiendo un `agentes/.keep` o el `index.html` del §6.

No activar “Static website hosting” en el bucket. El origin de CloudFront es el **REST** del bucket, no el website endpoint.

### 4.2 Certificado ACM (`us-east-1`)

1. Cambiar la consola a **N. Virginia (`us-east-1`)**.
2. ACM → Request public certificate.
3. FQDN: `descarga.paqsystems.com` (sin `*.` salvo que más adelante quieran wildcards).
4. Validation: DNS.
5. Create records in Route 53 (un CNAME `_xxx.descarga.paqsystems.com` en la zona `paqsystems.com.`).
6. Esperar **Issued**. Copiar el ARN del certificado.

Si ACM queda en `us-east-2`, CloudFront no lo lista. Borrar y rehacer en `us-east-1`.

### 4.3 Origin Access Control (OAC)

1. CloudFront → Origin access → Create control setting.
2. Name: `oac-paqsystems-descarga-agentes`.
3. Origin type: S3.
4. Signing: **Sign requests (recommended)** / `always`.

### 4.4 Distribución CloudFront

1. Create distribution.
2. Origin domain: bucket `paqsystems-descarga-agentes.s3.us-east-2.amazonaws.com` (REST, no website).
3. Origin access: **Origin access control** → el OAC del 4.3.
4. Al guardar, CloudFront ofrece **Copy policy**: pegarla en el bucket (paso 4.5). Si no aparece, usar el JSON del 4.5.
5. Viewer protocol: **Redirect HTTP to HTTPS**.
6. Allowed HTTP methods: GET, HEAD (OPTIONS opcional).
7. Cache policy: CachingOptimized (el exe cambia poco; al publicar uno nuevo invalidar, §7).
8. Alternate domain name (CNAME): `descarga.paqsystems.com`.
9. Custom SSL certificate: el ARN de ACM `us-east-1`.
10. Default root object: `index.html` (solo cubre `/`; la landing de producto es `/agentes/`).
11. Price class: North America + Europe alcanza para el piloto AR/US.
12. Tags: `app=paqagent`, `role=descarga`.
13. Create. Anotar **Distribution ID** y **Domain name** (`dxxxxx.cloudfront.net`).
14. Esperar Status **Deployed** (varios minutos).

Opcional (mejor UX en `/agentes/`): CloudFront Function o custom error `403/404` → `/agentes/index.html` con 200. Si no se hace, pedir a los clientes la URL del **exe** o la landing con barra final `/agentes/` y objeto `agentes/index.html`.

### 4.5 Bucket policy (solo CloudFront)

Reemplazar `DISTRIBUTION_ID` y `ACCOUNT_ID`:

```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Sid": "AllowCloudFrontOacRead",
      "Effect": "Allow",
      "Principal": {
        "Service": "cloudfront.amazonaws.com"
      },
      "Action": "s3:GetObject",
      "Resource": "arn:aws:s3:::paqsystems-descarga-agentes/*",
      "Condition": {
        "StringEquals": {
          "AWS:SourceArn": "arn:aws:cloudfront::ACCOUNT_ID:distribution/DISTRIBUTION_ID"
        }
      }
    }
  ]
}
```

No dar `s3:ListBucket` al público. No desactivar Block Public Access.

### 4.6 DNS Route 53

Zona `paqsystems.com.` (la que ya resuelve `gateway.paqsystems.com`):

1. Create record.
2. Record name: `descarga`.
3. Type: **A** — Alias **Yes**.
4. Route traffic to: **Alias to CloudFront distribution** → elegir la del 4.4.
5. (IPv6) AAAA alias al mismo destino, opcional.
6. No crear CNAME a `dxxxxx.cloudfront.net` si ya hay alias A; el alias es el patrón de `gateway` (allí es A a EIP; aquí A alias a CloudFront).

TTL del alias lo maneja Route 53.

### 4.7 Comprobar TLS y DNS

Desde cualquier PC (no hace falta VPN):

```powershell
Resolve-DnsName descarga.paqsystems.com
curl.exe -I https://descarga.paqsystems.com/agentes/index.html
```

Esperado: CNAME/alias hacia CloudFront; HTTP 200 o 403/404 **con certificado válido** de `descarga.paqsystems.com`. 403 antes de subir objetos es normal.

---

## 5. Subir el instalador (cada release)

En este repo:

```powershell
.\scripts\pack-installer.ps1
```

Con AWS CLI (cuando exista en el PC):

```powershell
$exe = "artifacts\PaqAgentSetup.exe"
$hash = "artifacts\PaqAgentSetup.exe.sha256"
aws s3 cp $exe s3://paqsystems-descarga-agentes/agentes/PaqAgentSetup.exe --content-type "application/octet-stream" --region us-east-2
aws s3 cp $hash s3://paqsystems-descarga-agentes/agentes/PaqAgentSetup.exe.sha256 --content-type "text/plain; charset=utf-8" --region us-east-2
aws cloudfront create-invalidation --distribution-id DISTRIBUTION_ID --paths "/agentes/PaqAgentSetup.exe" "/agentes/PaqAgentSetup.exe.sha256" "/agentes/" "/agentes/index.html"
```

Tras invalidar (1–5 min):

```powershell
curl.exe -I https://descarga.paqsystems.com/agentes/PaqAgentSetup.exe
Get-FileHash .\artifacts\PaqAgentSetup.exe -Algorithm SHA256
(Invoke-RestMethod https://descarga.paqsystems.com/agentes/PaqAgentSetup.exe.sha256)
```

El hash local y el publicado tienen que coincidir.

**Prohibido:** commitear el exe; subirlo a git de TANGO; pegar AgentToken en la URL.

---

## 6. Contenido mínimo de `agentes/index.html`

Página estática, sin login, UTF-8. Subir a `s3://paqsystems-descarga-agentes/agentes/index.html` con `Content-Type: text/html; charset=utf-8`.

```html
<!DOCTYPE html>
<html lang="es">
<head>
  <meta charset="utf-8">
  <title>Descarga PaqAgent</title>
</head>
<body>
  <h1>Instalador PaqAgent</h1>
  <p>Un solo archivo. Ejecutar como Administrador. No hace falta cuenta PaqSuite para bajar.</p>
  <p>El token del agente se carga en el asistente después del alta en PaqSuite. No va en esta URL.</p>
  <p><a href="PaqAgentSetup.exe">Descargar PaqAgentSetup.exe</a></p>
  <p>SHA256: ver <a href="PaqAgentSetup.exe.sha256">PaqAgentSetup.exe.sha256</a></p>
  <p>Gateway por defecto del asistente: https://gateway.paqsystems.com/agent-hub</p>
</body>
</html>
```

Actualizar a mano versión/fecha en este HTML en cada release (MVP). Una landing Laravel en TANGO puede esperar.

---

## 7. Invalidación de caché

CloudFront cachea el exe. Cada vez que se pisa `PaqAgentSetup.exe`, invalidar las paths del §5. Si no, el cliente puede bajar el binario viejo hasta el TTL.

---

## 8. Qué no hacer

- Colgar el exe en `gateway.paqsystems.com` (hub WSS).
- Bucket público con Block Public Access OFF (usar OAC).
- Website hosting S3 + CNAME (solo HTTP, sin ACM bien atado).
- ACM en Ohio para CloudFront.
- `tango-paqsystems.on-forge.com` como URL que se manda al cliente.
- Login, query `?token=`, zip o SFX.

---

## 9. Cierre de ticket / evidencia

- [x] 4.1–4.6 aplicados (bucket, ACM Issued, OAC, distribución, policy, alias A/AAAA).
- [x] CloudFront Status **Deployed**.
- [x] [urls-deploy.md](urls-deploy.md) con las URLs de la §1.
- [x] Primera subida de `PaqAgentSetup.exe` + SHA256 vía HTTPS público (`HEAD` 200). Verificar el hash en un Windows de cliente al instalar.

---

## 10. Cómo retomar la aplicación desde este repo

1. Autenticar el MCP AWS en Cursor **o** instalar [AWS CLI v2](https://docs.aws.amazon.com/cli/latest/userguide/getting-started-install.html) y `aws configure`.
2. Pedir: “Ejecutá el plan `docs/06-operacion/descarga-agentes-aws.md`”.
3. El agente crea bucket, OAC, ACM+validación Route 53, distribución, policy, alias `descarga`, sube `index.html` (el exe solo si ya existe `artifacts/PaqAgentSetup.exe`).
