# Visión del producto — Perfumeria

> Generado a partir del contexto entregado por la propietaria del proyecto (no por `/crear-vision`,
> ya que el dominio ya estaba definido). Cualquier cambio futuro a este documento debe pasar por el
> comando `/crear-vision` o por edición manual seguida de un incremento de versión en el historial (§10).

## 1. Descripción general

Sistema completo (API + 2 SPA) para una **perfumería** con venta híbrida: catálogo público sin
necesidad de registro, compra en línea con registro obligatorio del cliente, y un portal de
administración para el personal de la tienda. El catálogo distingue perfumes y una familia de
productos derivados (desodorantes, body sprays, shower gel, cremas corporales, bodys y estuches).

El sistema se compone de tres proyectos dentro de un mismo repositorio (ver `ADR-001`):

- **Perfumeria.Api** — API REST en ASP.NET Core 8, capas Abstracciones/API/Flujo/DA/Servicios.
- **perfumeria-tienda** — SPA en React + TypeScript, catálogo público + checkout de clientes.
- **perfumeria-admin** — SPA en React + TypeScript, portal de administración del personal.

## 2. Usuarios principales

| Rol | Qué hace en el sistema |
|-----|------------------------|
| Visitante (anónimo) | Navega el catálogo completo (perfumes y derivados) sin necesidad de registro |
| Cliente | Se registra únicamente para comprar; realiza pedidos y paga mediante la pasarela; consulta su historial de pedidos |
| Admin (personal de tienda) | CRUD de catálogo, inventario, usuarios del panel y visualización de clientes registrados |

> **Regla de seguridad (dato del negocio):** ver el catálogo NUNCA requiere autenticación. El
> registro/login solo se exige al iniciar el checkout, para reducir fricción y proteger el proceso
> de compra (evita cuentas de prueba/bots navegando el catálogo con sesión abierta).

## 3. Entidades principales

| Entidad | Descripción | Campos clave |
|---------|-------------|--------------|
| Marca | Marca comercial de un producto (transversal a todas las categorías) | Id (Guid), Nombre (string), PaisOrigen (string, opcional), Descripcion (string, opcional), LogoUrl (string, opcional), Activo (bool) |
| Perfume | Producto principal del catálogo | Id (Guid), IdMarca (Guid, FK), Nombre (string), CodigoBarras (string), Genero (string: Unisex\|Mujer\|Hombre), Categoria (string, ver §3.1), ImagenUrl (string), Descripcion (string, opcional), Activo (bool) |
| VariantePerfume | Presentación en mililitros de un perfume, con stock partido por canal | Id (Guid), IdPerfume (Guid, FK), Mililitros (decimal), Precio (decimal), StockTienda (int), StockVirtual (int), Activo (bool) |
| Desodorante | Derivado con posible perfume asociado | Id (Guid), IdMarca (Guid, FK), CodigoBarras (string), IdPerfumeDerivado (Guid, FK nullable), ImagenUrl (string), Nombre (string), Precio (decimal), StockTienda (int), StockVirtual (int), Activo (bool) |
| BodySpray | Derivado con posible perfume asociado | Id (Guid), IdMarca (Guid, FK), CodigoBarras (string), IdPerfumeDerivado (Guid, FK nullable), ImagenUrl (string), Nombre (string), Precio (decimal), StockTienda (int), StockVirtual (int), Activo (bool) |
| ShowerGel | Derivado, podría tener perfume asociado | Id (Guid), IdMarca (Guid, FK), CodigoBarras (string), IdPerfumeDerivado (Guid, FK nullable), ImagenUrl (string), Nombre (string), Precio (decimal), StockTienda (int), StockVirtual (int), Activo (bool) |
| CremaCorporal | Derivado, podría tener perfume asociado | Id (Guid), IdMarca (Guid, FK), CodigoBarras (string), IdPerfumeDerivado (Guid, FK nullable), ImagenUrl (string), Nombre (string), Precio (decimal), StockTienda (int), StockVirtual (int), Activo (bool) |
| Body | Derivado, podría tener perfume asociado | Id (Guid), IdMarca (Guid, FK), CodigoBarras (string), IdPerfumeDerivado (Guid, FK nullable), ImagenUrl (string), Nombre (string), Precio (decimal), StockTienda (int), StockVirtual (int), Activo (bool) |
| Estuche | Set/combo de dos o más productos de cualquier categoría anterior | Id (Guid), Nombre (string), ImagenUrl (string), Precio (decimal), StockTienda (int), StockVirtual (int), Activo (bool) |
| ItemEstuche | Línea que compone un Estuche | Id (Guid), IdEstuche (Guid, FK), TipoProducto (string: Perfume\|Desodorante\|BodySpray\|ShowerGel\|CremaCorporal\|Body), IdProducto (Guid), IdVariante (Guid, FK nullable — solo si TipoProducto = Perfume) |
| Usuario | Cuenta de acceso, tanto de personal como de clientes | Id (Guid), NombreUsuario (string), Email (string), PasswordHash (string), PasswordSalt (string), Rol (string: Admin\|Cliente), Activo (bool), FechaCreacion (DateTime), UltimoAcceso (DateTime, opcional) |
| Pedido | Compra realizada por un cliente | Id (Guid), IdCliente (Guid, FK), FechaPedido (DateTime), Estado (string: Pendiente\|Pagado\|Enviado\|Entregado\|Cancelado), Total (decimal), MetodoPago (string), IdTransaccionPasarela (string, opcional) |
| ItemPedido | Línea de un pedido | Id (Guid), IdPedido (Guid, FK), TipoProducto (string), IdProducto (Guid), IdVariante (Guid, FK nullable), Cantidad (int), PrecioUnitario (decimal) |

