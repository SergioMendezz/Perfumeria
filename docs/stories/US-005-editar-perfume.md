---
id: US-005
title: Editar un perfume existente en el catálogo
status: implementada
owner: Por definir
size: M
priority: media
related_epic: Gestión de catálogo (panel admin)
tags: [perfume, catalogo, admin, panel, edicion]
version: 0.2
last-reviewed: 2026-07-24
---

# US-005 · Editar un perfume existente en el catálogo

---

## 1. Historia de usuario

**Como** *Admin (personal de la tienda)*
**Quiero** *editar un perfume existente en el catálogo*
**Con el fin de** *corregir datos o actualizar la descripción e imagen sin tener que eliminarlo y
recrearlo*

---

## 2. Valor de negocio

- Evita perder el historial y las relaciones del registro (`Id`, `Variantes`, referencias en
  pedidos pasados) que se romperían si la única forma de corregir un dato fuera eliminar y volver a
  crear el perfume.
- Permite corregir errores de carga (nombre mal tipeado, imagen rota, categoría equivocada) sin
  reintroducir el riesgo de duplicar `CodigoBarras` que ya resolvió `US-003`.

---

## 3. Criterios INVEST

- [x] **Independiente** — no depende del checkout ni del registro de clientes; sí depende de que el
  perfume ya exista (`US-003`).
- [x] **Negociable** — los campos editables exactos pueden ajustarse con el PO.
- [x] **Valiosa** — es el único punto de corrección de datos de un perfume ya publicado.
- [x] **Estimable** — un endpoint de edición con las mismas validaciones que el alta (`US-003`).
- [x] **Small** — cubre solo la edición del `Perfume`; editar `VariantePerfume` (mililitros, precio,
  stock) queda fuera.
- [x] **Testable** — validable con payloads concretos y códigos HTTP.

---

## 4. Criterios de aceptación de alto nivel

**AC-01 — Edición exitosa de un perfume existente**
- **Dado** que existe un perfume con `Id = "b2c1a900-1111-4a2b-9c3d-000000000001"`,
  `CodigoBarras = "7501234567890"` y `Nombre = "Dior Sauvage"`
- **Cuando** el personal autenticado como `Admin` envía
  `PUT /api/perfume/b2c1a900-1111-4a2b-9c3d-000000000001` con
  `{ IdMarca: "3fa85f64-5717-4562-b3fc-2c963f66afa6", Nombre: "Dior Sauvage Elixir",
  CodigoBarras: "7501234567890", Genero: "Hombre", Categoria: "Eau de Parfum",
  ImagenUrl: "https://cdn.example.com/dior-sauvage-elixir.jpg" }`
- **Entonces** el sistema responde `200 OK` con el perfume actualizado, incluyendo
  `Nombre = "Dior Sauvage Elixir"` e `ImagenUrl` actualizada

**AC-02 — Perfume inexistente**
- **Dado** que no existe ningún perfume con `Id = "00000000-0000-0000-0000-000000000000"`
- **Cuando** el personal autenticado como `Admin` envía
  `PUT /api/perfume/00000000-0000-0000-0000-000000000000` con un payload válido
- **Entonces** el sistema responde `404 Not Found` y no crea un registro nuevo

**AC-03 — Requiere autenticación de Admin**
- **Dado** que la petición no incluye un token válido con rol `Admin`
- **Cuando** se envía `PUT /api/perfume/{id}` con un payload válido sobre un perfume existente
- **Entonces** el sistema responde `401` (sin token) o `403` (token de rol `Cliente`), y no modifica
  el perfume

**AC-04 — Código de barras duplicado con otro perfume**
- **Dado** que el perfume `A` tiene `CodigoBarras = "7501234567890"` y el perfume `B` (distinto
  `Id`) tiene `CodigoBarras = "7509999999999"`
- **Cuando** el personal autenticado como `Admin` edita el perfume `B` enviando
  `CodigoBarras = "7501234567890"` (el de `A`)
- **Entonces** el sistema responde `409 Conflict` y no modifica el perfume `B`

**AC-05 — Reenviar el propio código de barras sin cambio no genera falso conflicto**
- **Dado** que el perfume `A` tiene `CodigoBarras = "7501234567890"`
- **Cuando** el personal autenticado como `Admin` edita el perfume `A` reenviando ese mismo
  `CodigoBarras = "7501234567890"` junto con otros campos modificados
- **Entonces** el sistema responde `200 OK` y actualiza el perfume `A` normalmente (no lo trata como
  duplicado contra sí mismo)

**AC-06 — Categoría inválida o abreviada rechazada**
- **Dado** que el payload de edición envía `Categoria = "EDP"`
- **Cuando** el personal autenticado como `Admin` envía `PUT /api/perfume/{id}` con ese valor sobre
  un perfume existente
- **Entonces** el sistema responde `400 Bad Request` indicando que debe usarse el nombre completo de
  la categoría (`docs/vision.md` §3.1) y no modifica el perfume

