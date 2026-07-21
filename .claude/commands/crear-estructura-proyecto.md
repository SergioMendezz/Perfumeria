---
description: Crea la estructura completa del backend .NET (sln + csproj) de Perfumeria.Api en 5 capas (Abstracciones/API/Flujo/Servicios/DA), modelos e interfaces base. Se ejecuta una sola vez antes del primer ciclo TDD.
allowed-tools: Bash, Read, Write, Edit, Glob
---

# Comando · Crear estructura del proyecto backend

## Objetivo
Generar la solución .NET completa de `Perfumeria.Api` con las 5 capas de `ADR-003`, lista para
comenzar el ciclo TDD.

## Contexto obligatorio
Leer `docs/vision.md` y `docs/adr/ADR-003-arquitectura-backend.md` antes de ejecutar cualquier
comando.

> ⛔ **Si ya existe un `.sln`/`.slnx` en `Perfumeria.Api/`, detenerse y avisar al usuario.**

## Instrucciones

**Paso 1 — Verificar que no existe estructura previa.**

**Paso 2 — Crear solución y proyectos:**
```bash
cd Perfumeria.Api
dotnet new sln -n Perfumeria
dotnet new classlib -n Abstracciones -o Abstracciones
dotnet new webapi   -n API            -o API --no-https false
dotnet new classlib -n Flujo          -o Flujo
dotnet new classlib -n DA             -o DA
dotnet new classlib -n Servicios      -o Servicios
```

**Paso 3 — Proyectos de prueba:**
```bash
dotnet new xunit -n Abstracciones.Tests -o tests/Abstracciones.Tests
dotnet new xunit -n API.Tests           -o tests/API.Tests
dotnet new xunit -n Flujo.Tests         -o tests/Flujo.Tests
dotnet new xunit -n DA.Tests            -o tests/DA.Tests
dotnet new xunit -n Servicios.Tests     -o tests/Servicios.Tests
```

**Paso 4 — Agregar todos los proyectos a la solución** (`dotnet sln add ...` para cada uno).

**Paso 5 — Referencias entre proyectos:**
```bash
dotnet add Flujo reference Abstracciones
dotnet add DA reference Abstracciones
dotnet add Servicios reference Abstracciones
dotnet add API reference Abstracciones Flujo DA Servicios
dotnet add tests/Flujo.Tests reference Flujo Abstracciones
dotnet add tests/DA.Tests reference DA Abstracciones
dotnet add tests/API.Tests reference API Abstracciones
dotnet add tests/Servicios.Tests reference Servicios Abstracciones
```

**Paso 6 — Paquetes NuGet de prueba** (en cada `tests/*.Tests`):
```bash
dotnet add package FluentAssertions
dotnet add package NSubstitute
dotnet add package coverlet.collector
```

**Paso 7 — Paquetes de producción:**
```bash
dotnet add DA package Dapper
dotnet add DA package Microsoft.Data.SqlClient
dotnet add API package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add API package Swashbuckle.AspNetCore
dotnet add Servicios package BCrypt.Net-Next
```

**Paso 8 — Generar modelos base en `Abstracciones/Modelos/`** a partir de `docs/vision.md` §3: como
mínimo `Marca`, `Perfume`, `VariantePerfume`, `Usuario`, `Pedido`, `ItemPedido` (Base/Request/Response
por entidad, siguiendo el patrón documentado en `docs/vision.md` §3.2).

**Paso 9 — Generar interfaces en `Abstracciones/Interfaces/{API,Flujo,DA,Servicios}/`** — una por
capa y por entidad, siguiendo el patrón CRUD + `Activar` de `docs/vision.md` §5.

**Paso 10 — Crear `IRepositorioDapper`/`RepositorioDapper`** (DA) que expone `ObtenerRepositorio():
IDbConnection` vía `SqlConnection` desde la cadena de conexión `DefaultConnection`.

**Paso 11 — Configurar `Program.cs`** como composition root único: JWT Bearer (`ADR-003`), CORS para
los orígenes de `perfumeria-admin`/`perfumeria-tienda`, registro de todos los servicios por capa,
Swagger con seguridad Bearer.

**Paso 12 — `appsettings.json`:** sección `ConnectionStrings:DefaultConnection` y `Jwt:{Key,Issuer,
Audience,ExpiraHoras}` (con placeholders — nunca valores reales versionados).

**Paso 13 — Verificar que compila:** `dotnet build`.

## Reglas duras
- Nunca crear la estructura si ya existe un `.sln`/`.slnx`.
- Leer `docs/vision.md` para entidades y endpoints — no inventarlos.
- `Abstracciones` no depende de ningún otro proyecto propio.
- No crear una carpeta `Reglas` separada — las reglas de negocio puras van dentro de `Flujo`
  (`ADR-003`).
- Todo producto con inventario usa `StockTienda`/`StockVirtual`, nunca un campo `Stock` único.
- Ejecutar `dotnet build` al finalizar y reportar errores si los hay.

## Salida esperada
- Solución compilando sin errores.
- Modelos e interfaces listos para el primer ciclo `/generar-prueba-desde-ac`.
