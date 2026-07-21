# Guía de uso — Agentes, Comandos, Skills y demás (Perfumeria)

Esta guía existe para que puedas usar todo el andamiaje de gobernanza de este repositorio **sin
tener que memorizar nada** — si en algún momento no te acordás cómo se usa una parte, volvé a este
archivo. Está escrita pensando en que quizás es la primera vez que trabajás con Claude Code de esta
forma tan estructurada.

---

## 0. ¿Qué es todo esto y por qué existe?

Cuando le pedís código a una IA sin ningún tipo de estructura, es fácil terminar con: campos
inventados que nunca pediste, código sin pruebas, dependencias que no necesitabas, o reglas de
negocio que la IA "asumió" mal (por ejemplo, mezclar el stock de la tienda física con el de la
tienda en línea). Este repositorio existe para evitar eso: cada pieza de gobernanza (Instructions,
Prompts/Comandos, Agents, Skills, MCP, Workflows, Templates, Documentación) tiene un rol específico
para que Claude Code siempre tenga el contexto correcto antes de escribir una línea de código.

La tabla que te dio tu profesor (los "8 elementos de personalización") mapea así a este proyecto:

| # | Elemento (según tu profesor) | Analogía | Dónde vive en este repo | Equivalente en Claude Code |
|---|---|---|---|---|
| 1 | Instructions | Reglas siempre activas (constitución del país) | `CLAUDE.md` (resumen) + `docs/constitution.md` (completo) | `CLAUDE.md` se carga automáticamente en cada sesión |
| 2 | Prompts | Atajo de teclado (comando puntual) | `.claude/commands/*.md` | Comandos slash: `/generar-historia-de-usuario`, etc. |
| 3 | Agents | Persona con rol (piloto, copiloto) | `.claude/agents/*.md` | Subagentes — Claude los despacha para una tarea específica |
| 4 | Skills | Manual de procedimiento (receta) | `.claude/skills/<nombre>/SKILL.md` | Se activan solos, sin que vos los invoques |
| 5 | MCP | Cable de conexión a otros sistemas | `.mcp.json` | Servidores externos conectados a Claude Code |
| 6 | Workflows | Sensor automático (dispara ante evento) | `.claude/settings.json` (hooks locales) + `.github/workflows/` (CI) | Hooks de Claude Code + GitHub Actions |
| 7 | Templates | Molde de partida | `docs/templates/*.md` | Markdown que los comandos leen antes de generar un archivo nuevo |
| 8 | Documentación MD | Memoria del equipo | `docs/constitution.md`, `docs/vision.md`, `docs/adr/`, `docs/stories/` | Markdown en `docs/`, es la fuente de verdad de todo lo demás |

**Regla más importante de toda la guía:** si alguna vez Claude Code (o vos) no sabe algo sobre el
proyecto, la respuesta correcta es "vamos a `docs/vision.md` o pedimos que se aclare" — nunca
"asumamos algo razonable". Eso está codificado en `CLAUDE.md` y en cada agente.

---

## 1. Instructions — `CLAUDE.md` y `docs/constitution.md`

**Qué son:** reglas que Claude Code lee automáticamente en cada sesión, sin que vos hagas nada.

- `CLAUDE.md` (en la raíz) es el resumen corto — se carga siempre.
- `docs/constitution.md` es el detalle completo — `CLAUDE.md` lo referencia, y Claude lo abre cuando
  necesita el razonamiento completo detrás de una regla.

**Cuándo tocarlos:** casi nunca de forma directa. Si una regla necesita cambiar, no se edita el
archivo a mano — se crea un nuevo ADR que la reemplaza (ver §8). De hecho, hay un hook
(`.claude/settings.json`) que **bloquea** la edición directa de `docs/constitution.md` y de
`docs/adr/*.md` para forzar ese proceso.

**Ejemplo de uso:** no hace falta "usar" esto activamente — simplemente está ahí. Si querés
verificar que Claude las está respetando, podés preguntarle directamente: *"¿qué dice la
Constitution sobre el stock dual?"*

---

## 2. Prompts / Comandos — `.claude/commands/`

**Qué son:** "atajos de teclado". Escribís `/nombre-del-comando` en el chat de Claude Code y se
ejecuta una tarea puntual con instrucciones ya definidas — como pedirle a alguien que siga una
receta exacta en vez de improvisar.

**Cómo se usan:** escribí `/` en el chat y Claude Code te va a mostrar los comandos disponibles.
Elegí el que corresponda y completá los datos que te pida (o escribilos directamente después del
comando, ej. `/generar-historia-de-usuario agregar-marca Admin "crear una marca nueva" "..."`).

### Catálogo completo