> ⚠️ **Supuesto explícito (no provisto por la propietaria, requerido para que el checkout y la
> pasarela de pago funcionen):** se agregaron `Pedido` e `ItemPedido`. Deben confirmarse o ajustarse
> con `/generar-historia-de-usuario` antes de implementarse.
>
> ⚠️ **Supuesto explícito:** el encabezado "Estuches" del contexto original no traía campos propios;
> se modeló como combo de productos existentes (`Estuche` + `ItemEstuche`), análogo al módulo `Set`
> de PuraVidaFragance. Ajustar si la intención era otra.

### 3.1 Categorías de perfume (nombre completo, no abreviado)

| Categoría | Nombre a mostrar (NUNCA la sigla) |
|---|---|
| EDP | Eau de Parfum |
| EDT | Eau de Toilette |
| EDC | Eau de Cologne |
| Extracto | Extrait de Parfum |
| Agua fresca | Eau Fraîche |

**Regla dura:** la UI y las respuestas de la API **MUST** mostrar siempre el nombre completo
(`Eau de Parfum`), nunca la sigla (`EDP`). La sigla puede usarse como valor interno de búsqueda,
nunca como texto visible.

### 3.2 Modelos de transferencia (ejemplo — Perfume)

| Modelo | Campos |
|--------|--------|
| PerfumeRequest | IdMarca (Guid), Nombre (string), CodigoBarras (string), Genero (string), Categoria (string), ImagenUrl (string), Descripcion (string, opcional) |
| PerfumeResponse | Id (Guid), Marca (string), Nombre (string), CodigoBarras (string), Genero (string), Categoria (string), ImagenUrl (string), Descripcion (string), Activo (bool), Variantes (VarianteResponse[]) |

El mismo patrón Base/Request/Response aplica a Desodorante, BodySpray, ShowerGel, CremaCorporal,
Body y Estuche.

## 4. Reglas de negocio generales

- El catálogo (listar y ver detalle de cualquier categoría) es **público**; ningún endpoint de
  lectura de catálogo requiere token.
- Todo endpoint de escritura (crear/editar/eliminar/activar) sobre catálogo, marcas o usuarios del
  panel requiere rol `Admin`.
- El registro de cuenta (`Rol = Cliente`) es público (auto-registro). El registro de cuentas
  `Admin` **MUST NOT** ser público — solo un Admin existente puede crear otra cuenta Admin.
- Un `Desodorante`, `BodySpray`, `ShowerGel`, `CremaCorporal` o `Body` puede tener un
  `IdPerfumeDerivado` opcional; cuando existe, la ficha del producto **MUST** enlazar/redirigir a la
  ficha del perfume asociado en la tienda.
- El código de barras es único por producto dentro de su propia tabla (no necesariamente único entre
  categorías distintas). Debe indexarse para búsqueda rápida por número completo o por los últimos
  N dígitos (ver Skill `codigo-barras`).
- El stock se divide siempre en `StockTienda` (inventario físico, ajuste manual desde el panel) y
  `StockVirtual` (el que consume la tienda en línea). Solo `StockVirtual` se descuenta al confirmar
  un pedido pagado (ver `ADR-005`).
