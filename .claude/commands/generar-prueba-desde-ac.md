---
description: "Fase RED del ciclo TDD. Genera la prueba unitaria que falla desde un criterio de aceptación (AC), respetando la relación 1 a 1 y la nomenclatura del proyecto."
argument-hint: <US-###> <AC-##> <backend|frontend> <capa o feature>
allowed-tools: Read, Write, Edit, Bash, Grep, Glob
---

# Comando · Generar prueba desde AC (fase 🔴 RED)

> ⚠️ **Delegación obligatoria:** si el stack objetivo (`$ARGUMENTS`) es `backend`, este comando
> **MUST** delegarse al subagente `programador-api` (vía Task tool) en vez de ejecutarse
> directamente en el contexto principal. Si es `frontend`, delegar en `programador-spa-react`. Ver
> `.claude/agents/programador-api.md` y `.claude/agents/programador-spa-react.md`.

## Objetivo
Crear una prueba unitaria **que falla**, derivada directamente de un criterio de aceptación.

## Entradas (desde `$ARGUMENTS`)
- Historia de usuario (ej. `US-002`)
- Criterio de aceptación (ej. `AC-01`)
- Stack objetivo (`backend` o `frontend`)
- Capa (backend: `Flujo`, `DA`, `API`, `Servicios`) o feature (frontend: `catalogo`, `inventario`, etc.)

## Contexto obligatorio
- El archivo `docs/stories/<US>-*.md` — leer el AC textualmente.
- `docs/constitution.md` §2.2, §2.3, §2.6, §5.
- Estructura de proyectos de pruebas existente.

## Instrucciones

**Paso 0 — Verificar precondiciones:**
1. Backend: existe el `.sln`/`.slnx` y el `.csproj` de pruebas de la capa indicada, con xUnit +
   FluentAssertions + NSubstitute referenciados.
2. Frontend: existe `vitest.config`/config de Vitest en la SPA correspondiente.

> ⛔ **Si no existe la estructura, detenerse y avisar que primero debe correr
> `/crear-estructura-proyecto`, `/crear-estructura-bd` o `/crear-estructura-spa`.**

**Paso 1 — Leer el AC** en `docs/stories/` y extraer Dado/Cuando/Entonces completo.

**Paso 2 — Generar la prueba que falla:**
- Nombre: `Metodo_Escenario_ResultadoEsperado` (backend) / `metodo_escenario_resultadoEsperado`
  (frontend).
- Estructura AAA explícita con comentarios.
- Referencia al AC en comentario superior: `// AC-##: <título del escenario>`.
- **Backend:** `[Fact]`/`[Theory]`, `Substitute.For<I...>()`, aserciones con FluentAssertions.
- **Frontend:** `describe`/`it` en español, `renderHook`/`render`+`screen`+`userEvent`.
- **Si el AC es de concurrencia/idempotencia** (pago o stock, ver Constitution §2.5): el test
  **MUST** verificar interacción con las dependencias (`.Received(1)`, no solo el valor devuelto) y
  simular la segunda invocación/reintento explícitamente.

**Paso 3 — Crear stubs de la SUT** si no existe (todos los métodos lanzando
`NotImplementedException`) — sin esto el test falla en compilación, no en ejecución, y eso NO es
RED real.

**Paso 4 — Confirmar RED:** ejecutar `dotnet test --filter "<NombreTest>"` o
`npm run test -- <archivo>`. Debe fallar en ejecución (`NotImplementedException` o aserción), no en
compilación.

## Reglas duras
- Un solo AC por invocación. Sin placeholders — valores concretos.
- Sin `Thread.Sleep`, sin `[Ignore]`/`it.skip`.
- Prohibido `--passWithNoTests`.
- **Para la capa DA:** mockear `IRepositorioDapper` devolviendo un mock de `IDbConnection`; verificar
  con `.Received(1)` que `ObtenerRepositorio()` fue llamado.
- Un test que pasa con un valor hardcodeado sin usar las dependencias es inválido.

## Salida esperada
- Archivo de prueba que compila, falla al ejecutarse y referencia el AC.
- Comando para confirmar que falla.
