  ---
id: US-002
title: Buscar un producto en el inventario por código de barras
status: comprometida
owner: Por definir
size: M
priority: alta
related_epic: Gestión de inventario
tags: [inventario, codigo-barras, admin, panel]
version: 0.1
last-reviewed: 2026-07-21
---

# US-002 · Buscar un producto en el inventario por código de barras

---

## 1. Historia de usuario

**Como** *personal administrativo de la tienda*
**Quiero** *buscar un producto escaneándolo con la pistola de código de barras, o escribiendo solo
los últimos dígitos del código*
**Con el fin de** *encontrar rápidamente el producto correcto para editarlo o revisar su stock, sin
tener que escribir el código completo a mano*

---

## 2. Valor de negocio

- Con ~2 millones de unidades de inventario esperadas, buscar manualmente por nombre es lento;
  el código de barras es la forma más rápida y confiable de identificar un producto físico exacto.
- Permitir búsqueda por los últimos dígitos evita depender de que el código completo sea siempre
  legible o esté siempre a mano.

---

## 3. Criterios INVEST

- [x] **Independiente** — no depende del checkout ni del catálogo público.
- [x] **Negociable** — el número de dígitos finales aceptados puede ajustarse.
- [x] **Valiosa** — acelera directamente el trabajo diario del personal.
- [x] **Estimable** — un endpoint de búsqueda + un campo de entrada en el panel.
- [x] **Small** — acotado a la búsqueda, no a la edición.
- [x] **Testable** — validable con códigos de barras concretos.

---

## 4. Criterios de aceptación de alto nivel

**AC-01 — Búsqueda por código de barras completo**
- **Dado** que existe un perfume con `CodigoBarras = "7501234567890"`
- **Cuando** el personal autenticado como `Admin` consulta
  `GET /api/perfume/buscar-codigo-barras?codigo=7501234567890`
- **Entonces** el sistema responde `200 OK` con ese perfume exacto

**AC-02 — Búsqueda por los últimos dígitos**
- **Dado** que existe un perfume con `CodigoBarras = "7501234567890"`
- **Cuando** el personal consulta `GET /api/perfume/buscar-codigo-barras?codigo=67890`
- **Entonces** el sistema responde `200 OK` con una lista que incluye ese perfume (coincidencia por
  sufijo, no por subcadena en cualquier posición)

**AC-03 — Código no encontrado**
- **Dado** que ningún producto tiene un código de barras que coincida
- **Cuando** el personal busca `GET /api/perfume/buscar-codigo-barras?codigo=00000`
- **Entonces** el sistema responde `200 OK` con una lista vacía `[]` (no `404`, porque puede haber
  coincidencias en otras categorías de producto)

**AC-04 — Requiere autenticación de Admin**
- **Dado** que la petición no incluye un token válido con rol `Admin`
- **Cuando** se consulta `GET /api/perfume/buscar-codigo-barras?codigo=67890`
- **Entonces** el sistema responde `401` (sin token) o `403` (token de rol `Cliente`)

**AC-05 — Entrada de pistola lectora en el panel**
- **Dado** que el personal enfoca el campo de búsqueda del panel admin
- **Cuando** escanea un código con la pistola lectora (que simula tipeo + tecla Enter)
- **Entonces** el formulario dispara la búsqueda automáticamente al detectar el Enter, sin que el
  personal tenga que hacer clic en un botón

---

## 5. Restricciones y supuestos

- **Restricción:** esta historia cubre solo la búsqueda; la edición del producto encontrado es una
  historia separada.
- **Restricción:** la búsqueda por sufijo debe estar indexada (ver Skill `codigo-barras`) para no
  degradar el rendimiento con ~2 millones de unidades.
- **Supuesto:** la pistola lectora se comporta como un teclado estándar (entrada de texto + Enter),
  sin necesidad de driver especial — comportamiento típico de lectores USB HID.

---

## 6. Fuera de alcance

- Búsqueda simultánea entre todas las categorías de producto en un solo endpoint unificado —
  esta historia cubre `Perfume`; las demás categorías siguen el mismo patrón en historias futuras.
- Registro de un nuevo producto escaneando su código de barras por primera vez — historia futura.

---

## 7. Dependencias

- **Depende de:** que el campo `CodigoBarras` esté indexado en la tabla `Perfumes`.
- **Es dependencia de:** el flujo de edición rápida de inventario desde el panel admin.

---

## 8. Notas de refinamiento

- ¿Cuántos dígitos finales se consideran "suficientes" para una búsqueda por sufijo? Se sugiere un
  mínimo de 4 para evitar demasiadas coincidencias — confirmar con el PO.
- **AC-05 queda pendiente hasta que exista `perfumeria-admin`** (`/crear-estructura-spa`). El
  backend (AC-01 a AC-04) ya está implementado y probado, pero la entrada de pistola lectora es un
  comportamiento de frontend (campo de búsqueda del panel + detección de tecla Enter) que no puede
  implementarse ni testearse hasta que esa SPA exista. El `status` de esta historia se mantiene en
  `comprometida` en vez de `implementada` hasta cerrar este AC.

---

## 9. Historial

| Versión | Fecha | Autor | Cambios |
|---|---|---|---|
| 0.1 | 2026-07-21 | Camila | Versión inicial — historia de ejemplo generada junto con el resto del andamiaje de gobernanza. |
