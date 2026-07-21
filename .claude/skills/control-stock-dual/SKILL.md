---
name: control-stock-dual
description: Procedimiento para leer, ajustar y descontar StockTienda y StockVirtual de forma independiente en cualquier tabla de producto con inventario. Se activa automáticamente al trabajar en endpoints, stored procedures o componentes de UI que muestren o modifiquen cantidades de inventario.
---

# Skill · Control de stock dual (StockTienda / StockVirtual)

## Por qué existe esta skill
Ver `ADR-005-division-stock.md` para la decisión completa. Resumen operativo: **nunca existe un
campo `Stock` único** en este proyecto — siempre hay `StockTienda` (ajuste manual, informativo) y
`StockVirtual` (el que la tienda en línea puede vender).

## Regla de oro
> Si estás a punto de escribir o leer una columna llamada `Stock` a secas, deteneté: probablemente
> falta especificar `StockTienda` o `StockVirtual`.

## Procedimiento — backend (Flujo/DA)

1. **Lectura para catálogo público:** el endpoint de catálogo (`GET /api/perfume`, etc.) **MUST NOT**
   exponer los números crudos de `StockVirtual` al público (riesgo de negocio: revela inventario a la
   competencia). Exponer un campo derivado `disponible: bool` (`StockVirtual > 0`).

2. **Lectura para el panel admin:** `GET /api/variante/{id}` (o equivalente por categoría) **MUST**
   devolver ambos campos por separado, nunca sumados en la respuesta de la API (la suma, si hace
   falta mostrarla, se calcula en la UI y se etiqueta claramente como "total agregado").

3. **Ajuste manual desde el panel (`StockTienda`):**
   ```csharp
   // Flujo — ejemplo de forma, no de implementación completa
   public async Task AjustarStockTienda(Guid idVariante, int nuevaCantidad)
   {
       if (nuevaCantidad < 0) throw new ArgumentException("El stock no puede ser negativo");
       await _varianteDA.AjustarStockTienda(idVariante, nuevaCantidad);
   }
   ```
   Nunca reutilizar este método para tocar `StockVirtual` — son dos stored procedures distintos
   (`AjustarStockTienda` / `AjustarStockVirtual`), ver `/crear-estructura-bd`.

4. **Descuento automático al confirmar un pago (el caso más delicado):**
   - Ocurre **solo** dentro de `ConfirmarPagoPedido` (invocado desde el handler del webhook de la
     pasarela, `ADR-004`), nunca desde un endpoint que el frontend llame directamente.
   - Para cada `ItemPedido`: verificar `StockVirtual >= Cantidad` **antes** de descontar. Si algún
     ítem no alcanza, la transacción completa se revierte y el pedido **no** pasa a `Pagado` —
     reportar el conflicto (ver escenario de concurrencia abajo).
   - Todo el descuento + el cambio de `Pedido.Estado` ocurre en una **única transacción SQL**.
   - **Idempotencia:** antes de descontar, verificar si `IdTransaccionPasarela` ya fue procesado
     (columna única o tabla de control) — un webhook reintentado no debe descontar dos veces.

## Procedimiento — frontend

- El componente de inventario del panel admin (`perfumeria-admin/features/inventario`) **MUST**
  mostrar dos columnas o dos badges claramente etiquetados: "Tienda física" y "Tienda en línea" —
  nunca un solo número sin aclarar a qué canal pertenece.
- El carrito de `perfumeria-tienda` solo verifica/reserva contra `disponible` (derivado de
  `StockVirtual`) — nunca contra `StockTienda`.

## Escenario de concurrencia obligatorio (Constitution §2.5 / ADR-002 §3.1.6)
Al escribir la prueba de `ConfirmarPagoPedido`, incluir siempre: dos pagos aprobados simultáneamente
para la **última** unidad de `StockVirtual` de una variante — la prueba debe demostrar que solo uno
de los dos pedidos queda en `Pagado` y el otro recibe un estado de conflicto/error explícito, nunca
que ambos descuenten y el stock termine en negativo.

## Errores comunes a evitar
- Sumar `StockTienda + StockVirtual` en el backend y devolver solo ese total (pierde la información
  que el panel admin necesita).
- Descontar `StockVirtual` fuera de una transacción (deja al sistema en estado inconsistente si algo
  falla a mitad de camino).
- Confundir el endpoint de ajuste: llamar `stock-virtual` cuando la intención era `stock-tienda`.
