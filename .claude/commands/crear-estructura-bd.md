---
description: Crea el proyecto SQL Server Database Project (BD/) con tablas y stored procedures basados en docs/vision.md, incluyendo StockTienda/StockVirtual en toda tabla con inventario. Se ejecuta una sola vez, después de crear-estructura-proyecto.
allowed-tools: Bash, Read, Write, Edit, Glob
---

# Comando · Crear estructura de base de datos (BD/)

> ⚠️ **Delegación obligatoria:** este comando es de stack backend y **MUST** delegarse al
> subagente `programador-api` (vía Task tool) en vez de ejecutarse directamente en el contexto
> principal. Ver `.claude/agents/programador-api.md`.

## Objetivo
Generar `Perfumeria.Api/BD/BD.sqlproj` con tablas y stored procedures derivados de `docs/vision.md`.

## Contexto obligatorio
Leer `docs/vision.md` §3–§5 y `docs/adr/ADR-005-division-stock.md` antes de cualquier paso.

> ⛔ **Si `docs/vision.md` no existe o si ya existe `BD/BD.sqlproj`, detenerse y avisar.**

## Instrucciones

**Paso 1 — Generar GUID del proyecto:**
```powershell
[System.Guid]::NewGuid().ToString("B").ToUpper()
```

**Paso 2 — Crear `BD/BD.sqlproj`** (mismo esqueleto MSBuild que un SQL Server Database Project
estándar: `Sql160DatabaseSchemaProvider`, carpetas `dbo/Tables/` y `dbo/Stored Procedures/`).

**Paso 3 — Agregar el proyecto a la solución** (`dotnet sln add BD/BD.sqlproj`, o edición manual del
`.slnx` si el comando falla por limitaciones de SSDT).

**Paso 4 — Crear tablas en `BD/dbo/Tables/`** — una por entidad de `docs/vision.md` §3. Convenciones:

```sql
-- PK:              UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL
-- FK:               UNIQUEIDENTIFIER NOT NULL, CONSTRAINT [FK_<Tabla>_<Ref>] FOREIGN KEY (...) REFERENCES ...
-- Texto corto:       VARCHAR(100/200) NOT NULL
-- Texto libre:        TEXT NULL
-- Fechas:             DATETIME DEFAULT (getdate()) NOT NULL
-- Eliminación lógica:  Activo BIT DEFAULT ((1)) NOT NULL
-- Precio:              DECIMAL(10,2) NOT NULL, CHECK (Precio >= 0)
```

**Regla dura y específica de este dominio (ADR-005):** toda tabla que represente un producto con
inventario (`VariantesPerfume`, `Desodorantes`, `BodySprays`, `ShowerGeles`, `CremasCorporales`,
`Bodys`, `Estuches`) **MUST** tener **dos** columnas de stock, nunca una:

```sql
[StockTienda]  INT DEFAULT ((0)) NOT NULL, CONSTRAINT [CHK_<Tabla>_StockTienda] CHECK ([StockTienda] >= 0),
[StockVirtual] INT DEFAULT ((0)) NOT NULL, CONSTRAINT [CHK_<Tabla>_StockVirtual] CHECK ([StockVirtual] >= 0),
```

**Índices obligatorios** (dado el volumen esperado, `docs/vision.md` §4):
```sql
CREATE NONCLUSTERED INDEX [IX_<Tabla>_CodigoBarras] ON [dbo].[<Tabla>] ([CodigoBarras]);
CREATE NONCLUSTERED INDEX [IX_<Tabla>_IdMarca] ON [dbo].[<Tabla>] ([IdMarca]);
```

**Paso 5 — Crear stored procedures en `BD/dbo/Stored Procedures/`** — uno por endpoint de
`docs/vision.md` §5. Convenciones:

- `SET NOCOUNT ON;` al inicio.
- Escrituras dentro de `BEGIN TRANSACTION / COMMIT TRANSACTION`.
- Todo SP de escritura retorna el Id afectado.
- Eliminaciones son lógicas (`Activo = 0`), nunca `DELETE`.
- **`AjustarStockTienda` y `AjustarStockVirtual` son procedimientos separados** — nunca un único
  `AjustarStock` que reciba el canal como parámetro (ver `ADR-005` §3.1).
- **`ConfirmarPagoPedido`** (usado por el webhook, `ADR-004`) MUST: (1) verificar que
  `IdTransaccionPasarela` no fue procesado antes (idempotencia); (2) verificar `StockVirtual >=
  Cantidad` para cada `ItemPedido` antes de descontar; (3) descontar `StockVirtual` y actualizar
  `Pedido.Estado = 'Pagado'` dentro de la **misma** transacción.

**Paso 6 — Verificar integridad del `.sqlproj`** (cada `.sql` tiene su `<Build Include>`; tablas con
FK listadas después de las que referencian).

## Reglas duras
- Un archivo `.sql` por tabla y por stored procedure.
- Nunca lógica de negocio en los SP — solo persistencia y lectura.
- Los SP son la única interfaz entre `DA` (C#) y la BD — sin SQL inline en C#.
- Toda tabla de inventario tiene `StockTienda` y `StockVirtual` — nunca un campo `Stock` único.
- No insertar datos semilla (seeds) en este comando — es responsabilidad de un paso posterior.

## Salida esperada
- `BD/BD.sqlproj` completo, agregado a la solución.
- Un archivo `.sql` por tabla y por SP, incluyendo `ConfirmarPagoPedido` con su chequeo de
  idempotencia y de stock suficiente.