- Los campos `Nombre` y `Marca` de cualquier producto **MUST** normalizarse a mayúscula inicial tras
  cada espacio al crear/editar (ver Skill `nomenclatura-productos`).
- Eliminar un producto, marca o variante es una eliminación **lógica** (`Activo = false`), nunca un
  `DELETE` físico.
- Todo pedido pagado exitosamente **MUST** quedar asociado a un `IdTransaccionPasarela` devuelto por
  la pasarela de pago (ver `ADR-004`); ningún pedido cambia a `Pagado` sin ese identificador.
- El volumen esperado de catálogo es del orden de ~2 millones de unidades de inventario total
  (agregando `StockTienda` + `StockVirtual` de todas las variantes) — no 2 millones de productos
  distintos. Esto exige: índices sobre `CodigoBarras`, `IdMarca` y `Categoria`; paginación
  obligatoria en todo endpoint de listado (`?pagina=&tamano=`); nunca `SELECT *` sin `TOP`/paginado
  desde Dapper.

## 5. Endpoints (Perfumeria.Api)

| Método | Ruta | Descripción | Acceso |
|--------|------|-------------|--------|
| GET | /api/perfume | Lista perfumes (paginado, filtros género/categoría/marca) | Público |
| GET | /api/perfume/{id} | Detalle de un perfume con sus variantes | Público |
| GET | /api/perfume/buscar-codigo-barras?codigo= | Busca por código completo o parcial (últimos dígitos) | Admin |
| POST | /api/perfume | Agrega un perfume | Admin |
| PUT | /api/perfume/{id} | Edita un perfume | Admin |
| DELETE | /api/perfume/{id} | Elimina lógicamente (Activo=false) | Admin |
| PATCH | /api/perfume/{id}/activar | Reactiva un perfume | Admin |
| GET / POST / PUT / DELETE / PATCH | /api/desodorante, /api/bodyspray, /api/showergel, /api/cremacorporal, /api/body | Mismo patrón que Perfume | Público (GET) / Admin (resto) |
| GET / POST / PUT / DELETE / PATCH | /api/estuche | CRUD de combos + gestión de `ItemEstuche` | Público (GET) / Admin (resto) |
| GET / POST / PUT / DELETE / PATCH | /api/marca | CRUD de marcas | Público (GET) / Admin (resto) |
| POST | /api/variante | Agrega variante (ml/precio/stock) a un perfume | Admin |
| PUT / PATCH | /api/variante/{id}/stock-tienda, /api/variante/{id}/stock-virtual | Ajuste independiente de cada canal de stock | Admin |
| POST | /api/auth/registro-cliente | Auto-registro de cliente | Público |
| POST | /api/auth/login | Login (staff o cliente) | Público |
| POST | /api/auth/logout | Cierre de sesión (revoca token) | Autenticado |
| POST | /api/auth/recuperar-contrasena | Solicita enlace de recuperación de contraseña | Público |
| POST | /api/auth/restablecer-contrasena | Aplica nueva contraseña con token de recuperación | Público (con token) |
| GET / POST / PUT / PATCH | /api/usuario | CRUD de cuentas `Admin` del panel (nunca autocreación) | Admin |
| GET | /api/cliente | Lista clientes registrados (solo lectura) | Admin |
| POST | /api/pedido | Crea un pedido a partir del carrito | Cliente |
| GET | /api/pedido/mis-pedidos | Historial de pedidos del cliente autenticado | Cliente |
| GET | /api/pedido/{id} | Detalle de un pedido | Cliente (dueño) o Admin |
| POST | /api/pago/iniciar | Inicia el cobro con la pasarela para un pedido | Cliente |
| POST | /api/pago/webhook | Webhook de confirmación de la pasarela (server-to-server) | Pasarela (firma verificada, sin JWT) |

> ⚠️ Endpoints exactos de checkout/pago sujetos a refinamiento vía historias de usuario — ver
> `ADR-004` para el contrato del webhook.

## 6. Arquitectura del proyecto

Ver `docs/adr/ADR-003-arquitectura-backend.md` para el detalle de capas y la regla de dependencia.

```
Perfumeria.Api/
├── Abstracciones/
│   ├── Modelos/            → PerfumeBase/Request/Response, VarianteBase/..., etc.
│   └── Interfaces/{API,Flujo,DA,Servicios}/
├── API/Controllers/        → PerfumeController, DesodoranteController, ..., PagoController
├── Flujo/                  → orquestación + reglas de negocio (sin capa Reglas separada, ADR-003)
├── DA/                     → PerfumeDA, ... (Dapper + Stored Procedures)
├── Servicios/               → JwtService, PasarelaPagoService
└── BD/                      → BD.sqlproj (Tables/ + Stored Procedures/)
```