| Comando | Cuándo usarlo | Se usa... |
|---|---|---|
| `/crear-vision [sección]` | Solo si `docs/vision.md` necesita cambios (agregar una entidad, corregir un supuesto ⚠️) | Rara vez — el dominio ya está definido |
| `/generar-historia-de-usuario` | Para crear una US completa y formal, con INVEST y varios AC | Antes de una funcionalidad mediana o grande |
| `/generar-historia` | Versión rápida, para una tarea chica dentro de una US ya existente | Para ajustes pequeños |
| `/crear-estructura-proyecto` | **Una sola vez**, para generar el esqueleto de `Perfumeria.Api` | Al principio del proyecto |
| `/crear-estructura-bd` | **Una sola vez**, después de `/crear-estructura-proyecto` | Al principio del proyecto |
| `/crear-estructura-spa tienda` / `admin` | **Una vez por cada SPA** | Al principio del proyecto |
| `/generar-prueba-desde-ac` | Fase 🔴 RED — escribir la prueba que falla | Cada vez que empezás a implementar un AC |
| `/implementar-para-pasar-prueba` | Fase 🟢 GREEN — código mínimo para pasar la prueba | Justo después del RED |
| `/seguridad-asegurar-api` | Agregar JWT + roles al backend | Una vez que existan los primeros endpoints |
| `/seguridad-asegurar-spa tienda` / `admin` | Agregar login/guards a una SPA | Una vez que exista el login del backend |

---

## 3. Agents — `.claude/agents/`

**Qué son:** "personas con un rol". Cada agente tiene una personalidad, un conjunto de comandos que
sabe usar, límites claros de lo que **no** debe hacer, y conoce las reglas específicas de su parte
del proyecto. Se los invoca cuando el trabajo es lo suficientemente grande como para beneficiarse de
ese enfoque especializado (a diferencia de un comando, que es una tarea puntual).

**Cómo se usan:** podés pedirle a Claude directamente *"usá el agente programador-api para
implementar el endpoint de búsqueda por código de barras"*, o simplemente describir la tarea — Claude
Code puede despachar el subagente automáticamente si detecta que encaja con su descripción.

### Catálogo completo

| Agente | Rol | Ejemplo de cuándo invocarlo |
|---|---|---|
| `analista-requisitos` | Crea y refina historias de usuario | *"Necesito refinar la US-002 antes de implementarla"* |
| `programador-api` | Backend C# .NET, 5 capas, TDD | *"Implementá el endpoint de ajuste de StockTienda"* |
| `programador-spa-react` | Ambas SPA, React + TypeScript | *"Creá la vista de catálogo de perfumes"* |
| `seguridad-programador-api` | JWT propio + webhook de pago | *"Asegurá el endpoint del webhook de ONVO Pay"* |
| `seguridad-programador-spa` | Login, guards, checkout seguro | *"Agregá el login a perfumeria-admin"* |

**Diferencia clave con un comando:** un comando (`/generar-prueba-desde-ac`) hace *una* tarea puntual
con pasos fijos. Un agente mantiene un rol completo durante una conversación más larga y decide *qué*
comandos usar y en qué orden, dentro de sus límites documentados.

---

## 4. Skills — `.claude/skills/`

**Qué son:** "manuales de procedimiento" — a diferencia de los comandos, **no los invocás vos
escribiendo algo**. Claude los detecta y aplica solo cuando la tarea coincide con la descripción del
skill. Son el lugar donde vive el conocimiento específico de este dominio que se repite en muchas
tareas distintas.

### Catálogo completo

| Skill | Se activa cuando... |
|---|---|
| `nomenclatura-productos` | Se crea/edita el `Nombre` o `Marca` de cualquier producto |
| `control-stock-dual` | Se toca cualquier cosa relacionada a `StockTienda`/`StockVirtual` |
| `codigo-barras` | Se trabaja en búsqueda de inventario o registro de `CodigoBarras` |
| `pasarela-pago-cr` | Se trabaja en checkout, pagos, o `PasarelaPagoService` |

**No necesitás hacer nada especial** para que se activen — si le pedís a Claude *"implementá el
formulario para crear un perfume"*, el skill `nomenclatura-productos` se aplica solo porque el
formulario toca el campo `Nombre`.

---

## 5. MCP — `.mcp.json`

**Qué es:** la forma en que Claude Code se conecta a herramientas o sistemas externos (por ejemplo,
GitHub, una base de datos, un servicio de diseño). Es como un "cable" que le da acceso a algo que no
tiene de forma nativa.

**Estado actual:** `.mcp.json` está vacío a propósito — no hay servidores MCP conectados todavía.
Por gobernanza (`docs/constitution.md` §8.6/§9.6), cualquier servidor que se agregue debe:
1. Operar en modo solo-lectura por defecto, salvo que necesites explícitamente que escriba.
2. Quedar registrado en este mismo archivo (nunca "conectado a mano" y olvidado).

