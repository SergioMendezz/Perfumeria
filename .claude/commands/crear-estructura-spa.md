---
description: Crea la estructura completa de una de las dos SPA (perfumeria-tienda o perfumeria-admin) con Vite + React + TypeScript estricto + Vitest y arquitectura feature-based. Se ejecuta una vez por cada SPA, antes del primer ciclo TDD frontend de esa SPA.
argument-hint: <tienda|admin>
allowed-tools: Bash, Read, Write, Edit, Glob
---

# Comando · Crear estructura de una SPA

## Objetivo
Generar `perfumeria-tienda/` o `perfumeria-admin/` (según `$ARGUMENTS`) con Vite + React + TypeScript
estricto, Vitest y arquitectura feature-based.

## Contexto obligatorio
- `docs/vision.md` — features requeridas según la SPA indicada (§6).
- `docs/adr/ADR-Frontend-001-react-funcional.md`, `ADR-Frontend-002-lista-blanca-dependencias.md`.

> ⛔ **Si `$ARGUMENTS` no es `tienda` ni `admin`, preguntar cuál de las dos SPA se quiere crear.**
> ⛔ **Si ya existe `package.json` en la carpeta correspondiente, detenerse y avisar.**

## Instrucciones

**Paso 1 — Crear proyecto Vite + React + TypeScript:**
```bash
npx create-vite@latest perfumeria-<tienda|admin> --template react-swc-ts
cd perfumeria-<tienda|admin>
npm install
```

**Paso 2 — Instalar Vitest y testing-library (versiones exactas, sin `^` ni `~`):**
```bash
npm install --save-exact vitest @testing-library/react @testing-library/user-event @testing-library/jest-dom jsdom @vitest/coverage-v8
npm install --save-exact react-router-dom
npm install --save-exact -D tailwindcss @tailwindcss/vite
```

**Paso 3 — `tsconfig.json` estricto** (`ADR-Frontend-001` §3.1 — `strict`,
`noUncheckedIndexedAccess`, `noImplicitReturns`, `noUnusedLocals`, `noUnusedParameters`,
`exactOptionalPropertyTypes`, `forceConsistentCasingInFileNames`).

**Paso 4 — `vite.config.ts`** con el plugin de Tailwind v4 como segundo plugin (después de
`react()`) y configuración de Vitest (`globals: true`, `environment: 'jsdom'`,
`setupFiles: './src/core/setup-tests.ts'`).

**Paso 5 — `package.json` scripts:** `dev`, `build` (`tsc && vite build`), `test`
(`vitest run`), `test:watch`, `test:coverage`, `lint` (`tsc --noEmit`).

**Paso 6 — Estructura feature-based** (`docs/constitution.md` §4.6):

- Si `$ARGUMENTS = tienda`: `features/{catalogo,carrito,checkout,cuenta}`.
- Si `$ARGUMENTS = admin`: `features/{inventario,catalogo,usuarios,clientes}`.

```
src/
├── core/{setup-tests.ts, types.ts}
├── features/<las de arriba>/{components,hooks,services,types,views}
├── shared/{components,hooks}
├── router.ts
└── main.tsx
```

**Paso 7 — `src/core/setup-tests.ts`:**
```ts
import '@testing-library/jest-dom'
```

**Paso 8 — `.env.development`:**
```
VITE_API_URL=http://localhost:5140/api
```

**Paso 9 — Verificar:** `npm run lint && npm run test`.

## Reglas duras
- Sin dependencias fuera de `ADR-Frontend-002` — versiones exactas.
- Sin `any`, sin `React.FC`, sin librerías de estado global prohibidas.
- HTTP solo vía `useRecurso` (se crea en el ciclo TDD, no en este comando).
- Nombres en español; `use`/`maneje`/`al`.
- Leer `docs/vision.md` para las features — no inventarlas.

## Salida esperada
- Proyecto compilando (`npm run lint` sin errores) y `npm run test` en verde.
- Estructura feature-based creada según la SPA indicada.
- Mensaje con los comandos para continuar: `npm run dev` y `/generar-prueba-desde-ac`.