### SPA — perfumeria-tienda (catálogo público + checkout)
```
src/
├── core/                 # config, tipos globales
├── features/
│   ├── catalogo/          # componentes/hooks/services/types/views por categoría
│   ├── carrito/
│   ├── checkout/           # integra la pasarela de pago
│   └── cuenta/             # registro/login/historial de pedidos del cliente
├── shared/{components,hooks}
├── router.ts
└── main.tsx
```

### SPA — perfumeria-admin (panel del personal)
```
src/
├── core/
├── features/
│   ├── inventario/         # incluye búsqueda por código de barras + lectura de pistola
│   ├── catalogo/            # CRUD perfumes/derivados/estuches/marcas
│   ├── usuarios/            # CRUD de cuentas Admin
│   └── clientes/            # solo lectura
├── shared/{components,hooks}
├── router.ts
└── main.tsx
```

## 7. Fuera de alcance (versión actual)

- Integración con un sistema de punto de venta (POS) físico — `StockTienda` se ajusta manualmente.
- Facturación electrónica ante Hacienda (Costa Rica) — se documenta como trabajo futuro, no como
  parte de esta versión.
- Programas de fidelización / cupones / descuentos.
- Multi-idioma y multi-moneda (se asume colones costarricenses, ₡).
- Aplicación móvil nativa.

## 8. Matriz de permisos

El API usa **JWT propio** (sin servicio de seguridad externo) con `BCrypt` para contraseñas y el rol
como claim de texto (`ClaimTypes.Role`), igual que el patrón de referencia (ver `ADR-003`).

### Roles definidos
| Rol | Valor del claim | Descripción |
|-----|------------------|-------------|
| Admin | `"Admin"` | Personal de la tienda. Acceso completo a catálogo, inventario, usuarios y clientes. |
| Cliente | `"Cliente"` | Comprador registrado. Acceso a su propio carrito, pedidos y datos. |

### Permisos por endpoint (resumen — ver §5 para el detalle completo)
| Método | Ruta | Rol requerido | Anotación C# |
|--------|------|----------------|--------------|
| GET | /api/{categoria} | — (público) | `[AllowAnonymous]` |
| POST/PUT/DELETE/PATCH | /api/{categoria} | Admin | `[Authorize(Roles = "Admin")]` |
| GET | /api/perfume/buscar-codigo-barras | Admin | `[Authorize(Roles = "Admin")]` |
| POST | /api/auth/registro-cliente | — (público) | `[AllowAnonymous]` |
| POST | /api/pedido | Cliente | `[Authorize(Roles = "Cliente")]` |
| GET | /api/cliente | Admin | `[Authorize(Roles = "Admin")]` |
| POST | /api/pago/webhook | — (firma de la pasarela, no JWT) | `[AllowAnonymous]` + validación de firma |

> Todos los endpoints protegidos devuelven `401` sin token válido y `403` con rol incorrecto.

## 9. Cumplimiento legal (Costa Rica)

Ver `docs/adr/ADR-006-cumplimiento-legal-cr.md` para el detalle y las referencias legales. Resumen
de obligaciones que la tienda en línea **MUST** cumplir:

- Términos y condiciones visibles, incluyendo plazo de entrega y derecho de retracto de 8 días
  hábiles (Ley 7472 y su reglamento, Capítulo X — comercio electrónico).
- Política de privacidad y consentimiento expreso para tratamiento de datos personales
  (Ley 8968 — Protección de la Persona frente al tratamiento de sus datos personales).
- Comprobante de pago/confirmación de pedido al cliente tras cada transacción exitosa.
- Mecanismo gratuito y visible para quejas/reclamos, con plazos de respuesta indicados.
- Precio final visible en colones (₡) antes de confirmar el pago.

> ⚠️ Este documento no constituye asesoría legal. Antes de publicar la tienda, un profesional en
> Derecho debe validar los textos legales del sitio.

## 10. Historial

| Versión | Fecha | Cambios |
|---------|-------|---------|
| 0.1 | 2026-07-21 | Versión inicial — generada a partir del contexto entregado para el proyecto "Perfumeria" (nombre provisional). Incluye supuestos marcados con ⚠️ pendientes de confirmar. |
