---
description: Genera una historia de usuario completa (US) con criterios INVEST y AC en formato GWT, siguiendo docs/templates/US-template.md y el flujo SDD+TDD.
argument-hint: <nombre-corto-kebab-case> <rol: Visitante|Cliente|Admin> <capacidad requerida> <beneficio de negocio>
allowed-tools: Read, Write, Glob
---

# Comando · Generar historia de usuario (US)

> ⚠️ **Delegación obligatoria:** este comando **MUST** delegarse al subagente
> `analista-requisitos` (vía Task tool) en vez de ejecutarse directamente en el contexto principal.
> Ver `.claude/agents/analista-requisitos.md`.

## Objetivo
Crear `docs/stories/US-###-<nombre>.md` como punto de partida ejecutable para el ciclo SDD+TDD.

## Entradas (parseadas desde `$ARGUMENTS`)
- **Nombre corto de la historia** (kebab-case, ej. `agregar-variante-perfume`).
- **Rol del usuario** (`Visitante`, `Cliente` o `Admin` — ver `docs/vision.md` §2).
- **Capacidad requerida** — qué necesita poder hacer.
- **Beneficio de negocio** — por qué lo necesita.

Si `$ARGUMENTS` no trae los 4 datos, preguntar los que falten antes de continuar — no inventarlos.

## Contexto obligatorio (leer antes de generar, en este orden)
1. `docs/templates/US-template.md` — estructura exacta.
2. `docs/vision.md` — entidades, reglas de negocio, endpoints ya definidos para este rol.
3. Listar `docs/stories/` para determinar el siguiente `US-###`.

> ⛔ **Prohibido generar el archivo sin haber leído el template y `docs/vision.md` primero.**

## Instrucciones

**Paso 1 — Completar el frontmatter:** `id`, `title`, `status: borrador`, `owner: Por definir`,
`size` (inferido), `priority` (inferida del rol/impacto), `related_epic`, `tags`, `version: 0.1`,
`last-reviewed: <fecha actual>`.

**Paso 2 — Completar cada sección:**
- §1 Historia: `Como / Quiero / Con el fin de`.
- §2 Valor de negocio: ≥ 2 viñetas concretas.
- §3 INVEST: marcar `[x]` los que cumplen; justificar los que no.
- §4 AC en GWT: ≥ 3 escenarios (éxito + validaciones). **Si la historia toca `Pedido`, pago o
  cualquier campo `Stock*`, agregar un AC de concurrencia/idempotencia** (Constitution §2.5).
- §5 Restricciones y supuestos: ≥ 1 de cada.
- §6 Fuera de alcance: ≥ 2 ítems.
- §7 Dependencias.
- §8 Notas de refinamiento: preguntas abiertas.
- §9 Historial: entrada de creación.

**Paso 3 — Revisar contra el checklist del agente `analista-requisitos`** antes de reportar como
completa (ver `.claude/agents/analista-requisitos.md` §3).

## Reglas duras
- Leer siempre el template y `docs/vision.md` antes de generar.
- Idioma español.
- No omitir ninguna de las 9 secciones sin justificar.
- Cada AC verificable objetivamente — nunca "el sistema responde correctamente".
- La US no contiene diseño técnico — expresa valor de negocio.

## Salida esperada
Archivo `docs/stories/US-###-<nombre>.md` creado, completo y revisado contra el checklist.
