---
description: Guía para completar o actualizar docs/vision.md haciendo preguntas por sección, sin inventar ningún dato. Usar solo si vision.md necesita cambios estructurales — para el dominio actual de Perfumeria, vision.md ya está completo.
argument-hint: [sección a actualizar, opcional]
allowed-tools: Read, Write, Edit, Glob
---

# Comando · Crear/actualizar documento de visión del producto

## Objetivo
Mantener `docs/vision.md` completo y consistente, haciendo preguntas al usuario sección por sección,
sin inventar ningún dato. Todo campo debe venir de las respuestas del usuario o ya estar en el
archivo actual.

## Contexto obligatorio
- Leer `docs/templates/vision-template.md` para la estructura esperada.
- Leer `docs/vision.md` actual — **ya contiene el dominio completo de Perfumeria**, incluyendo
  varios supuestos marcados con ⚠️ que deben confirmarse con el usuario antes de darlos por buenos.

> ⛔ **Nunca inventar entidades, campos, reglas ni endpoints nuevos. Preguntar siempre.**

## Instrucciones

**Si `$ARGUMENTS` está vacío:**
1. Mostrar un resumen de `docs/vision.md` actual (entidades, endpoints, supuestos ⚠️ pendientes).
2. Preguntar si el usuario quiere: (a) confirmar/corregir los supuestos marcados con ⚠️, (b) agregar
   una entidad o regla nueva, o (c) no hay cambios por ahora.

**Si `$ARGUMENTS` indica una sección** (p. ej. "entidades", "endpoints", "reglas de negocio"):
1. Mostrar el contenido actual de esa sección.
2. Preguntar sección por sección los cambios necesarios — nunca reescribir todo el documento de una
   sola vez sin mostrar antes qué se va a modificar.
3. Validar consistencia: toda FK debe referenciar una entidad ya definida; todo endpoint nuevo debe
   tener al menos una regla de negocio asociada; toda entidad con inventario debe tener
   `StockTienda`/`StockVirtual` (nunca un campo `Stock` único, ver `ADR-005`).

## Reglas duras
- Preguntar antes de escribir — no regenerar `vision.md` completo sin confirmación explícita.
- Si se detecta una inconsistencia (FK a entidad no definida, endpoint sin regla, stock sin dividir),
  alertar y pedir corrección antes de continuar.
- Actualizar siempre el Historial (§10) con la nueva versión y fecha al guardar cambios.

## Salida esperada
- `docs/vision.md` actualizado y coherente, con su Historial incrementado.
- Mensaje de confirmación resumiendo qué cambió.