**AC-07 — Nombre normalizado a mayúscula inicial tras cada espacio**
- **Dado** que el payload de edición envía `Nombre = "dior sauvage elixir"`
- **Cuando** el personal autenticado como `Admin` edita un perfume existente con ese valor
- **Entonces** el perfume se persiste con `Nombre = "Dior Sauvage Elixir"` (ver Skill
  `nomenclatura-productos`)

---

## 5. Restricciones y supuestos

- **Restricción:** esta historia no incluye la edición de `VariantePerfume` (mililitros, precio,
  stock) — eso se hace por separado a través de los endpoints de variante
  (`docs/vision.md` §5: `/api/variante/{id}/stock-tienda`, `/api/variante/{id}/stock-virtual`, y el
  endpoint de datos de variante que no está aún cubierto por ninguna US).
- **Restricción:** el `IdMarca` debe referenciar una `Marca` ya existente, igual que en `US-003`.
- **Supuesto:** la edición reemplaza los campos editables del `Perfume` (`Nombre`, `CodigoBarras`,
  `Genero`, `Categoria`, `ImagenUrl`, `Descripcion`) en conjunto (payload completo tipo `PUT`, no
  `PATCH` parcial) — `docs/vision.md` §5 solo lista `PUT` para este endpoint.

---

## 6. Fuera de alcance

- Edición de `VariantePerfume` (ml, precio, stock) — historia futura.
- Activar/desactivar (eliminación lógica) de un perfume — ya tiene su propio método declarado
  (`Activar`) en `IPerfumeFlujo`/`IPerfumeDA`; es una historia separada.
- Subida de archivo de imagen — `ImagenUrl` se recibe como URL ya alojada, igual que en `US-003`.
- Edición de marcas — depende del CRUD de `Marca`, historia separada.

---

## 7. Dependencias

- **Depende de:** `US-003` (alta de perfume) — no se puede editar un perfume que no existe.
- **Es dependencia de:** ninguna historia identificada por ahora.

---

## 8. Notas de refinamiento

- ⚠️ **Supuesto a confirmar:** el beneficio de negocio original mencionaba "actualizar precio", pero
  `docs/vision.md` §3 **no** define un campo `Precio` en la entidad `Perfume` — el precio vive en
  `VariantePerfume` (asociada por `IdPerfume`). Por eso ningún AC de esta historia toca `Precio`.
  Confirmar con el PO si el enunciado se refería a editar la variante (fuera de alcance aquí, ver
  §6) o si `Perfume` debería tener un campo `Precio` propio — en ese caso requeriría actualizar
  `docs/vision.md` §3 primero, no asumirlo en esta US.
- ¿`409 Conflict` sigue siendo el código correcto para colisión de `CodigoBarras` contra *otro*
  perfume, igual que en `US-003` AC-02? Se asume que sí por consistencia; confirmar antes de
  implementar (`/generar-prueba-desde-ac` para AC-04).
- **Estado del código al 2026-07-24 (actualiza la nota anterior, ya obsoleta):** los siete AC quedaron
  implementados y probados en las tres capas donde aplica. `PerfumeFlujo.Editar` valida existencia
  (AC-02, `PerfumeNoEncontradoException`), código de barras duplicado excluyendo el propio `Id`
  (AC-04/AC-05, `CodigoBarrasDuplicadoException`), categoría (AC-06, `CategoriaInvalidaException`) y
  normaliza el nombre (AC-07), reutilizando los mismos validadores que `Crear` tras un refactor que
  extrajo `ValidarYNormalizarPerfume` como orquestación compartida. `PerfumeDA.Editar` invoca el SP
  `Perfume_Editar` (ya existía en `BD/`, se corrigió su `SELECT` final para devolver el registro
  completo). `PerfumeController.Editar` expone `PUT /api/perfume/{id}` con
  `[Authorize(Roles = "Admin")]` (AC-03) y traduce `PerfumeNoEncontradoException`/
  `CodigoBarrasDuplicadoException` a `404`/`409`. AC-03 y AC-05 no requirieron una fase GREEN propia:
  ya quedaban cubiertos como efecto correcto del diseño de AC-01 y AC-04 respectivamente, y se
  agregaron como tests de regresión.

---

## 9. Historial

| Versión | Fecha | Autor | Cambios |
|---|---|---|---|
| 0.1 | 2026-07-23 | Claude Code | Versión inicial — generada con `/generar-historia-de-usuario`. |
| 0.2 | 2026-07-24 | Claude Code | AC-01 a AC-07 implementados y probados en las 3 capas (Flujo/DA/API), ciclo TDD completo. Refactor posterior: unificó validación de código de barras duplicado entre `Crear`/`Editar`, extrajo `ValidarYNormalizarPerfume` y `MapearFila` para eliminar duplicación. Ver detalle en §8. |
