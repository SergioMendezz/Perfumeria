# <Nombre del repositorio / producto>

> *Frase corta que describe el propósito del repositorio.*

**Estado:** 🟢 activo · **Versión:** 0.1 · **Última actualización:** YYYY-MM-DD

---

## 📌 Descripción

*Descripción de 3–5 líneas del propósito del repositorio.*

---

## 🏗️ Estructura del repositorio

```
<repo-name>/
├── CLAUDE.md            # Instrucciones siempre activas para Claude Code
├── GUIA-DE-AGENTES.md    # Guía de uso de agentes/comandos/skills
├── .claude/
│   ├── agents/
│   ├── commands/
│   └── skills/
├── .github/workflows/
├── docs/
│   ├── constitution.md
│   ├── vision.md
│   ├── adr/
│   ├── templates/
│   └── stories/
├── <Producto>.Api/
├── <producto>-admin/
└── <producto>-tienda/
```

---

## 🚀 Requisitos previos

- .NET SDK 8
- Node.js LTS
- SQL Server (local o contenedor)

---

## ⚡ Cómo empezar

```bash
git clone <url>
# Backend
cd <Producto>.Api/API && dotnet restore && dotnet run
# Frontend
cd <producto>-admin && npm ci && npm run dev
cd <producto>-tienda && npm ci && npm run dev
```

---

## 📚 Documentación

- **Visión del producto:** `docs/vision.md`
- **Decisiones (ADRs):** `docs/adr/`
- **Historias de usuario:** `docs/stories/`

---

## 🧪 Flujo de desarrollo

```
US → AC (GWT) → 🔴 RED → 🟢 GREEN → 🔵 REFACTOR → PR
```

---

## 🤖 Claude Code en este repositorio

Ver `GUIA-DE-AGENTES.md` para el catálogo completo de agentes, comandos y skills.

---

## 🔒 Seguridad

- Dependencias controladas por catálogo justificado con ADR.
- No se aceptan secretos en el repositorio.

---

## 📄 Licencia

Ver `LICENSE`.
