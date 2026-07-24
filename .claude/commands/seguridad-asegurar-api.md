---
description: Aplica autenticación/autorización JWT propia (BCrypt + roles Admin/Cliente) a Perfumeria.Api y asegura el endpoint del webhook de la pasarela de pago con validación de firma.
allowed-tools: Read, Write, Edit, Bash, Grep, Glob
---

# 🔐 Comando — Asegurar el API (Perfumeria.Api)

Eres un experto en **ASP.NET Core 8** y seguridad de APIs. Tu tarea es agregar autenticación y
autorización al proyecto `Perfumeria.Api`, respetando la arquitectura de 5 capas (`ADR-003`).

> ⚠️ **Delegación obligatoria:** este comando **MUST** delegarse al subagente
> `seguridad-programador-api` (vía Task tool) en vez de ejecutarse directamente en el contexto
> principal. Ver `.claude/agents/seguridad-programador-api.md`.

## 🎯 Objetivo

Proteger los endpoints con **JWT propio** (sin servicio externo) y **`[Authorize(Roles)]`**, y
asegurar `POST /api/pago/webhook` contra payloads no auténticos.

## ⛔ Restricciones (obligatorias)

- **NO** depender de un servicio de seguridad externo — el JWT se emite y valida dentro del mismo
  API (`ADR-003`).
- **NO** hardcodear `Jwt:Key` ni la secret key de la pasarela — leer de configuración/variables de
  entorno.
- Mantener la arquitectura de 5 capas (Abstracciones, API, Flujo, Servicios, DA).

## ✅ Pasos que debes ejecutar

1. **Instalar dependencias:** `Microsoft.AspNetCore.Authentication.JwtBearer` (8.0.*),
   `BCrypt.Net-Next` (en `Servicios`).

2. **`appsettings.json`:** sección `Jwt` (`Key` ≥ 32 chars, `Issuer`, `Audience`, `ExpiraHoras`) y
   `ConnectionStrings:DefaultConnection`. Valores reales solo en configuración no versionada.

3. **`Program.cs`:**
   - `AddAuthentication(JwtBearerDefaults).AddJwtBearer(...)` con `TokenValidationParameters`
     validando Issuer, Audience, Lifetime y SigningKey.
   - CORS explícito para los orígenes de `perfumeria-admin` y `perfumeria-tienda` (nunca
     `AllowAnyOrigin()` combinado con credenciales).
   - `app.UseAuthentication(); app.UseAuthorization();` en ese orden.

4. **Proteger los Controllers:** `[Authorize]` a nivel de clase por defecto.
   `[AllowAnonymous]` solo en: endpoints de catálogo (GET de productos/marcas), 
   `POST /api/auth/registro-cliente`, `POST /api/auth/login`, y `POST /api/pago/webhook` (con
   verificación de firma en su lugar, ver paso 5). Usar `[Authorize(Roles = "Admin")]` o
   `[Authorize(Roles = "Cliente")]` según `docs/vision.md` §8.

5. **Asegurar el webhook de pago:** implementar la verificación de firma/token que el proveedor
   (ONVO Pay o Tilopay, `ADR-004`) especifica en su documentación, **antes** de tocar
   `Pedido.Estado` o `StockVirtual`. Si la firma no es válida, responder rechazo sin efectos
   secundarios.

6. **Verificar:** sin token = `401`; token con rol incorrecto = `403`; token correcto = `200`; webhook
   con firma inválida = rechazado sin cambios en BD; webhook reenviado (mismo
   `IdTransaccionPasarela`) = `200` sin duplicar el descuento de stock.

## 📤 Salida esperada

- Diff de `Program.cs`, `appsettings.json` (con placeholders seguros), Controllers modificados y el
  handler del webhook.
- Explicación breve de por qué el webhook nunca lleva `[Authorize]` estándar pero sí validación de
  firma.

## 🧪 Criterios de aceptación

- [ ] El API compila y protege todos los endpoints salvo los explícitamente públicos.
- [ ] Endpoints devuelven 401/403/200 según corresponda.
- [ ] El webhook rechaza payloads sin firma válida.
- [ ] El webhook es idempotente ante reintentos.
- [ ] No hay secretos hardcodeados en el repositorio.
