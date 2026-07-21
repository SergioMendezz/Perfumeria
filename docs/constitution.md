---
title: Constitution — Principios Arquitectónicos de Perfumeria
type: constitution
scope: academico
version: 0.1
status: activo
last-reviewed: 2026-07-21
authors: [Camila]
reviewers: [Por definir]
supersedes: []
related_adrs: [ADR-001, ADR-002, ADR-003, ADR-Frontend-001, ADR-Frontend-002, ADR-004, ADR-005, ADR-006]
immutability: alta
audience: [Claude Code, Agents, Skills, Commands, Desarrolladores, Docente del curso]
---

# 📜 Constitution — Principios Arquitectónicos de Perfumeria

> **Propósito:** este documento contiene los **principios arquitectónicos inmutables** que gobiernan
> toda generación de código asistida por IA (Claude Code) y todo desarrollo humano dentro del
> alcance del proyecto Perfumeria.
>
> **Regla suprema:** ningún prompt, agente, skill, hook, comando o práctica puede violar los
> principios aquí definidos. Si una necesidad concreta lo requiere, debe tramitarse como un nuevo
> ADR que supersede formalmente el principio en cuestión.
>
> **Nota de origen:** este documento adapta la Constitution del taller de Diseño y Desarrollo de
> Sistemas (basada en `Template.API`) al contexto específico de una tienda de perfumería con
> pasarela de pago, inventario dual y cumplimiento legal costarricense. La estructura concreta del
> repositorio sigue el patrón observado en el proyecto de referencia `PuraVidaFragance` (ver
> `ADR-001` y `ADR-003`).

---

## 0. Preámbulo — cómo leer este documento

- Cada principio se enuncia con la fórmula **"MUST" / "MUST NOT" / "SHOULD" / "SHOULD NOT"**
  (convención RFC 2119).
- Los principios están **numerados** para referenciarlos en `CLAUDE.md`, agentes, comandos, skills y
  ADRs (`Constitution §3.2`, etc.).
- Este documento es **inmutable en su intención**. Los ajustes se hacen creando un ADR que lo
  modifica o supersede.
- Está escrito para ser **consumido por Claude Code como contexto de generación** (referenciado desde
  `CLAUDE.md`). Por eso mantiene estructura simple, oraciones cortas y ejemplos concretos.

---

## 1. Principios rectores generales

### 1.1 Estándares de industria
El código y la documentación **MUST** basarse en estándares reconocidos (Microsoft Learn, Clean
Architecture, SOLID, C4, ADR, TDD Red-Green-Refactor, React docs). Toda excepción **MUST**
justificarse en un ADR.

### 1.2 Fundamentación explícita
Toda decisión relevante **MUST** referenciar un estándar externo o una política interna vigente.
Las decisiones "porque sí" están prohibidas.

### 1.3 Documento como artefacto ejecutable
Todo documento del ciclo de vida (US, AC, ADR, vision) **MUST** alimentar directamente pruebas,
comandos, skills o revisión de código. Se prohíbe el "documento de vitrina".

### 1.4 Único punto de verdad
La fuente primaria de la especificación **MUST** residir en la carpeta `docs/` del repositorio,
versionada en Git. `CLAUDE.md` es la puerta de entrada que apunta aquí, no una fuente primaria
duplicada.

### 1.5 Mínimo suficiente
Solo se documenta y se implementa lo que aporta valor accionable. La sobre-documentación y el
over-engineering **MUST NOT** aceptarse.

### 1.6 Trazabilidad end-to-end
La cadena **US → AC → Prueba → Código → Commit → PR** **MUST** ser reconstruible en todo momento.

### 1.7 Dependencias mínimas
Toda dependencia externa **MUST** estar en el catálogo de librerías justificadas con ADR
(`ADR-Frontend-002`). Las excepciones **MUST** aprobarse mediante nuevo ADR.

---

## 2. Principios del flujo de desarrollo (SDD + TDD)

### 2.1 Spec antes que código
Ningún cambio productivo **MUST** iniciarse sin una historia de usuario (US) con criterios de
aceptación (AC) en formato GWT (`Dado / Cuando / Entonces`). Únicas excepciones: cambios cosméticos
puros (formato, comentarios, renombres).

### 2.2 Test antes que implementación (TDD)
Ciclo obligatorio: **🔴 RED → 🟢 GREEN → 🔵 REFACTOR**.

1. **RED** — escribir la prueba que falla derivada del AC.
2. **GREEN** — escribir el código mínimo que hace pasar la prueba.
3. **REFACTOR** — aplicar SOLID + Clean Code sin romper las pruebas.

Ningún código productivo **MUST** existir sin una prueba que lo respalde.

### 2.3 Relación 1 a 1 entre AC, pruebas y código
- 1 AC **MUST** tener 1 o más pruebas unitarias.
- 1 prueba unitaria **MUST** validar 1 unidad pública de código.