**Ejemplo de cómo se vería agregar uno** (no está activo, es solo referencia si en algún momento
querés conectar, por ejemplo, un servidor de GitHub):

```json
{
  "mcpServers": {
    "github": {
      "command": "npx",
      "args": ["-y", "@modelcontextprotocol/server-github"],
      "env": { "GITHUB_PERSONAL_ACCESS_TOKEN": "tu-token-aqui" }
    }
  }
}
```

Si en algún momento necesitás un MCP (por ejemplo, para conectar directamente a la base de datos
SQL Server y que Claude pueda leer datos reales), pedíselo a Claude Code — te puede ayudar a buscar
el servidor adecuado en el directorio de conectores.

---

## 6. Workflows — hooks locales + CI

Este elemento tiene **dos partes** en este proyecto, porque cubren dos momentos distintos:

### 6.1 Hooks locales (`.claude/settings.json`)
Se ejecutan **mientras trabajás con Claude Code**, antes o después de que Claude use una
herramienta. En este proyecto hay uno configurado: bloquea que Claude edite directamente
`docs/constitution.md` o cualquier archivo en `docs/adr/` — si Claude (o vos) lo intenta, el hook lo
detiene y explica que hace falta un ADR nuevo en su lugar.

### 6.2 CI — Integración continua (`.github/workflows/`)
Se ejecutan **cuando subís cambios a GitHub** (en un Pull Request o al hacer push a `main`):
- `ci-api.yml` — compila el backend, corre las pruebas con cobertura, verifica formato, y escanea
  dependencias vulnerables.
- `ci-frontend.yml` — corre en ambas SPA (`perfumeria-tienda` y `perfumeria-admin`): verifica
  TypeScript, lint, pruebas con cobertura, **y rechaza el build si detecta una dependencia prohibida**
  como `axios` en `package.json` (ver `ADR-Frontend-002`).

**No necesitás correr esto manualmente** — se dispara solo. Si un PR falla el CI, el mensaje de error
te va a decir exactamente qué regla se incumplió.

---

## 7. Templates — `docs/templates/`

**Qué son:** el "molde" que un comando lee antes de generar un archivo nuevo, para que el resultado
siempre tenga la misma estructura (igual que cuando armás un documento a partir de una plantilla de
Word).

| Template | Lo usa... |
|---|---|
| `US-template.md` | `/generar-historia-de-usuario` |
| `ADR-template.md` | Cuando vos o Claude crean un ADR nuevo a mano |
| `vision-template.md` | `/crear-vision` |
| `README-template.md` | Si en algún momento necesitás un README para un sub-proyecto nuevo |

---

## 8. Documentación MD — `docs/`

Esta es la "memoria del equipo": todo lo que alguien necesitaría saber para retomar el proyecto está
escrito acá, no solo en la cabeza de quien lo armó.

- **`docs/constitution.md`** — todas las reglas, con su razonamiento. Es la fuente de verdad; si algo
  contradice este archivo, gana este archivo.
- **`docs/vision.md`** — el dominio completo del negocio: entidades, reglas, endpoints, permisos.
  Ya está lleno con la información de Perfumeria — pero tiene varios puntos marcados con ⚠️ que son
  supuestos que se hicieron para poder avanzar y que conviene confirmar antes de implementarlos.
- **`docs/adr/`** — 8 decisiones arquitectónicas ya tomadas, cada una con sus alternativas
  consideradas y por qué se descartaron. Si alguna vez te preguntás *"¿por qué se hizo así y no
  de otra forma?"*, la respuesta está en un ADR.
- **`docs/stories/`** — historias de usuario. Hay dos de ejemplo (`US-001`, `US-002`) para que veas
  el formato esperado antes de generar las tuyas con `/generar-historia-de-usuario`.

### ¿Qué son los supuestos marcados con ⚠️ en `docs/vision.md`?

