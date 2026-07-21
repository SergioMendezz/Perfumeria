---
adr_id: ADR-005
title: División de inventario en StockTienda y StockVirtual por variante/producto
status: aceptado
date: 2026-07-21
authors: [Camila]
reviewers: [Por definir]
tags: [backend, base-de-datos, inventario, critico]
supersedes: []
superseded_by: []
---

# ADR-005 · División de inventario (StockTienda / StockVirtual)

## 1. Estado

`aceptado`.

---

## 2. Contexto

El negocio opera una tienda física y una tienda en línea. Ambos canales venden del mismo catálogo de
productos, pero **no** deben compartir el mismo contador de existencias: una venta presencial no
debería poder dejar sin stock una compra en línea ya confirmada, y viceversa. El requisito original
lo expresa así: *"división de stocks (inventario para tienda física y otro para virtual)"* e
*"inventario directo para tienda en línea"*.

No existe (en el alcance de este proyecto) integración con un punto de venta físico automatizado —
el ajuste del stock de tienda física es manual, hecho por el personal desde el panel admin.

---

## 3. Decisión

Cada tabla de producto con inventario (`VariantePerfume`, `Desodorante`, `BodySpray`, `ShowerGel`,
`CremaCorporal`, `Body`, `Estuche`) tiene **dos columnas de stock independientes**:
`StockTienda` (int) y `StockVirtual` (int), en vez de un único campo `Stock`.

### 3.1 Detalle de la decisión

- **`StockVirtual`** es el único que la tienda en línea (`perfumeria-tienda`) puede leer para decidir
  disponibilidad y el único que se descuenta automáticamente al confirmarse un pago (`ADR-004`).
- **`StockTienda`** es informativo/administrable manualmente desde `perfumeria-admin`; ningún flujo
  automático lo modifica en esta versión.
- Los endpoints de ajuste de stock son **independientes por canal**:
  `PATCH /api/variante/{id}/stock-tienda` y `PATCH /api/variante/{id}/stock-virtual` — nunca un
  único endpoint genérico `stock` que reciba el canal como parámetro libre (evita errores de canal
  equivocado en el cliente).
- El descuento de `StockVirtual` al confirmar un pago **MUST** ejecutarse dentro de la misma
  transacción SQL que marca el `Pedido` como `Pagado`, y **MUST** validar que `StockVirtual >=
  Cantidad` antes de descontar (si no alcanza, el pedido no se marca `Pagado` y se reporta error de
  disponibilidad — nunca un stock negativo).
- El total de unidades del catálogo (mencionado como "~2 millones en stock") se entiende como la suma
  de `StockTienda + StockVirtual` de todas las variantes/productos, no como un límite por producto
  individual.

### 3.2 Alcance

- **Aplica a:** todas las tablas con inventario del catálogo.
- **No aplica a:** una futura integración con un POS físico automatizado, que requeriría revisar este
  ADR (p. ej. para sincronizar `StockTienda` en tiempo real).

---

## 4. Alternativas evaluadas

| Alternativa | Descripción | Pros | Contras | ¿Descartada por? |
|---|---|---|---|---|
| A. Un solo campo `Stock` compartido | Igual que `VariantesPerfume.Stock` en `PuraVidaFragance` | Simplicidad | Una venta física deja sin stock la tienda en línea sin ninguna señal — viola el requisito explícito del negocio | Descartada — no cumple el requisito de negocio |
| **B. Dos columnas (`StockTienda`/`StockVirtual`) en la misma tabla** ✅ | Cada variante/producto trae ambos contadores | Simple de consultar (sin JOIN adicional), cumple el requisito, fácil de migrar desde el modelo de referencia agregando 1 columna | Requiere disciplina para no confundir el endpoint de ajuste | **Elegida** |
| C. Tabla `Inventario` separada por canal (`IdProducto`, `Canal`, `Cantidad`) | Modelo más "normalizado" | Extensible a más canales en el futuro (p. ej. una segunda sucursal física) | Complejidad de JOIN adicional para un caso de solo 2 canales fijos | Pospuesta — se documenta como posible evolución si aparece una tercera sucursal |

---

## 5. Consecuencias

### 5.1 Positivas
- Cumple el requisito de negocio explícito sin ambigüedad.
- Los reportes de "stock total" son una simple suma, sin JOIN adicional.
- El descuento automático solo afecta el canal correcto.

### 5.2 Negativas / Costos
- Si en el futuro se necesitara un tercer canal (p. ej. una segunda sucursal), este ADR debe
  revisarse hacia la Alternativa C.

### 5.3 Neutrales / Observaciones
- El panel admin debe mostrar ambos contadores por separado y nunca sumarlos automáticamente en la
  UI sin dejar claro que es un total agregado (evita que el personal confunda "stock total" con
  "stock disponible para vender en línea").

---

## 6. Cumplimiento y verificación

- Prueba obligatoria: confirmar un pago no debe modificar `StockTienda`.
- Prueba obligatoria: ajustar `StockTienda` desde el panel no debe modificar `StockVirtual`.
- Prueba de concurrencia (`ADR-002` §3.1.6): dos pagos simultáneos por la última unidad de
  `StockVirtual` — solo uno debe tener éxito.

---

## 7. Excepciones

- Migrar a una tabla `Inventario` separada (Alternativa C) requiere un nuevo ADR.

---

## 8. Referencias

- `docs/vision.md` §4 y §6.
- Modelo de referencia: `VariantesPerfume.sql` de `PuraVidaFragance` (campo `Stock` único — punto de
  partida que este ADR modifica).
- ADRs relacionados: `ADR-003` (arquitectura backend), `ADR-004` (pasarela de pago).

---

## 9. Historial

| Fecha | Estado | Autor | Cambio |
|---|---|---|---|
| 2026-07-21 | aceptado | Camila | Versión inicial. |
