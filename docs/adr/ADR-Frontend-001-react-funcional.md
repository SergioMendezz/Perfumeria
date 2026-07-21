---
adr_id: ADR-Frontend-001
title: Adopción de React funcional con hooks y TypeScript estricto para ambas SPA
status: aceptado
date: 2026-07-21
authors: [Camila]
reviewers: [Por definir]
tags: [frontend, react, typescript, arquitectura]
supersedes: []
superseded_by: []
---

# ADR-Frontend-001 · React funcional + TypeScript estricto

## 1. Estado

`aceptado`.

---

## 2. Contexto

Perfumeria tiene dos SPA: `perfumeria-tienda` (catálogo público + checkout) y `perfumeria-admin`
(panel del personal). El proyecto de referencia estructural, `PuraVidaFragance`, implementó sus dos
SPA equivalentes (`pura-vida-tienda`, `pura-vida-admin`) en **JavaScript plano**, sin TypeScript ni
suite de pruebas visible.

El objetivo explícito de este proyecto es que quede "muy sólido" — lo que incluye poder confiar en
el compilador para detectar errores antes de producción, algo especialmente valioso en
`perfumeria-tienda` porque ahí vive el checkout con dinero real.

---

## 3. Decisión

Se adoptan, para **ambas** SPA:

1. **Componentes funcionales + hooks** como único paradigma permitido.
2. **TypeScript en modo estricto** para todos los archivos `.ts`/`.tsx`.

### 3.1 Detalle de la decisión

- Todo componente es una función. Prohibido `class extends React.Component`. Única excepción:
  `ErrorBoundary`.
- Prohibido `React.FC`: props tipados con `interface` en la firma de la función.
- Toda lógica reutilizable se extrae en custom hooks (prefijo `use`), por ejemplo
  `use-buscar-codigo-barras.ts`, `use-carrito.ts`.

`tsconfig.json` mínimo obligatorio:

```json
{
  "compilerOptions": {
    "strict": true,
    "noUncheckedIndexedAccess": true,
    "noImplicitReturns": true,
    "noUnusedLocals": true,
    "noUnusedParameters": true,
    "exactOptionalPropertyTypes": true,
    "forceConsistentCasingInFileNames": true
  }
}
```

- Prohibido `any` — usar `unknown` + estrechamiento.
- Estados asíncronos como discriminated unions (`inicial | cargando | exito | error`).

### 3.2 Alcance

- **Aplica a:** `perfumeria-tienda` y `perfumeria-admin`, ambas nuevas.
- **No aplica a:** el proyecto de referencia `PuraVidaFragance`, que no se modifica — solo se usa
  como inspiración de estructura, no de stack de lenguaje.

---

## 4. Alternativas evaluadas

| Alternativa | Descripción | Pros | Contras | ¿Descartada por? |
|---|---|---|---|---|
| A. Replicar JavaScript plano como `PuraVidaFragance` | Copiar el stack del proyecto de referencia tal cual | Menor curva de aprendizaje, coincide 100% con la referencia | Sin red de seguridad de tipos en un checkout con dinero real | Riesgo inaceptable dado el objetivo de "proyecto sólido" |
| B. Componentes de clase | Modelo tradicional | Familiar para OO | Ceremonia, no alineado con React moderno | Descartada — no es el modelo recomendado desde 2019 |
| C. Funcionales + hooks + TS estricto ✅ | Modelo moderno recomendado por el taller | Defectos capturados en compilación, refactor seguro, alineado con la Constitution | Mayor esfuerzo inicial de tipado | **Elegida** |

---

## 5. Consecuencias

### 5.1 Positivas
- El checkout y el carrito (datos monetarios) se benefician directamente de tipado estricto.
- Reutilización efectiva vía custom hooks (`use-carrito`, `use-buscar-codigo-barras`).
- Refactor con red de seguridad tipográfica al evolucionar el catálogo (nuevas categorías de producto).

### 5.2 Negativas / Costos
- Mayor tiempo inicial de tipado que copiar directamente el JavaScript de `PuraVidaFragance`.
- Requiere convertir mentalmente los ejemplos JS de la referencia a TS al consultarlos.

### 5.3 Neutrales / Observaciones
- La discriminated union para estados async exige manejo exhaustivo — el compilador lo obliga
  (positivo, no un costo real).

---

## 6. Cumplimiento y verificación

- `tsconfig.json` de ambos SPA con la configuración mínima de §3.1.
- `npx tsc --noEmit` sin errores como gate de CI.
- Revisión de código rechaza `class` (salvo `ErrorBoundary`) y `any`.

---

## 7. Excepciones

- `ErrorBoundary` — única clase permitida.
- Integración con el SDK/widget de la pasarela de pago (`ADR-004`) puede requerir `unknown` con
  casteo controlado si el SDK no publica tipos — nunca `any`, y debe documentarse en el archivo.

---

## 8. Referencias

- React Docs — *Introducing Hooks*.
- TypeScript Handbook — *strict mode*.
- Proyecto de referencia (solo estructura, no lenguaje): `PuraVidaFragance`.
- ADRs relacionados: `ADR-Frontend-002` (dependencias).

---

## 9. Historial

| Fecha | Estado | Autor | Cambio |
|---|---|---|---|
| 2026-07-21 | aceptado | Camila | Versión inicial. Documenta explícitamente la divergencia deliberada del stack de lenguaje respecto al proyecto de referencia. |