### 2.4 Cobertura mínima
El proyecto **MUST** definir cobertura mínima en CI. **MUST NOT** usarse `--passWithNoTests` ni
equivalentes.

### 2.5 Escenarios mínimos obligatorios por US
Toda historia de usuario **MUST** cubrir con AC los escenarios que apliquen: éxito, validación de
entrada, autenticación/autorización, no encontrado/vacío, duplicidad (p. ej. código de barras
repetido), fallo de dependencia externa (p. ej. la pasarela de pago no responde).

### 2.6 Nomenclatura de pruebas
`Metodo_Escenario_ResultadoEsperado`.

### 2.7 Trazabilidad en el commit y PR
Todo commit o PR **MUST** referenciar el AC (`AC-##`) y la historia de usuario (`US-###`).

---

## 3. Principios de arquitectura backend (C# API)

### 3.1 Arquitectura por capas (5 capas — ver ADR-003)
Toda solución backend **MUST** organizarse en 5 capas con responsabilidades separadas. A diferencia
del taller genérico, Perfumeria **no** usa una capa `Reglas` independiente: las reglas de negocio
puras viven dentro de `Flujo`, siguiendo el patrón validado en `PuraVidaFragance` (ver `ADR-003` para
la justificación completa y las alternativas evaluadas).

| Capa | Responsabilidad única |
|---|---|
| **Abstracciones** | Modelos + interfaces. **MUST NOT** contener implementación. |
| **API** | Recibir HTTP, validar shape, delegar a Flujo, componer dependencias (composition root). **MUST NOT** contener lógica de negocio. |
| **Flujo** | Orquestar el caso de uso Y aplicar las reglas de negocio puras. **MUST NOT** hacer I/O directo a BD (delega en DA). |
| **Servicios** | Adaptadores a servicios externos (JWT, pasarela de pago). |
| **DA (AccesoDatos)** | Acceso a base de datos vía Dapper + Stored Procedures. **MUST NOT** contener lógica de negocio ni SQL inline (todo vía SP). |

### 3.2 Regla de dependencia (Dependency Rule)
Las dependencias **MUST** apuntar siempre hacia adentro: hacia `Abstracciones`. Ninguna capa **MUST**
depender de detalles concretos de otra capa; solo de interfaces publicadas en `Abstracciones`.

### 3.3 Composition Root único
La composición de dependencias (DI container) **MUST** realizarse exclusivamente en `API/Program.cs`.

### 3.4 Un proyecto de pruebas por capa
Cada capa **MUST** tener su propio proyecto de pruebas espejado en `tests/`.

### 3.5 Controllers "thin"
Los controllers **MUST** limitarse a: recibir la petición HTTP, validar el shape del payload,
invocar al Flujo y mapear la respuesta.

### 3.6 Nomenclatura de proyectos
`Perfumeria.Abstracciones`, `Perfumeria.Api`, `Perfumeria.Flujo`, `Perfumeria.DA`,
`Perfumeria.Servicios` (namespaces internos: `Abstracciones`, `API`, `Flujo`, `DA`, `Servicios`, tal
como en el proyecto de referencia).

---

## 4. Principios de arquitectura frontend (2 SPA React + TypeScript)

Perfumeria tiene **dos** SPA independientes que comparten las mismas reglas duras:
`perfumeria-tienda` (catálogo público + checkout) y `perfumeria-admin` (panel del personal).

### 4.1 Solo componentes funcionales + hooks
Todo componente **MUST** ser una función. `class extends React.Component` **MUST NOT** usarse.
Única excepción: `ErrorBoundary`.

### 4.2 Toda lógica reutilizable en custom hooks
Prefijo `use` obligatorio.

### 4.3 TypeScript estricto
`tsconfig.json` **MUST** activar como mínimo: `strict`, `noUncheckedIndexedAccess`,
`noImplicitReturns`, `noUnusedLocals`, `noUnusedParameters`, `exactOptionalPropertyTypes`,
`forceConsistentCasingInFileNames`. `any` **MUST NOT** usarse.

> **Nota de gobernanza vs. referencia:** el proyecto de referencia `PuraVidaFragance` está escrito en
> JavaScript plano y usa `axios`. Perfumeria adopta deliberadamente el estándar más estricto del
> taller (TypeScript + sin `axios`) porque el objetivo explícito es un proyecto "sólido" — ver
> `ADR-Frontend-001` y `ADR-Frontend-002` para el razonamiento y las alternativas evaluadas.

### 4.4 Tipado de props
`interface` en la firma de la función. `React.FC` **MUST NOT** usarse.

### 4.5 Estados asíncronos como discriminated unions
`inicial | cargando | exito | error`, con manejo exhaustivo obligado por TypeScript.

