# CLAUDE.md — Perfumeria

Reglas mínimas **siempre activas** para trabajar en este repositorio con Claude Code. El detalle
completo y el razonamiento de cada regla viven en `docs/constitution.md` — este archivo es un
resumen operativo, no la fuente de verdad. Si algo aquí y en la Constitution se contradice,
**gana la Constitution**.

> 📖 ¿Primera vez usando este repositorio? Leé primero `GUIA-DE-AGENTES.md` — explica cómo usar
> cada agente, comando y skill sin necesidad de memorizar nada de esto.

## Cómo trabajamos (el ciclo obligatorio)

```
docs/vision.md (ya completado)
   ↓
/generar-historia-de-usuario   ← US con criterios GWT
   ↓
🔴 RED   → /generar-prueba-desde-ac      (prueba que falla)
🟢 GREEN → /implementar-para-pasar-prueba (código mínimo)
🔵 REFACTOR → limpieza manual sin romper pruebas
   ↓
PR referenciando US-### y AC-##
```

**Regla de oro: no hay código de producción sin una prueba que lo respalde.**

## Reglas de código (resumen — detalle en Constitution §3–§7)

- Español en identificadores de dominio (clases, métodos, variables, componentes). Palabras
  reservadas de C#/TypeScript en su idioma original.
- Backend: 5 capas (`Abstracciones`, `API`, `Flujo`, `Servicios`, `DA`) — **sin** capa `Reglas`
  separada; las reglas de negocio puras viven en `Flujo` (Constitution §3.1, `ADR-003`).
- Frontend (`perfumeria-tienda` y `perfumeria-admin`): solo componentes funcionales + hooks,
  TypeScript estricto, sin `any`, sin `React.FC`, sin `axios` (usar `useRecurso<T>()` con `fetch`
  nativo). Ver `ADR-Frontend-001` y `ADR-Frontend-002` para la lista blanca de dependencias.
- Métodos ≤ 20 líneas, sin números mágicos, sin código muerto.
- **No inventar campos, entidades ni reglas** que no estén en `docs/vision.md`. Si falta
  información, preguntar antes de asumir (igual que se hizo al generar `vision.md` — cada supuesto
  quedó marcado con ⚠️ y debe confirmarse antes de implementarse).

## Reglas específicas del dominio (no genéricas — léelas aunque conozcas el patrón general)

- **Catálogo público, checkout privado:** ver y listar productos nunca requiere token. Registrarse
  solo es obligatorio para comprar (`docs/vision.md` §2).
- **Stock dividido:** todo producto con inventario tiene `StockTienda` y `StockVirtual` separados.
  Solo `StockVirtual` se descuenta en una compra en línea. Nunca fusionar ambos campos en uno solo
  "Stock" genérico. Ver Skill `control-stock-dual` y `ADR-005`.
- **Nombre y Marca con mayúscula inicial tras cada espacio:** regla de negocio, no solo de estilo.
  Ver Skill `nomenclatura-productos`.
- **Categorías de perfume siempre con nombre completo** (`Eau de Parfum`, nunca `EDP`) en cualquier
  texto visible al usuario. Ver `docs/vision.md` §3.1.
- **Código de barras:** todo módulo de inventario debe soportar búsqueda tanto por código completo
  como por los últimos dígitos, y aceptar entrada de pistola lectora (input + Enter). Ver Skill
  `codigo-barras`.
- **Pasarela de pago:** nunca capturar ni almacenar datos de tarjeta en formularios propios del
  backend o del SPA — siempre delegar en el checkout/SDK de la pasarela (`ADR-004`,
  Skill `pasarela-pago-cr`, Constitution §8.5).
- **Cumplimiento legal CR:** cualquier pantalla de checkout o registro debe respetar lo indicado en
  `ADR-006` (derecho de retracto, política de privacidad, comprobante de pago).

## Criterios de aceptación

Formato GWT obligatorio (Dado/Cuando/Entonces). El "Entonces" debe ser verificable (código HTTP,
campo devuelto, mensaje exacto) — nunca "funciona bien".

## Herramientas de prueba

- Backend: xUnit + FluentAssertions + NSubstitute, estructura `// Arrange / // Act / // Assert`.
- Frontend: Vitest + @testing-library/react, `describe`/`it` en español.

## Cómo pedirle ayuda a Claude Code

- **Agentes** (`.claude/agents/`): invocá con el Task tool describiendo el rol, o pedile a Claude
  "usá el agente programador-api" — Claude lo despacha como subagente.
- **Comandos** (`.claude/commands/`): escribí `/generar-historia-de-usuario`, `/generar-prueba-desde-ac`,
  `/implementar-para-pasar-prueba`, etc.
- **Skills** (`.claude/skills/`): se activan solos cuando la tarea coincide (no hace falta invocarlos).
- Ver `GUIA-DE-AGENTES.md` para el catálogo completo con ejemplos.

## Commits

`feat: agregar búsqueda de perfume por código de barras` (verbo en imperativo, Conventional Commits).

## Límites globales — "never touch" sin ADR nuevo

- ❌ Modificar `docs/constitution.md` o cualquier ADR de `docs/adr/` directamente — se supersede con
  un ADR nuevo, nunca se edita en sitio.
- ❌ Agregar dependencias fuera de `ADR-Frontend-002` sin ADR de excepción.
- ❌ Guardar datos de tarjeta, CVV, contraseñas en texto plano o llaves de la pasarela en el repo.
- ❌ Fusionar `StockTienda` y `StockVirtual` en un solo campo.
- ❌ Mostrar siglas de categoría de perfume (`EDP`) en la interfaz de la tienda.
