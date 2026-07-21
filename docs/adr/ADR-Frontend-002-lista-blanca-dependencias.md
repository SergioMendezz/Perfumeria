---
adr_id: ADR-Frontend-002
title: Lista blanca de dependencias para ambas SPA
status: aceptado
date: 2026-07-21
authors: [Camila]
reviewers: [Por definir]
tags: [frontend, seguridad, dependencias, gobernanza, critico]
supersedes: []
superseded_by: []
priority: critica
---

# ADR-Frontend-002 · Lista blanca de dependencias

## 1. Estado

`aceptado` — prioridad crítica.

---

## 2. Contexto

`perfumeria-tienda` procesa datos de checkout y se comunica con una pasarela de pago real. Cada
dependencia externa es superficie de ataque adicional en un flujo donde ya hay dinero real de por
medio. El proyecto de referencia (`PuraVidaFragance`) usa `axios` para todas sus llamadas HTTP; este
proyecto adopta el estándar más estricto del taller: nada de librerías HTTP de terceros.

---

## 3. Decisión

Se adopta una lista blanca de dependencias para ambas SPA.

### 3.1 Dependencias permitidas

**Runtime:**

| Categoría | Paquete | Justificación |
|---|---|---|
| Framework | `react`, `react-dom` | Plataforma base |
| Lenguaje | `typescript` | `ADR-Frontend-001` |
| Ruteo | `react-router-dom` | Excepción documentada — necesaria para SPA multi-vista (igual que en la referencia) |
| SDK de la pasarela de pago | El SDK oficial de ONVO Pay o Tilopay (ver `ADR-004`) | Único caso de librería de un proveedor externo permitida — es la vía recomendada por el propio proveedor para no manejar datos de tarjeta directamente (PCI SAQ-A) |

**Toolchain:**

| Categoría | Paquete | Justificación |
|---|---|---|
| Build | `vite` | Toolchain del taller |
| Testing | `vitest`, `@testing-library/react` | TDD (`ADR-002`) |
| Estilos | `tailwindcss` (v4, vía `@tailwindcss/vite`) | Utilidades atómicas, sin conflicto con las reglas duras |

### 3.2 Dependencias prohibidas sin aprobación explícita

- HTTP: `axios`, `ky`, `superagent` → usar `useRecurso<T>()` con `fetch` nativo.
- Utilidades: `lodash`, `underscore`, `ramda` → APIs nativas de ES2022+.
- Fechas: `moment`, `date-fns`, `dayjs` → `Intl.DateTimeFormat`.
- Estado global: `redux`, `zustand`, `mobx`, `recoil`, `jotai` → `useState` + `useReducer` +
  `useContext` (el carrito de compras se modela así, ver Skill `control-stock-dual`).
- Formularios: `formik`, `react-hook-form` → `useState` + `useReducer` nativo.
- Validación: `yup`, `zod`, `joi` → funciones puras + TypeScript.
- CSS utilities: `classnames`, `clsx` → template literals.

### 3.3 Reglas de `package.json`

- Versiones exactas (sin `^` ni `~`).
- `package-lock.json` versionado.
- Sin dependencias fuera de esta lista.

### 3.4 Alcance

- **Aplica a:** `perfumeria-tienda` y `perfumeria-admin`.
- **No aplica a:** dependencias transitivas heredadas de paquetes autorizados.

---

## 4. Alternativas evaluadas

| Alternativa | Descripción | Pros | Contras | ¿Descartada por? |
|---|---|---|---|---|
| A. Libertad total (como la referencia, con `axios`) | Cada quien decide | Coincide con `PuraVidaFragance` | Superficie de ataque mayor en un flujo de pago real | Riesgo inaceptable dado el dinero real en juego |
| B. Lista negra | Prohibir paquetes conocidos | Menos fricción | No escalable | Enfoque reactivo insuficiente |
| C. Lista blanca ✅ | Solo lo aprobado | Superficie mínima, gobierno claro | Requiere mantener el catálogo | **Elegida** |

---

## 5. Consecuencias

### 5.1 Positivas
- Superficie de ataque mínima en el flujo de checkout.
- Bundle más liviano.
- El único paquete de terceros permitido (SDK de la pasarela) tiene una justificación de seguridad
  explícita, no de conveniencia.

### 5.2 Negativas / Costos
- Mayor esfuerzo para implementar `useRecurso<T>()` propio en lugar de usar `axios` directamente.
- Requiere mantenimiento activo de esta lista si el proyecto crece.

---

## 6. Cumplimiento y verificación

- Hook local (`.claude/settings.json`) o revisión de PR que escanea `package.json` e imports en
  busca de paquetes no autorizados.
- Escaneo de vulnerabilidades (SCA) obligatorio en CI antes de cualquier release.

---

## 7. Excepciones

1. Se identifica la necesidad de una dependencia no listada.
2. Se documenta en un PR a este ADR: problema que resuelve, alternativas nativas evaluadas, por qué
   no aplican, y el SDK/paquete concreto propuesto.
3. Aprobación antes de mergear el PR que la introduce.

---

## 8. Referencias

- Cloud Security Alliance — *Software Supply Chain Security*.
- OpenSSF Scorecard.
- MDN Web Docs — `fetch`, `Intl`, `structuredClone`.
- ADRs relacionados: `ADR-Frontend-001`, `ADR-004` (pasarela de pago).

---

## 9. Historial

| Fecha | Estado | Autor | Cambio |
|---|---|---|---|
| 2026-07-21 | aceptado | Camila | Versión inicial. Agrega el SDK de la pasarela como única excepción de proveedor externo permitida. |
