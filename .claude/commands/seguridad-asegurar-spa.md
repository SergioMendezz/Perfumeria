---
description: Aplica autenticación (login, token, guards) a perfumeria-tienda y/o perfumeria-admin, y asegura que el checkout delegue en la pasarela de pago sin capturar datos de tarjeta propios.
argument-hint: <tienda|admin>
allowed-tools: Read, Write, Edit, Bash, Grep, Glob
---

# 🔐 Comando — Asegurar el SPA (perfumeria-tienda / perfumeria-admin)

Eres un experto en **React + TypeScript + Vite** y consumo seguro de APIs con JWT. Tu tarea es
agregar autenticación a la SPA indicada en `$ARGUMENTS`, consumiendo `Perfumeria.Api`.

## 🎯 Objetivo

- Hacer login contra `POST {VITE_API_URL}/auth/login`.
- Almacenar el `token` recibido y adjuntarlo como `Authorization: Bearer` en toda llamada.
- Proteger rutas privadas con un guard según el rol requerido.
- **Si `$ARGUMENTS = tienda`:** además, asegurar que el checkout delega en el checkout hospedado o
  SDK de la pasarela de pago (`ADR-004`) — nunca un formulario propio de tarjeta.

## ⛔ Restricciones (obligatorias)

- **NO** modificar el backend; solo el frontend.
- **NO** duplicar la lógica de `fetch`: centralizar en `apiFetch` (sin `axios`, `ADR-Frontend-002`).
- Tipar todo con TypeScript estricto (sin `any` innecesario).
- **NO** crear un `<input>` de número de tarjeta/CVV en `perfumeria-tienda`.

## ✅ Pasos que debes ejecutar

1. **`.env`:** `VITE_API_URL`.
2. **`src/seguridad/tokenStore.ts`:** `get/set/clear/isAuthenticated`.
3. **`src/seguridad/authService.ts`:** `login(email, password)` → `POST /auth/login`; `logout()`.
   Si `$ARGUMENTS = tienda`, agregar también `registrarCliente(...)` → `POST /auth/registro-cliente`.
4. **`src/seguridad/apiClient.ts`:** `apiFetch<T>` con Bearer automático y manejo de `401` (limpiar
   token + redirigir a `/login`) y `403` (mensaje de acceso denegado).
5. **`src/seguridad/RequireAuth.tsx`:** guard parametrizable por rol
   (`<RequireAuth rol="Admin">` / `<RequireAuth rol="Cliente">`).
6. **`src/pages/LoginPage.tsx`:** formulario de login con manejo de credenciales inválidas.
7. **Si `$ARGUMENTS = tienda`:** envolver `/checkout` y `/mis-pedidos` con
   `<RequireAuth rol="Cliente">`; el resto del catálogo queda público. Verificar que el componente
   de checkout redirige/monta el SDK de la pasarela sin campos de tarjeta propios.
8. **Si `$ARGUMENTS = admin`:** envolver toda la app con `<RequireAuth rol="Admin">`.
9. **Refactor:** reemplazar `fetch` directo por `apiFetch` en los servicios existentes.

## 📤 Salida esperada

- Archivos nuevos bajo `src/seguridad/` + `LoginPage.tsx`.
- Router actualizado con los guards correspondientes al rol.

## 🧪 Criterios de aceptación

- [ ] El login guarda el token y redirige a la ruta privada correcta.
- [ ] Toda llamada al API incluye `Authorization: Bearer`.
- [ ] Un `401` limpia la sesión y redirige a `/login`.
- [ ] Las rutas privadas no son accesibles sin sesión del rol correcto.
- [ ] (Solo `tienda`) El checkout no renderiza campos de tarjeta propios.