### 4.6 Estructura feature-based
Por dominio funcional (`catalogo`, `carrito`, `checkout`, `cuenta`, `inventario`, `usuarios`,
`clientes`), no por tipo de archivo.

### 4.7 HTTP, formularios y grids
- HTTP **MUST** hacerse vía el custom hook `useRecurso<T>()` del proyecto (fetch nativo).
- Formularios **MUST** usar `useState` + `useReducer` nativos.
- `axios`, `fetch` directo en componentes, `formik`, `react-hook-form` **MUST NOT** usarse.

### 4.8 Estado global
`useState` + `useReducer` + `useContext`. Librerías externas de estado **MUST NOT** usarse sin ADR
de excepción (el carrito de compras se modela con `useReducer` + `useContext`, ver Skill
`control-stock-dual` para la interacción con el stock virtual).

### 4.9 Tamaño de componente
Un archivo, ~150 líneas máximo.

### 4.10 Dependencias del SPA
Solo del catálogo de librerías justificadas (`ADR-Frontend-002`). Versiones exactas (sin `^`/`~`).

---

## 5. Principios de nomenclatura y estilo

### 5.1 Idioma
Identificadores de dominio en **español**. Palabras reservadas de lenguaje/frameworks en su idioma
original.

### 5.2 Nomenclatura frontend

| Elemento | Convención |
|---|---|
| Archivos de componente | PascalCase (`ListaPerfumes.tsx`) |
| Hooks, services, utils | kebab-case (`use-buscar-codigo-barras.ts`) |
| Custom hooks | prefijo `use` obligatorio |
| Props booleanas | prefijo `es`, `tiene`, `debe` |
| Handlers internos | prefijo `maneje` |
| Props de callback | prefijo `al` (`alGuardar`, `alEscanear`) |
| Constantes globales | UPPER_SNAKE_CASE |

### 5.3 Nomenclatura backend

| Elemento | Convención |
|---|---|
| Clases | PascalCase en español |
| Interfaces | Prefijo `I` + PascalCase |
| Variables locales | camelCase |
| Pruebas | `Metodo_Escenario_ResultadoEsperado` |

### 5.4 Nomenclatura de datos de producto (regla de negocio, no solo estilo)
Los campos `Nombre` y `Marca` de cualquier producto **MUST** normalizarse a mayúscula inicial tras
cada espacio (ver Skill `nomenclatura-productos`). Esta regla aplica tanto en el formulario del panel
admin como en la validación del backend (defensa en profundidad).

### 5.5 Commits
**Conventional Commits** en imperativo (`feat: agregar búsqueda por código de barras`). Tipos
válidos: `feat`, `fix`, `refactor`, `docs`, `test`, `chore`, `perf`, `hotfix`.

---

## 6. Principios SOLID

Cada PR **MUST** verificar SRP, OCP, LSP, ISP, DIP como parte del refactor.

---

## 7. Principios de Clean Code

Legibilidad primero, nombres descriptivos, métodos ≤ 20 líneas, sin números mágicos, ≤ 2 niveles de
anidamiento, sin código muerto, comentarios que explican el *por qué*.

---

## 8. Principios de seguridad

### 8.1 Sin secretos en el repositorio
Credenciales, claves JWT, cadenas de conexión y llaves de la pasarela de pago **MUST NOT** vivir en
el código ni en configuración versionada. Usar variables de entorno / secret manager.

### 8.2 Validación de entradas
Toda entrada externa **MUST** validarse cerca del borde del sistema (DataAnnotations en
Abstracciones + validación de shape en el Controller).

### 8.3 Principio de menor privilegio
Todo endpoint es protegido por defecto (`[Authorize]` a nivel de clase); `[AllowAnonymous]` solo con
justificación explícita documentada en el propio controller.

### 8.4 Cadena de suministro
Solo dependencias del catálogo justificado. Escaneo de vulnerabilidades (SCA) obligatorio en CI.

### 8.5 Pasarela de pago (dato sensible)
El backend **MUST NOT** almacenar números de tarjeta, CVV ni datos sensibles de pago (alcance
PCI SAQ-A). Toda captura de datos de tarjeta ocurre en el checkout hospedado o el SDK de la pasarela,
nunca en formularios propios. Ver `ADR-004`.

### 8.6 MCP con guardarraíles
Los servidores MCP conectados **MUST** estar en el catálogo autorizado (`.mcp.json`) y operar en modo
*read-only* por defecto salvo aprobación explícita.

---

## 9. Principios de personalización de Claude Code

### 9.1 Instructions siempre activas
El repositorio **MUST** incluir un `CLAUDE.md` en la raíz con las reglas mínimas siempre activas.
Debe mantenerse conciso; el detalle vive en `docs/constitution.md` (§1.4).

