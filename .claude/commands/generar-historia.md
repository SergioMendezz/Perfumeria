---
description: Versión rápida de generar-historia-de-usuario — crea una historia sencilla con 1-2 criterios de aceptación, lista para empezar el ciclo TDD sin todo el formalismo de INVEST. Útil para tareas pequeñas dentro de una US ya existente.
argument-hint: <qué querés lograr, en una frase>
allowed-tools: Read, Write, Glob
---

# Comando · Generar historia (rápida)

## Qué hace
Crea un archivo en `docs/stories/US-<nombre>-rapida.md` que describe **qué se quiere lograr**, los
**datos involucrados** y **cómo se sabrá que funciona** (uno o dos criterios de aceptación), sin el
formalismo completo de INVEST. Pensado para tareas pequeñas y acotadas.

## Qué se pregunta si `$ARGUMENTS` no lo trae
- **¿Qué se quiere lograr?** (ej. "editar el precio de una variante de perfume")
- **¿Quién lo usa?** (`Visitante`, `Cliente` o `Admin`)
- **¿Cómo se sabrá que funciona?** (código de estado, mensaje, o dato devuelto concreto)

## Qué genera

```markdown
# US · <título en una frase>

## Historia
Como <rol>
Quiero <capacidad>
Para <beneficio>

## Criterio de aceptación (AC-01) — caso exitoso
Dado que <situación con datos concretos>
Cuando <acción HTTP/UI concreta>
Entonces el sistema responde con <código HTTP> y <campo/valor devuelto>

## Criterio de aceptación (AC-02) — validación: <nombre de la regla>
Dado que <entrada inválida concreta>
Cuando <acción>
Entonces el sistema responde con <código de error> y mensaje = "<texto exacto>"
```

## Reglas
- Un AC por escenario; el "Entonces" siempre lleva código HTTP + valor concreto.
- Si la tarea toca `Stock*` o `Pedido`, agregar un AC de concurrencia (ver Constitution §2.5).
- Todo en español, lenguaje simple.

## Checklist antes de guardar
- [ ] `docs/vision.md` ya cubre la entidad/regla involucrada — si no, avisar y sugerir `/crear-vision`.
- [ ] El endpoint/acción está definido (método HTTP + ruta, o vista + acción de UI).
- [ ] Los campos requeridos tienen tipo de dato y ejemplo concreto.
- [ ] El "Entonces" dice exactamente qué devuelve el sistema.

## Después de esto
Sugerir el siguiente paso: `/generar-prueba-desde-ac` para crear la prueba que falla (🔴 RED).
