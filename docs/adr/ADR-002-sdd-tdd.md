---
adr_id: ADR-002
title: Adopción de Spec-Driven Development y Test-Driven Development como práctica obligatoria
status: aceptado
date: 2026-07-21
authors: [Camila]
reviewers: [Por definir]
tags: [proceso, backend, frontend, testing, calidad]
supersedes: []
superseded_by: []
---

# ADR-002 · Adopción de SDD + TDD como práctica obligatoria

## 1. Estado

`aceptado`.

---

## 2. Contexto

El desarrollo asistido por Claude Code introduce el riesgo de "vibe-coding": generar código sin
especificación previa ni pruebas que lo respalden. En un dominio con dinero real de por medio
(pasarela de pago) y datos personales de clientes, ese riesgo es más grave que en un CRUD genérico:
un error de stock virtual mal descontado o un webhook de pago mal validado puede significar pérdida
de dinero real o inventario vendido dos veces.

Se requiere un flujo que transforme cada requisito en una especificación ejecutable y cada criterio
de aceptación en una prueba antes de escribir una sola línea de código productivo.

---

## 3. Decisión

Se adopta el flujo **Spec-Driven Development (SDD) + Test-Driven Development (TDD)** como práctica
obligatoria para todo desarrollo de nuevas funcionalidades y refactors.

### 3.1 Detalle de la decisión

```
Historia de usuario (US)
       ↓
Criterios de aceptación (AC en formato GWT)
       ↓
🔴 RED    → Prueba unitaria que falla (derivada del AC)
       ↓
🟢 GREEN  → Código mínimo que hace pasar la prueba
       ↓
🔵 REFACTOR → Aplicar SOLID + Clean Code sin romper pruebas
       ↓
Commit + PR (con referencia AC-##)
```

**Reglas obligatorias:**

1. No hay código sin AC que lo motive.
2. No hay código sin prueba que lo respalde.
3. Relación 1:N entre AC y pruebas; 1:1 entre prueba y unidad pública.
4. Nomenclatura de pruebas: `Metodo_Escenario_ResultadoEsperado`.
5. Trazabilidad en el commit/PR: debe referenciar el AC correspondiente.
6. **Escenarios mínimos ampliados para flujos de dinero e inventario** (más estrictos que el mínimo
   genérico del taller): además de éxito/validación/auth/no-encontrado/duplicidad/fallo-externo, todo
   caso de uso de **pago** o de **descuento de stock** MUST incluir un escenario de
   **concurrencia/idempotencia** (dos compras simultáneas de la última unidad de `StockVirtual`; un
   webhook de la pasarela reintentado dos veces no debe duplicar el pago).
7. Cobertura mínima definida en CI; prohibido `--passWithNoTests` o equivalente.

### 3.2 Alcance

- **Aplica a:** todo desarrollo nuevo (endpoints, componentes, integración con la pasarela) y
  refactors sustantivos.
- **No aplica a:** cambios cosméticos puros (formato, comentarios, renombres).

---

## 4. Alternativas evaluadas

| Alternativa | Descripción | Pros | Contras | ¿Descartada por? |
|---|---|---|---|---|
| A. Pruebas después del código | Práctica histórica | Menor fricción inicial | Casos borde de dinero/stock no cubiertos por diseño | Riesgo inaceptable en flujos de pago |
| B. Solo TDD sin SDD | Pruebas primero, sin US formal | Ciclo Red-Green preservado | Sin especificación, cada desarrollo interpreta distinto el requisito | Insuficiente para un dominio con reglas de negocio finas (stock dual, derivados) |
| C. SDD + TDD ✅ | Spec ejecutable → pruebas → código | Trazabilidad end-to-end, contexto rico para Claude Code | Curva de aprendizaje, disciplina requerida | **Elegida** |

---

## 5. Consecuencias

### 5.1 Positivas
- Trazabilidad auditable US → AC → Prueba → Código → Commit → PR.
- Los escenarios de concurrencia en pagos/stock quedan cubiertos por diseño, no por accidente.
- Claude Code dispone de contexto rico (US + AC) para generar código pertinente.

### 5.2 Negativas / Costos
- Curva de aprendizaje si no se ha practicado TDD antes.
- Mayor tiempo aparente en los primeros ciclos.

### 5.3 Neutrales / Observaciones
- La velocidad percibida cambia: menos código por hora, más código correcto por entrega.

---

## 6. Cumplimiento y verificación

- Comandos `/generar-prueba-desde-ac` e `/implementar-para-pasar-prueba` refuerzan el ciclo.
- CI ejecuta la suite completa y reporta cobertura en cada PR.
- Revisión de código valida la trazabilidad AC ↔ Test.

---

## 7. Excepciones

- Hotfixes críticos de producción pueden mergear sin ciclo completo, con compromiso escrito de
  completar US + AC + pruebas en un PR de seguimiento inmediato.

---

## 8. Referencias

- Kent Beck — *Test-Driven Development: By Example* (2002).
- Martin Fowler — *TestDrivenDevelopment* (bliki).
- Robert C. Martin — *Clean Architecture* (2017).
- ADRs relacionados: `ADR-001` (monorepo), `ADR-004` (pasarela de pago), `ADR-005` (stock dual).

---

## 9. Historial

| Fecha | Estado | Autor | Cambio |
|---|---|---|---|
| 2026-07-21 | aceptado | Camila | Versión inicial, adaptada del taller con escenarios de concurrencia ampliados para pagos y stock. |
