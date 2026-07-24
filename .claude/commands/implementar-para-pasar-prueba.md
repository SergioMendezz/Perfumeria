---
description: "Fase GREEN del ciclo TDD. Genera el código mínimo que hace pasar una prueba unitaria previamente escrita en fase RED, respetando las 5 capas y SOLID."
argument-hint: <ruta del archivo de prueba> <nombre del test específico> <backend|frontend>
allowed-tools: Read, Write, Edit, Bash, Grep, Glob
---

# Comando · Implementar para pasar la prueba (fase 🟢 GREEN)

> ⚠️ **Delegación obligatoria:** si el stack (`$ARGUMENTS`) es `backend`, este comando **MUST**
> delegarse al subagente `programador-api` (vía Task tool) en vez de ejecutarse directamente en el
> contexto principal. Si es `frontend`, delegar en `programador-spa-react`. Ver
> `.claude/agents/programador-api.md` y `.claude/agents/programador-spa-react.md`.

## Objetivo
Escribir el **código mínimo necesario** para que una prueba que actualmente falla pase, sin agregar
funcionalidad no cubierta por la prueba (YAGNI).

## Entradas (desde `$ARGUMENTS`)
- Archivo de prueba objetivo.
- Nombre del test específico que debe pasar.
- Stack (`backend`/`frontend`).

## Contexto obligatorio
- `docs/constitution.md` §2.2, §3, §4, §6, §7.
- `docs/adr/ADR-003-arquitectura-backend.md` (backend) / `ADR-Frontend-001` (frontend).
- Código existente en la capa/feature destino para reutilizar patrones.

## Instrucciones

- **Leer el archivo de prueba** para entender: la SUT y su firma esperada, las dependencias
  sustituidas (mocks), el resultado observable esperado y el AC referenciado.
- **Determinar la capa/archivo a crear o modificar:**
  - **Backend:** regla de negocio u orquestación → `Flujo/`; controller → `API/Controllers/`;
    adaptador externo (JWT, pasarela de pago) → `Servicios/`; acceso a datos → `DA/`
    (**si el SP referenciado no existe en `BD/dbo/Stored Procedures/`, crearlo y registrarlo en
    `BD.sqlproj` antes de continuar** — el SP debe existir antes de que la implementación C# se
    considere completa); interfaces/DTOs → `Abstracciones/`.
  - **Frontend:** dentro del feature correspondiente (`hooks/`, `services/`, `components/`).
- **Escribir solo el código necesario** para que el test pase — no agregar métodos/casos que la
  prueba no cubre; no optimizar prematuramente (el refactor es una fase separada).
- **Respetar la arquitectura de 5 capas (Constitution §3):** `Flujo` no hace I/O directo a BD; `API`
  no contiene lógica de negocio; `DA` no contiene lógica de negocio ni SQL inline.
- **Respetar SOLID:** inyección por constructor; depender de interfaces.
- **Frontend:** componente función; sin `any`; props con `interface`; estados async como
  discriminated union; HTTP vía `useRecurso`.
- **Ejecutar la suite** afectada y confirmar que el test pasa.
- **Mostrar al final:** archivos creados/modificados, resultado del test (✅), sugerencias de
  refactor pendientes para la fase 🔵.

## Reglas duras
- No modificar la prueba para que pase — la prueba es la especificación.
- No agregar comentarios "TODO" ni código muerto.
- Nombres en español; sin números mágicos; métodos ≤ 20 líneas.
- Sin dependencias fuera del catálogo justificado.
- **Solo modificar archivos que la prueba referencia directamente.** Si la implementación requiere
  tocar archivos fuera de la capa destino (`Program.cs`, `router.ts`), detenerse y pedir un ciclo
  RED→GREEN específico para ese cambio.
- **Para operaciones de stock o pago:** el método debe validar `StockVirtual >= Cantidad` antes de
  descontar, y debe operar dentro de una transacción — nunca retornar un valor hardcodeado sin
  persistir de verdad.
- **Antes de declarar GREEN:** ¿la implementación usa todas las dependencias que el test configura?
  Si no, el test es insuficiente y debe corregirse antes de implementar.

## Salida esperada
- Código productivo mínimo en la capa correcta.
- Confirmación de que la prueba pasa + sugerencias de refactor.
