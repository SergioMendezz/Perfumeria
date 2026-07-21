---
adr_id: ADR-001
title: Adopción de modelo monorepo por producto
status: aceptado
date: 2026-07-21
authors: [Camila]
reviewers: [Por definir]
tags: [arquitectura, proceso, git, gobierno]
supersedes: []
superseded_by: []
---

# ADR-001 · Adopción de modelo monorepo por producto

## 1. Estado

`aceptado`.

---

## 2. Contexto

El material de gobernanza de referencia (`Template.API`, del curso) recomienda un modelo
**polirrepo**: un repositorio por componente (API, SPA, documentación). Sin embargo, el proyecto de
referencia estructural que se debe imitar — `PuraVidaFragance` — usa un **monorepo**: un único
repositorio con tres carpetas de primer nivel (`PuraVidaFragance.API`, `pura-vida-admin`,
`pura-vida-tienda`).

Perfumeria es un proyecto académico desarrollado por un equipo pequeño (posiblemente una sola
persona) con un único ciclo de release, no múltiples productos independientes. El escenario que
justifica el polirrepo (equipos separados, releases desacoplados, permisos por departamento) no
aplica aquí.

---

## 3. Decisión

Se adopta el **modelo monorepo por producto**, siguiendo la estructura de `PuraVidaFragance`.

### 3.1 Detalle de la decisión

- **Un solo repositorio** `Perfumeria` con tres carpetas de primer nivel:
  ```
  Perfumeria/
  ├── Perfumeria.Api/       ← solución .NET (Abstracciones/API/Flujo/DA/Servicios + BD)
  ├── perfumeria-admin/     ← SPA React + TypeScript (panel del personal)
  └── perfumeria-tienda/    ← SPA React + TypeScript (catálogo público + checkout)
  ```
- **Gobernanza centralizada** en la raíz: `CLAUDE.md`, `.claude/`, `docs/`, `.github/workflows/`.
- **Nomenclatura de proyectos internos:** `Perfumeria.<Capa>` para el backend;
  `perfumeria-admin` / `perfumeria-tienda` (kebab-case) para las SPA, igual que
  `pura-vida-admin` / `pura-vida-tienda`.

### 3.2 Alcance

- **Aplica a:** este proyecto y cualquier fork académico directo.
- **No aplica a:** una eventual escisión futura a producción con equipos separados por componente —
  en ese escenario se debe abrir un nuevo ADR reevaluando polirrepo.

---

## 4. Alternativas evaluadas

| Alternativa | Descripción | Pros | Contras | ¿Descartada por? |
|---|---|---|---|---|
| **A. Polirrepo por componente** (recomendación genérica del taller) | Un repo por API/SPA/Doc | Aislamiento total, permisos granulares | Sobrecarga de gestión para un equipo pequeño; no coincide con el proyecto de referencia que se debe imitar | Desalineado con el requisito explícito de imitar `PuraVidaFragance` |
| **B. Monorepo único** ✅ | Todo en un repositorio, 3 carpetas | Un solo `git clone`, un solo lugar para PRs, coincide con `PuraVidaFragance`, más simple para 1 equipo pequeño | Menor aislamiento de releases (aceptable a esta escala) | **Elegida** |
| **C. Monorepo con tooling (Nx/Turborepo)** | Monorepo con orquestador de build | Cacheo de builds, tareas cruzadas | Complejidad de tooling injustificada para 3 proyectos pequeños | Excede la necesidad actual |

---

## 5. Consecuencias

### 5.1 Positivas
- Estructura idéntica a la del proyecto de referencia, facilitando comparar y aprender de él.
- Un solo lugar para la gobernanza (`CLAUDE.md`, `docs/`), sin duplicar `constitution.md` en 3 repos.
- Menor fricción para un equipo pequeño: un solo PR puede tocar API y SPA si el cambio lo requiere.

### 5.2 Negativas / Costos
- CI debe filtrar por carpeta modificada para no reconstruir todo en cada cambio (ver
  `.github/workflows/`).
- Si el proyecto creciera a producción con equipos separados, este ADR debería revisarse.

### 5.3 Neutrales / Observaciones
- El versionado semántico, si se necesitara, tendría que aplicarse por carpeta, no de forma global.

---

## 6. Cumplimiento y verificación

- Revisión manual: la estructura de carpetas de nivel raíz coincide con §3.1.
- CI (`.github/workflows/ci-api.yml`, `ci-frontend.yml`) usa `paths:` para evitar builds cruzados.

---

## 7. Excepciones

- Migrar a polirrepo requiere un nuevo ADR que documente el motivo (equipo, escala, o releases
  desacoplados) y sea aprobado explícitamente.

---

## 8. Referencias

- Proyecto de referencia estructural: `PuraVidaFragance` (compartido por la propietaria del proyecto).
- Martin Fowler — *MonorepoVsPolyrepo* (blog).
- GitHub Docs — *About repositories*.

---

## 9. Historial

| Fecha | Estado | Autor | Cambio |
|---|---|---|---|
| 2026-07-21 | aceptado | Camila | Versión inicial — reemplaza la recomendación genérica de polirrepo del taller por monorepo, alineado con `PuraVidaFragance`. |
