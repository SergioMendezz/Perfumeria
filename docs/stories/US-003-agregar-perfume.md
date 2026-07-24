---
id: US-003
title: Agregar un perfume nuevo al catálogo
status: implementada
owner: Por definir
size: M
priority: alta
related_epic: Gestión de catálogo (panel admin)
tags: [perfume, catalogo, admin, panel, alta]
version: 0.3
last-reviewed: 2026-07-24
---

# US-003 · Agregar un perfume nuevo al catálogo

---

## 1. Historia de usuario

**Como** *Admin (personal de la tienda)*
**Quiero** *agregar un perfume nuevo al catálogo*
**Con el fin de** *poder venderlo en la tienda*

---

## 2. Valor de negocio

- Expandir el catálogo es la única forma de ofrecer productos nuevos a la venta — sin este flujo el
  catálogo queda estático y el negocio no puede crecer su oferta.
- Mantener el `CodigoBarras` único por producto evita conflictos de identificación física entre la
  tienda y el sistema, y sostiene la búsqueda rápida por código (`US-002`).

---

## 3. Criterios INVEST

- [x] **Independiente** — no depende del checkout ni del registro de clientes; sí depende de que
  exista al menos una `Marca` activa (ver §7).
- [x] **Negociable** — los campos exactos del formulario pueden ajustarse con el PO.
- [x] **Valiosa** — es el único punto de entrada para poblar el catálogo de perfumes.
- [x] **Estimable** — un endpoint de creación + validaciones conocidas.
- [x] **Small** — cubre solo el alta del `Perfume`; variantes (ml/precio/stock) quedan fuera.
- [x] **Testable** — validable con payloads concretos y códigos HTTP.

---

## 4. Criterios de aceptación de alto nivel

**AC-01 — Alta exitosa de un perfume nuevo**
- **Dado** que existe una `Marca` activa con `Id = "3fa85f64-5717-4562-b3fc-2c963f66afa6"` y ningún
  perfume tiene `CodigoBarras = "7501234567890"`
- **Cuando** el personal autenticado como `Admin` envía
  `POST /api/perfume` con `{ IdMarca: "3fa85f64-5717-4562-b3fc-2c963f66afa6", Nombre: "Dior Sauvage",
  CodigoBarras: "7501234567890", Genero: "Hombre", Categoria: "Eau de Parfum", ImagenUrl: "https://cdn.example.com/dior-sauvage.jpg" }`
- **Entonces** el sistema responde `201 Created` con el perfume creado, incluyendo un `Id` nuevo y
  `Activo = true`

**AC-02 — Código de barras duplicado**
- **Dado** que ya existe un perfume con `CodigoBarras = "7501234567890"`
- **Cuando** el personal envía `POST /api/perfume` con ese mismo `CodigoBarras`
- **Entonces** el sistema responde `409 Conflict` y no crea un segundo registro

**AC-03 — Requiere autenticación de Admin**
- **Dado** que la petición no incluye un token válido con rol `Admin`
- **Cuando** se envía `POST /api/perfume` con un payload válido
- **Entonces** el sistema responde `401` (sin token) o `403` (token de rol `Cliente`), y no crea el
  perfume

**AC-04 — Nombre y Marca normalizados a mayúscula inicial tras cada espacio**
- **Dado** que el payload envía `Nombre = "dior sauvage"`
- **Cuando** el personal autenticado como `Admin` envía `POST /api/perfume` con ese valor
- **Entonces** el perfume se persiste con `Nombre = "Dior Sauvage"` (ver Skill
  `nomenclatura-productos`)

**AC-05 — Categoría inválida o abreviada rechazada**
- **Dado** que el payload envía `Categoria = "EDP"`
- **Cuando** el personal autenticado como `Admin` envía `POST /api/perfume` con ese valor
- **Entonces** el sistema responde `400 Bad Request` indicando que debe usarse el nombre completo de
  la categoría (`docs/vision.md` §3.1) y no crea el perfume

---

## 5. Restricciones y supuestos

- **Restricción:** esta historia no incluye el alta de `VariantePerfume` (mililitros, precio,
  stock) — eso es una historia separada que asigna variantes a un perfume ya creado.
- **Restricción:** el `IdMarca` debe referenciar una `Marca` ya existente; esta historia no crea
  marcas nuevas.
- **Supuesto:** existen marcas activas registradas en el sistema antes de ejecutar esta historia.

---

## 6. Fuera de alcance

- Alta de `VariantePerfume` (ml, precio, stock) — historia futura.
- Edición (`PUT`) o eliminación lógica (`DELETE`) de un perfume existente — historias separadas.
- Subida de archivo de imagen — `ImagenUrl` se recibe como URL ya alojada, no como upload.
- Alta de marcas — depende del CRUD de `Marca`, historia separada.

---

## 7. Dependencias

- **Depende de:** que exista al menos una `Marca` activa registrada (CRUD de `Marca`).
- **Es dependencia de:** la historia de alta de `VariantePerfume` (asigna ml/precio/stock a un
  perfume ya creado) y de `US-002` (la búsqueda por código de barras solo tiene sentido sobre
  perfumes ya cargados).

---

## 8. Notas de refinamiento

- ¿`ImagenUrl` es obligatorio al crear, o puede completarse después en una edición? `docs/vision.md`
  §3 no lo marca como opcional (a diferencia de `Descripcion`) — se asume obligatorio en AC-01;
  confirmar con el PO.
- ¿`409 Conflict` es el código HTTP correcto para `CodigoBarras` duplicado, o el equipo prefiere
  `400 Bad Request` con un mensaje de validación? `docs/vision.md` §5 no lo especifica — se eligió
  `409` por ser la convención estándar para conflictos de unicidad; confirmar antes de implementar
  (`/generar-prueba-desde-ac` para AC-02).
- **Pendiente técnico:** los AC de autorización (401/403) están cubiertos por test de reflexión, no
  por ejecución real del middleware. Cuando existan 4-5 endpoints protegidos, evaluar agregar
  `Microsoft.AspNetCore.Mvc.Testing` + `WebApplicationFactory<Program>` como un ciclo aparte de tests
  de integración, cubriendo todos de una vez.

---

## 9. Historial

| Versión | Fecha | Autor | Cambios |
|---|---|---|---|
| 0.1 | 2026-07-21 | Claude Code | Versión inicial — generada con `/generar-historia-de-usuario`. |
| 0.2 | 2026-07-22 | Claude Code | Se agregó pendiente técnico en §8 sobre cobertura real de autorización (401/403) vía tests de integración. |
| 0.3 | 2026-07-24 | Claude Code | AC-01 a AC-05 implementados y probados en las 3 capas (Flujo/DA/API). Se detectó y corrigió durante auditoría que `PerfumeController.Crear` no traducía `CodigoBarrasDuplicadoException`/`CategoriaInvalidaException` a `409`/`400` (habría devuelto `500`) — cerrado con ciclo RED→GREEN y tests de API (`Crear_CodigoBarrasDuplicado_Retorna409`, `Crear_CategoriaAbreviadaOInvalida_Retorna400`). |