### 9.2 Comandos como macros gobernadas
Los comandos (`.claude/commands/`) **MUST** declarar frontmatter (`description`, `argument-hint`,
`allowed-tools` cuando aplique). Un comando **MUST** tener un único objetivo homogéneo.

### 9.3 Agentes con límites explícitos
Los subagentes (`.claude/agents/`) **MUST** documentar: comandos ejecutables, testing, estructura,
estilo, git workflow y límites ("never touch").

### 9.4 Skills como procedimientos gobernados
Los Skills (`.claude/skills/<nombre>/SKILL.md`) **MUST** documentar un procedimiento reutilizable y
acotado (p. ej. normalizar nombres, dividir stock, integrar la pasarela). Claude los invoca
automáticamente cuando el contexto de la tarea coincide con su descripción — a diferencia de un
comando, no requieren invocación explícita del usuario.

### 9.5 Workflows con revisión humana
Los workflows (hooks locales en `.claude/settings.json` y CI en `.github/workflows/`) **MUST NOT**
hacer merge ni deploy sin validación humana. Todo hook que bloquee una acción **MUST** explicar por
qué en su mensaje de salida.

### 9.6 MCP bajo aprobación explícita
Ningún servidor MCP **MUST** conectarse a sistemas productivos (BD real, cuenta real de la pasarela)
sin aprobación explícita y sin estar en `.mcp.json`.

---

## 10. Principios de gobierno y ciclo de vida

### 10.1 Cambios a la Constitution
Solo mediante un ADR que la referencie explícitamente.

### 10.2 Ciclo de vida de artefactos
Todo artefacto (Instructions, Comandos, Agentes, Skills, MCP, Workflows, Templates) **MUST** pasar
por: propuesta → documentación → feedback → aprobación → publicación → revisión periódica →
deprecación con ADR de sustitución si aplica.

### 10.3 Revisión periódica
Al menos una vez por hito del curso.

### 10.4 Excepciones documentadas
Toda excepción a un principio **MUST** documentarse como ADR.

---

## 11. Verificación y cumplimiento

| Nivel | Mecanismo |
|---|---|
| Generación | `CLAUDE.md` + agentes + comandos + skills alimentados por esta Constitution |
| Local | Hooks de Claude Code (`.claude/settings.json`), linter, formatter |
| Continuo | CI (`.github/workflows/`): build, test, cobertura, SCA |
| Humano | Revisión de código obligatoria por al menos 1 par (o autorrevisión documentada en proyectos individuales) |

---

## 12. Resumen ejecutivo

1. 📜 **Especificar antes de codificar** (US + AC en GWT).
2. 🧪 **Probar antes de implementar** (Red-Green-Refactor).
3. 🔗 **Trazar de punta a punta** (US → AC → Test → Código → PR).
4. 🏛️ **5 capas en backend** (sin capa Reglas separada — ADR-003).
5. ⚛️ **Solo funciones + hooks + TypeScript estricto** en ambas SPA.
6. 📦 **Solo dependencias del catálogo justificado**.
7. 🏗️ **SOLID en cada refactor**.
8. 🧹 **Clean Code sin excepciones**.
9. 🔒 **Seguridad y PCI SAQ-A como principio no negociable en pagos**.
10. 📊 **Stock dual (tienda/virtual) como regla de negocio central**, no un detalle técnico.
11. ⚖️ **Cumplimiento legal costarricense (Ley 7472, Ley 8968)** no es opcional.
12. 🤖 **Claude Code gobernado por CLAUDE.md, Agentes, Comandos, Skills y Workflows**.
13. 🗳️ **Excepciones solo vía ADR**.

---

## 13. Referencias

- **ADRs vigentes:** ver frontmatter de este documento.
- **Documento de dominio:** `docs/vision.md`.
- **Estándares externos:** Robert C. Martin (Clean Architecture, Clean Code, SOLID), Kent Beck (TDD),
  Martin Fowler (Refactoring), React docs, TypeScript Handbook, Michael Nygard (ADR), OWASP.
- **Legal (Costa Rica):** Ley N.° 7472 y su Reglamento (Decreto N.° 37899-MEIC, Capítulo X — reforma
  vía Decreto N.° 40703-MEIC), Ley N.° 8968 de Protección de la Persona frente al tratamiento de sus
  datos personales.
- **Proyecto de referencia estructural:** `PuraVidaFragance` (repositorio compartido por la
  propietaria del proyecto).

---

## 14. Historial

| Versión | Fecha | Autor | Cambios |
|---|---|---|---|
| 0.1 | 2026-07-21 | Camila | Versión inicial, adaptada de la Constitution del taller (`Template.API`) al dominio de Perfumeria. |

---

*Constitution versión 0.1 · Proyecto Perfumeria · Este documento gobierna sobre CLAUDE.md, agentes,
comandos, skills, hooks y workflows.*
