---
adr_id: ADR-003
title: Arquitectura backend de 5 capas sin capa Reglas separada, JWT propio con BCrypt
status: aceptado
date: 2026-07-21
authors: [Camila]
reviewers: [Por definir]
tags: [arquitectura, backend, seguridad]
supersedes: []
superseded_by: []
---

# ADR-003 · Arquitectura backend de 5 capas + JWT propio

## 1. Estado

`aceptado`.

---

## 2. Contexto

El material de gobernanza genérico del taller (`Template.API`) prescribe **6 capas** en el backend:
`Abstracciones, API, Flujo, Reglas, Servicios, AccesoDatos` — con `Reglas` como capa separada y pura
(sin I/O) para las reglas de negocio. También prescribe consumir un servicio de seguridad externo
(`Seguridad.API` + middleware NuGet `Autorizacion.*`) para emitir y validar JWT.

El proyecto de referencia estructural, `PuraVidaFragance`, usa en cambio **5 capas**
(`Abstracciones, API, DA, Flujo, Servicios` — sin `Reglas` separada) y un **JWT autocontenido**:
el propio API emite y valida sus tokens, usando `BCrypt` para hash de contraseñas y el rol como
claim de texto simple (`"Admin"` / `"Cliente"`), sin depender de un servicio de seguridad externo ni
de un paquete NuGet privado.

Para un proyecto de alcance académico y equipo pequeño, depender de un servicio de seguridad externo
y un feed de paquetes privado agrega complejidad operativa (dos APIs para levantar, sincronizar
`Issuer`/`Audience`/`Key` entre ambas, gestionar un feed de GitHub Packages) sin un beneficio claro
a esta escala.

---

## 3. Decisión

Se adopta la arquitectura de **5 capas sin `Reglas` separada** y **JWT propio con BCrypt**, siguiendo
el patrón de `PuraVidaFragance`.

### 3.1 Detalle de la decisión

**Capas:**

| Capa | Responsabilidad | Prohibido |
|---|---|---|
| Abstracciones | Modelos + interfaces | Implementación |
| API | HTTP, validación de shape, composition root | Lógica de negocio |
| Flujo | Orquestación del caso de uso **+ reglas de negocio puras** (validaciones de dominio, cálculos) | I/O directo a BD (delega en DA) |
| Servicios | Adaptadores externos: `JwtService`, `PasarelaPagoService` | Lógica de negocio |
| DA | Acceso a datos vía Dapper + Stored Procedures | Lógica de negocio, SQL inline |

Las reglas de negocio que en el taller genérico vivirían en `Reglas` (p. ej. "el código de barras no
puede repetirse", "no se puede vender más de `StockVirtual` disponible") se ubican como métodos
privados o clases auxiliares **dentro de `Flujo`**, manteniéndolas puras (sin I/O directo) pero sin
un proyecto de capa adicional.

**Seguridad — JWT propio:**
- El propio `Perfumeria.Api` emite el JWT en `POST /api/auth/login` (sin servicio externo).
- Contraseñas con `BCrypt.Net` (`BCrypt.Verify` / `BCrypt.HashPassword`), nunca hash propio.
- Claims: `ClaimTypes.NameIdentifier`, `ClaimTypes.Email`, `ClaimTypes.Name`, `ClaimTypes.Role`
  (valor: `"Admin"` o `"Cliente"`, string simple).
- Revocación de tokens al hacer logout: tabla `TokensRevocados` (Id, Token, IdUsuario, FechaExpira),
  igual que en el proyecto de referencia.
- `[Authorize]` a nivel de clase por defecto; `[AllowAnonymous]` explícito en endpoints de catálogo,
  registro de cliente y webhook de la pasarela.

### 3.2 Alcance

- **Aplica a:** todo el backend de Perfumeria.
- **No aplica a:** un eventual escenario multi-producto donde compartir un servicio de seguridad
  centralizado sí se justifique — ese caso requeriría un nuevo ADR.

---

## 4. Alternativas evaluadas

| Alternativa | Descripción | Pros | Contras | ¿Descartada por? |
|---|---|---|---|---|
| A. 6 capas con `Reglas` separada + `Seguridad.API` externa (taller genérico) | El estándar "de libro" del curso | Máxima separación de responsabilidades; reutilizable entre productos | Complejidad operativa alta para 1 API y equipo pequeño; no coincide con el proyecto de referencia | Sobre-ingeniería para el alcance actual |
| B. 5 capas + JWT propio ✅ | Igual a `PuraVidaFragance` | Más simple de operar y depurar; un solo proyecto para levantar; coincide con el proyecto de referencia | Reglas de negocio menos aisladas (mitigado con disciplina de Clean Code dentro de `Flujo`) | **Elegida** |
| C. Sin capas (todo en Controllers) | Mínimo esfuerzo | Rapidez inicial | Viola SOLID/Clean Architecture; imposible de testear en aislamiento | Descartada — viola Constitution §6 |

---

## 5. Consecuencias

### 5.1 Positivas
- Un solo servicio que levantar y depurar durante el desarrollo del curso.
- Estructura idéntica al proyecto de referencia, facilitando comparar patrones.
- Menos superficie de configuración compartida (no hay que sincronizar `Issuer`/`Audience`/`Key`
  entre dos APIs).

### 5.2 Negativas / Costos
- Las reglas de negocio puras deben mantenerse disciplinadamente sin I/O dentro de `Flujo`, ya que
  no hay un compilador/capa que lo fuerce estructuralmente.
- Si el proyecto creciera a multi-producto, la autenticación tendría que centralizarse — trabajo de
  migración futuro.

### 5.3 Neutrales / Observaciones
- `BCrypt.Net` es la librería estándar de la industria para hash de contraseñas — no requiere ADR de
  excepción a la lista blanca de backend.

---

## 6. Cumplimiento y verificación

- Revisión de código valida que `Flujo` no importe `Microsoft.Data.SqlClient` ni haga I/O directo.
- Pruebas unitarias de `Flujo` no requieren base de datos (mocks de `IPerfumeDA`, etc.).
- CI ejecuta `dotnet build /warnaserror` y la suite de pruebas de las 5 capas.

---

## 7. Excepciones

- Extraer una capa `Reglas` explícita para un caso de uso puntual con lógica muy compleja requiere
  ADR de excepción documentando el caso.

---

## 8. Referencias

- Proyecto de referencia: `PuraVidaFragance.API` (`Program.cs`, `AuthFlujo.cs`, `JwtService.cs`).
- Robert C. Martin — *Clean Architecture*.
- Microsoft Learn — *ASP.NET Core JWT Bearer Authentication*.
- ADRs relacionados: `ADR-001` (monorepo), `ADR-002` (SDD+TDD).

---

## 9. Historial

| Fecha | Estado | Autor | Cambio |
|---|---|---|---|
| 2026-07-21 | aceptado | Camila | Versión inicial — reemplaza el modelo de 6 capas + seguridad externa del taller por el patrón de 5 capas + JWT propio de `PuraVidaFragance`. |
