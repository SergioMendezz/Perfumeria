---
id: US-001
title: Listar el catálogo de perfumes sin necesidad de registro
status: implementada
owner: Por definir
size: S
priority: alta
related_epic: Catálogo público
tags: [perfume, catalogo, publico, consulta]
version: 0.2
last-reviewed: 2026-07-21
---

# US-001 · Listar el catálogo de perfumes sin necesidad de registro

---

## 1. Historia de usuario

**Como** *visitante de la tienda en línea*
**Quiero** *ver el catálogo de perfumes disponibles sin tener que crear una cuenta*
**Con el fin de** *decidir si quiero comprar antes de comprometerme a registrarme*

---

## 2. Valor de negocio

- Reduce la fricción de entrada: exigir registro para solo mirar productos aleja compradores
  potenciales (regla de negocio explícita del proyecto, `docs/vision.md` §2).
- Un catálogo indexable y visible públicamente favorece el descubrimiento orgánico (SEO) de la
  tienda.

---

## 3. Criterios INVEST

- [x] **Independiente** — no depende de que exista el checkout ni el registro de clientes.
- [x] **Negociable** — el orden y los filtros visibles pueden ajustarse con el PO.
- [x] **Valiosa** — es la puerta de entrada de todo el negocio en línea.
- [x] **Estimable** — consulta + paginado + renderizado.
- [x] **Small** — un endpoint de lectura y una vista.
- [x] **Testable** — validable con datos concretos y códigos HTTP.

---

## 4. Criterios de aceptación de alto nivel

**AC-01 — Listado paginado de perfumes activos**
- **Dado** que existen 25 perfumes activos registrados
- **Cuando** un visitante sin token consulta `GET /api/perfume?pagina=1&tamano=20`
- **Entonces** el sistema responde `200 OK` con 20 perfumes y un campo `total = 25`

**AC-02 — Catálogo vacío**
- **Dado** que no hay perfumes activos registrados
- **Cuando** un visitante consulta `GET /api/perfume`
- **Entonces** el sistema responde `200 OK` con una lista vacía `[]` (nunca `404`)

**AC-03 — Sin necesidad de autenticación**
- **Dado** que un visitante no envía el header `Authorization`
- **Cuando** consulta `GET /api/perfume` o `GET /api/perfume/{id}`
- **Entonces** el sistema responde `200 OK` (nunca `401`)

**AC-04 — Categoría con nombre completo, nunca sigla**
- **Dado** que un perfume tiene `Categoria = "Eau de Parfum"` almacenado en la base de datos
- **Cuando** un visitante consulta `GET /api/perfume/{id}`
- **Entonces** el campo `categoria` de la respuesta es exactamente `"Eau de Parfum"`, nunca `"EDP"`

**AC-05 — Perfume inactivo no aparece en el catálogo público**
- **Dado** que un perfume tiene `Activo = false`
- **Cuando** un visitante consulta `GET /api/perfume`
- **Entonces** ese perfume no aparece en la respuesta

---

## 5. Restricciones y supuestos

- **Restricción:** esta historia cubre solo lectura; no incluye creación/edición (eso es
  responsabilidad del panel admin, historias separadas).
- **Restricción:** el paginado es obligatorio dado el volumen esperado de catálogo
  (`docs/vision.md` §4).
- **Supuesto:** existen perfumes de prueba cargados en la base de datos para validar el AC-01.

---

## 6. Fuera de alcance

- Filtros avanzados (por marca, género, rango de precio) — historia futura.
- Búsqueda por texto libre — historia futura.
- Vista de detalle con variantes (ml/precio) — historia futura (`US-002` en adelante).

---

## 7. Dependencias

- **Depende de:** que exista al menos la tabla `Perfumes` con datos semilla.
- **Es dependencia de:** la vista de detalle de perfume y el carrito de compras.

---

## 8. Notas de refinamiento

- ¿El tamaño de página por defecto debe ser 20 o algún otro número? Pendiente de definir con el PO.
- ¿Se debe exponer el `StockVirtual` en la respuesta pública, o solo un booleano `disponible`? Por
  seguridad de negocio (no revelar niveles de inventario a la competencia), se sugiere exponer solo
  `disponible: bool` — confirmar antes de implementar.

---

## 9. Historial

| Versión | Fecha | Autor | Cambios |
|---|---|---|---|
| 0.1 | 2026-07-21 | Camila | Versión inicial — historia de ejemplo generada junto con el resto del andamiaje de gobernanza. |
| 0.2 | 2026-07-21 | Claude Code | Implementados AC-01 a AC-04 (Flujo/DA/API) con pruebas unitarias xUnit+FluentAssertions+NSubstitute en verde. AC-05 (exclusión de perfumes inactivos) queda resuelto en el `WHERE Activo = 1` de `Perfume_Obtener.sql`, sin test unitario propio — requiere un test de integración contra SQL Server, pendiente de la capa de pruebas correspondiente. |
