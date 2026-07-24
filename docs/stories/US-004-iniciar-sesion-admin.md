---
id: US-004
title: Iniciar sesión en el panel de administración
status: comprometida
owner: Por definir
size: S
priority: alta
related_epic: Autenticación y seguridad
tags: [auth, login, admin, seguridad, jwt]
version: 0.2
last-reviewed: 2026-07-24
---

# US-004 · Iniciar sesión en el panel de administración

---

## 1. Historia de usuario

**Como** *Admin (personal de la tienda)*
**Quiero** *iniciar sesión con mi correo y contraseña*
**Con el fin de** *acceder al panel de administración*

---

## 2. Valor de negocio

- Es el punto de entrada obligatorio a todo el panel admin: sin login funcional, ningún otro flujo
  administrativo (catálogo, inventario, usuarios, clientes) es accesible.
- Un login que rechaza correctamente credenciales inválidas y cuentas inactivas protege el catálogo,
  el inventario y los datos de clientes de accesos no autorizados (`docs/adr/ADR-003-arquitectura-backend.md`).

---

## 3. Criterios INVEST

- [x] **Independiente** — no depende del catálogo público ni del checkout; requiere que exista al
  menos un usuario `Admin` ya cargado (ver §7).
- [x] **Negociable** — el tiempo de expiración del token y el mensaje exacto de error pueden
  ajustarse con el PO.
- [x] **Valiosa** — bloquea o habilita todo el trabajo diario del personal en el panel.
- [x] **Estimable** — un endpoint de login + validación de credenciales + emisión de JWT, patrón ya
  definido en `ADR-003`.
- [x] **Small** — cubre solo el inicio de sesión; el registro de cuentas `Admin` y el cierre de
  sesión (`logout`) son historias separadas.
- [x] **Testable** — validable con credenciales concretas y códigos HTTP.

---

## 4. Criterios de aceptación de alto nivel

**AC-01 — Login exitoso con credenciales válidas de Admin**
- **Dado** que existe un usuario con `Email = "admin@perfumeria.cr"`, `Rol = "Admin"`,
  `Activo = true`, cuya contraseña registrada (hasheada con BCrypt) es `"Clave123!"`
- **Cuando** se envía `POST /api/auth/login` con
  `{ Email: "admin@perfumeria.cr", Password: "Clave123!" }`
- **Entonces** el sistema responde `200 OK` con un token JWT no vacío y un campo `rol = "Admin"`

**AC-02 — Contraseña incorrecta**
- **Dado** que existe un usuario con `Email = "admin@perfumeria.cr"` y `Rol = "Admin"`
- **Cuando** se envía `POST /api/auth/login` con
  `{ Email: "admin@perfumeria.cr", Password: "ContraseñaIncorrecta" }`
- **Entonces** el sistema responde `401 Unauthorized` con un mensaje genérico (no debe indicar si el
  correo existe o no)
/
**AC-03 — Cuenta inactiva no puede iniciar sesión**
- **Dado** que existe un usuario con `Email = "exempleado@perfumeria.cr"`, `Rol = "Admin"` y
  `Activo = false`, con contraseña correcta
- **Cuando** se envía `POST /api/auth/login` con esas credenciales
- **Entonces** el sistema responde `401 Unauthorized` (mismo tratamiento que credenciales inválidas,
  para no revelar el estado de la cuenta)

**AC-04 — Correo no registrado**
- **Dado** que no existe ningún usuario con `Email = "inexistente@perfumeria.cr"`
- **Cuando** se envía `POST /api/auth/login` con ese correo y cualquier contraseña
- **Entonces** el sistema responde `401 Unauthorized` con el mismo mensaje genérico de AC-02 (evita
  enumeración de usuarios registrados)

**AC-05 — Actualiza el último acceso en un login exitoso**
- **Dado** un login exitoso como el de AC-01
- **Cuando** se completa la autenticación
- **Entonces** el campo `UltimoAcceso` del usuario se actualiza a la fecha y hora del login

---

## 5. Restricciones y supuestos

- **Restricción:** el endpoint `POST /api/auth/login` es compartido por `Admin` y `Cliente`
  (`docs/vision.md` §5); esta historia valida específicamente el flujo de `Admin` — el login de
  `Cliente` reutiliza el mismo endpoint y no repite estos AC.
- **Restricción:** las contraseñas se validan con `BCrypt.Verify`, nunca con comparación de texto
  plano ni hash propio (`ADR-003` §3.1).
- **Supuesto:** existe al menos un usuario `Admin` ya cargado en la base de datos (el alta de cuentas
  `Admin` es una historia separada, ya que `docs/vision.md` §4 prohíbe el autoregistro de `Admin`).

---

## 6. Fuera de alcance

- Registro de una cuenta `Admin` nueva — historia separada (`POST /api/usuario`, solo por otro
  `Admin`).
- Cierre de sesión / revocación de token (`POST /api/auth/logout`) — historia separada.
- Recuperación y restablecimiento de contraseña (`POST /api/auth/recuperar-contrasena`,
  `POST /api/auth/restablecer-contrasena`) — historias separadas.
- Login del rol `Cliente` — mismo endpoint, pero sin AC propios en esta historia (ver §5).

---

## 7. Dependencias

- **Depende de:** que exista al menos un usuario `Admin` activo cargado en la base de datos.
- **Es dependencia de:** todas las historias del panel admin que requieren `[Authorize(Roles = "Admin")]`
  (`US-003` y las que sigan).

---

## 8. Notas de refinamiento

- ¿Cuántas horas debe durar el token antes de expirar? `appsettings.json` ya define
  `Jwt:ExpiraHoras = 8` como placeholder — confirmar con el PO si es el valor de negocio correcto.
- ¿El mensaje de error de AC-02/AC-03/AC-04 debe ser textualmente idéntico en los tres casos, o basta
  con que el código HTTP sea el mismo? Se asume mensaje genérico idéntico por buena práctica de
  seguridad (evitar enumeración de usuarios) — confirmar redacción exacta con el PO antes de
  implementar.
- ⚠️ **Cobertura incompleta confirmada el 2026-07-24:** los AC-01 a AC-05 están implementados y
  probados en `Flujo.Tests` (`AuthFlujoTests`) y en `DA.Tests` (`UsuarioDATests`, para
  `ObtenerCredencialesPorEmail`/`ActualizarUltimoAcceso`), pero **no existe `AuthControllerTests.cs`**
  — no hay ningún test automatizado que confirme que `AuthController.Login` traduce
  `CredencialesInvalidasException` a `401` o el login exitoso a `200` con el token. Al leer el código
  real, `AuthController.Login` sí tiene el `try/catch` correcto, pero eso no está verificado por
  prueba alguna. El `status` se mantiene en `comprometida` hasta agregar esa cobertura de capa API
  (ciclo `/generar-prueba-desde-ac US-004 AC-01 backend API`, y así sucesivamente para los demás AC).

---

## 9. Historial

| Versión | Fecha | Autor | Cambios |
|---|---|---|---|
| 0.1 | 2026-07-21 | Claude Code | Versión inicial — generada con `/generar-historia-de-usuario`. |
| 0.2 | 2026-07-24 | Claude Code | Confirmado que AC-01 a AC-05 están implementados y probados en `Flujo.Tests`/`DA.Tests`, pero falta cobertura de capa API (`AuthControllerTests.cs` no existe). Status queda en `comprometida` hasta cerrar ese hueco — ver nota en §8. |