Cuando la información que diste tenía un vacío (por ejemplo, el módulo "Estuches" no traía campos
propios, o no estaba claro si "recordar contraseña" significa "mantener sesión" o "recuperar
contraseña"), se tomó la decisión más razonable y se dejó marcada explícitamente en vez de
inventarla en silencio. Antes de implementar esa parte, vale la pena confirmarla o ajustarla —
podés hacerlo con `/crear-vision` o simplemente editando `docs/vision.md` a mano.

---

## 9. Flujo de trabajo completo, paso a paso (de cero a un endpoint funcionando)

Esto es lo que harías, en orden, para construir la primera funcionalidad real:

1. **Leé `docs/vision.md`** — especialmente los ⚠️ — y confirmá o ajustá lo que haga falta.
2. **`/crear-estructura-proyecto`** — genera el esqueleto de `Perfumeria.Api` (5 capas).
3. **`/crear-estructura-bd`** — genera el proyecto de base de datos con las tablas y SPs.
4. **`/crear-estructura-spa tienda`** y **`/crear-estructura-spa admin`** — genera ambas SPA.
5. Elegí una historia ya escrita (`docs/stories/US-001-listar-catalogo-perfumes.md`) o generá una
   nueva con **`/generar-historia-de-usuario`**.
6. Para cada criterio de aceptación de esa historia:
   - **`/generar-prueba-desde-ac`** → escribe la prueba que falla (🔴 RED).
   - **`/implementar-para-pasar-prueba`** → escribe el código mínimo que la hace pasar (🟢 GREEN).
   - Revisá el código a mano y pedile a Claude que lo limpie sin romper la prueba (🔵 REFACTOR).
7. Repetí el paso 6 para cada AC de cada historia.
8. Una vez que exista el primer endpoint de login: **`/seguridad-asegurar-api`**.
9. Una vez asegurado el backend: **`/seguridad-asegurar-spa tienda`** y
   **`/seguridad-asegurar-spa admin`**.
10. Para el checkout: revisá `docs/adr/ADR-004-pasarela-pago.md` y el skill `pasarela-pago-cr` antes
    de pedir la implementación — te van a dar el contexto exacto de qué proveedor usar y cómo.

---

## 10. Preguntas frecuentes

**¿Tengo que escribir todo esto en cada mensaje?**
No. `CLAUDE.md` se carga solo. Los comandos empiezan con `/`. Los skills se activan solos. Solo los
agentes a veces conviene mencionarlos explícitamente si querés forzar un rol específico.

**¿Qué pasa si le pido algo que contradice una regla de `docs/constitution.md`?**
Claude te lo va a señalar y te va a explicar por qué, en vez de simplemente obedecer — así está
diseñado en cada agente y en `CLAUDE.md` (sección "Límites globales").

**¿Puedo editar `docs/vision.md` yo directamente, sin usar `/crear-vision`?**
Sí, es un Markdown normal. `/crear-vision` simplemente ayuda a hacerlo de forma guiada y consistente,
pero no es obligatorio pasar por el comando.

**¿Por qué no puedo editar un ADR directamente?**
Porque un hook de gobernanza lo bloquea a propósito (`docs/constitution.md` §10.1) — un ADR aceptado
representa una decisión ya tomada; si cambia, se documenta como una decisión *nueva* que reemplaza a
la anterior, para no perder el historial de "por qué se decidió así antes".

**¿Qué hago si un comando o agente no sabe algo de mi negocio?**
Va a preguntarte en vez de inventarlo — esa es una regla dura en toda la gobernanza
(`CLAUDE.md`: *"No inventar campos, entidades ni reglas que no estén en docs/vision.md"*).

**¿Dónde edito el nombre "Perfumeria" si decido cambiarlo más adelante?**
Aparece en: `docs/vision.md` (título), `docs/constitution.md` (frontmatter y varias menciones),
todos los ADR, `CLAUDE.md`, este archivo, y en los nombres de proyecto (`Perfumeria.Api`,
`perfumeria-admin`, `perfumeria-tienda`) una vez que los crees con los comandos de estructura. No hay
un único punto centralizado — es un buscar y reemplazar consciente si decidís cambiarlo.

---

## 11. Glosario rápido

| Sigla / término | Significado |
|---|---|
| **US** | User Story / Historia de Usuario |
| **AC** | Acceptance Criteria / Criterio de Aceptación |
| **GWT** | Given/When/Then → Dado/Cuando/Entonces |
| **INVEST** | Independiente, Negociable, Valiosa, Estimable, Small, Testable — criterios de calidad de una US |
| **SDD** | Spec-Driven Development — especificar antes de programar |
| **TDD** | Test-Driven Development — probar antes de implementar |
| **RED / GREEN / REFACTOR** | Las 3 fases del ciclo TDD: prueba que falla → código mínimo → limpieza |
| **ADR** | Architecture Decision Record — documento que registra una decisión y su razonamiento |
| **SOLID** | 5 principios de diseño orientado a objetos (SRP, OCP, LSP, ISP, DIP) |
| **PCI SAQ-A** | El nivel más liviano de cumplimiento de seguridad de tarjetas — aplica cuando nunca tocás datos de tarjeta directamente (nuestro caso, gracias al checkout hospedado) |
| **JWT** | JSON Web Token — el "pase" que prueba que un usuario inició sesión |
| **Dapper** | Librería de acceso a datos ligera para .NET, usada junto con Stored Procedures |
| **SPA** | Single Page Application — aplicación web que no recarga la página completa |
