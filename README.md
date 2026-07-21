# Perfumeria

> Tienda de perfumería con catálogo público, checkout con pasarela de pago costarricense e
> inventario dividido entre tienda física y virtual. Nombre de producto provisional — ver
> `docs/vision.md` para renombrarlo.

**Estado:** 🟡 en gobernanza — sin código todavía · **Versión:** 0.1 · **Última actualización:** 2026-07-21

---

## 📌 Descripción

Este repositorio contiene el **andamiaje de gobernanza para Claude Code** de Perfumeria: reglas,
agentes, comandos, skills, ADRs y documentación de dominio ya completados. Todavía **no** contiene
código de producción — está diseñado para que Claude Code (u otra persona del equipo) construya
`Perfumeria.Api`, `perfumeria-admin` y `perfumeria-tienda` siguiendo el ciclo SDD+TDD documentado
en `docs/constitution.md`.

Si es tu primera vez acá, empezá por `GUIA-DE-AGENTES.md` — ahí está explicado, con ejemplos, cómo
usar cada agente, comando y skill sin tener que memorizar toda la Constitution.

---

## 🏗️ Estructura del repositorio

```
Perfumeria/
├── CLAUDE.md                 # Instrucciones siempre activas para Claude Code
├── GUIA-DE-AGENTES.md          # Guía de uso — empezar por acá
├── .mcp.json                   # Catálogo de servidores MCP autorizados (vacío por ahora)
├── .claude/
│   ├── settings.json            # Hooks locales de gobernanza
│   ├── agents/                  # 5 subagentes especializados
│   ├── commands/                 # 10 comandos (/generar-historia-de-usuario, etc.)
│   └── skills/                    # 4 skills de dominio (se activan solos)
├── .github/workflows/            # CI de la API y de ambas SPA
├── docs/
│   ├── constitution.md            # Fuente de verdad de todas las reglas
│   ├── vision.md                  # Dominio completo: entidades, endpoints, permisos
│   ├── adr/                       # 8 decisiones arquitectónicas documentadas
│   ├── templates/                 # Plantillas para nuevos ADR/US/README
│   └── stories/                   # Historias de usuario de ejemplo (US-001, US-002)
├── Perfumeria.Api/                # (a crear con /crear-estructura-proyecto)
├── perfumeria-admin/               # (a crear con /crear-estructura-spa admin)
└── perfumeria-tienda/               # (a crear con /crear-estructura-spa tienda)
```

---

## 🚀 Requisitos previos

- .NET SDK 8
- Node.js LTS + npm
- SQL Server (local, contenedor, o Azure SQL) para el proyecto `BD/`
- Claude Code (CLI, extensión de VS Code/JetBrains, o la app de escritorio)

---

## ⚡ Cómo empezar

1. Abrí este repositorio con Claude Code.
2. Leé `docs/vision.md` — el dominio ya está definido, con algunos supuestos marcados con ⚠️ que
   conviene confirmar primero.
3. Corré, en este orden:
   ```
   /crear-estructura-proyecto
   /crear-estructura-bd
   /crear-estructura-spa tienda
   /crear-estructura-spa admin
   ```
4. Para cada funcionalidad nueva:
   ```
   /generar-historia-de-usuario
   /generar-prueba-desde-ac
   /implementar-para-pasar-prueba
   ```
5. Una vez que exista un login: `/seguridad-asegurar-api` y `/seguridad-asegurar-spa`.

Ver `GUIA-DE-AGENTES.md` para el detalle completo de cada paso.

---

## 📚 Documentación

- **Reglas siempre activas:** `CLAUDE.md`
- **Fuente de verdad completa:** `docs/constitution.md`
- **Dominio del negocio:** `docs/vision.md`
- **Decisiones arquitectónicas:** `docs/adr/` (8 ADR, incluyendo pasarela de pago, stock dual y
  cumplimiento legal costarricense)
- **Historias de usuario:** `docs/stories/`

---

## 🧪 Flujo de desarrollo

```
docs/vision.md → US (GWT) → 🔴 RED → 🟢 GREEN → 🔵 REFACTOR → PR
```

Ningún código de producción existe sin una prueba que lo respalde (`docs/constitution.md` §2).

---

## 🤖 Claude Code en este repositorio

- **Agentes** (`.claude/agents/`): analista-requisitos, programador-api, programador-spa-react,
  seguridad-programador-api, seguridad-programador-spa.
- **Comandos** (`.claude/commands/`): 10 comandos, ver `GUIA-DE-AGENTES.md` para el catálogo.
- **Skills** (`.claude/skills/`): nomenclatura-productos, control-stock-dual, codigo-barras,
  pasarela-pago-cr — se activan solos según el contexto de la tarea.

---

## 🔒 Seguridad y cumplimiento

- Dependencias controladas por lista blanca con ADR (`docs/adr/ADR-Frontend-002-*.md`).
- Nunca se capturan datos de tarjeta en formularios propios (`docs/adr/ADR-004-*.md`).
- Cumplimiento legal costarricense documentado en `docs/adr/ADR-006-*.md` — **requiere revisión de
  un profesional en Derecho antes de publicar el sitio**.

---

## 🧭 Convenciones

- **Ramas:** `feature/<US-###>-<descripcion>`, `bugfix/<descripcion>`, `hotfix/<descripcion>`.
- **Commits:** Conventional Commits en imperativo (`feat: agregar búsqueda por código de barras`).
- **Nomenclatura de código:** ver `CLAUDE.md` y `docs/constitution.md` §5.

---

## 📝 Historial de cambios

Ver el historial (§10/§9/§14 según el archivo) dentro de `docs/vision.md`, cada ADR y
`docs/constitution.md`.
