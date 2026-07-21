---
adr_id: ADR-006
title: Cumplimiento legal costarricense para comercio electrónico (Ley 7472 y Ley 8968)
status: aceptado
date: 2026-07-21
authors: [Camila]
reviewers: [Por definir]
tags: [legal, cumplimiento, frontend, backend]
supersedes: []
superseded_by: []
---

# ADR-006 · Cumplimiento legal costarricense para comercio electrónico

## 1. Estado

`aceptado` para el diseño técnico. **Los textos legales concretos (Términos y Condiciones, Política
de Privacidad) requieren revisión de un profesional en Derecho antes de publicar el sitio** — este
ADR no es asesoría legal, solo documenta los requisitos técnicos que el sistema debe soportar para
que esos textos puedan cumplirse.

---

## 2. Contexto

El requisito original pide explícitamente que "la página cumpla con las leyes de Costa Rica para
páginas en línea". El comercio electrónico en Costa Rica se rige principalmente por:

- **Ley N.° 7472** (Ley de Promoción de la Competencia y Defensa Efectiva del Consumidor) y su
  Reglamento, reformado por el **Decreto N.° 40703-MEIC** que agregó el **Capítulo X — Protección al
  consumidor en el contexto del comercio electrónico**.
- **Ley N.° 8968** — Protección de la Persona frente al Tratamiento de sus Datos Personales, aplicable
  porque la tienda recolecta datos personales de clientes (nombre, correo, dirección de envío).

Los elementos obligatorios identificados en la investigación (julio 2026) incluyen: información clara
sobre plazos de entrega, **derecho de retracto de 8 días**, comprobante de pago tras cada transacción,
mecanismo gratuito y transparente de quejas/reclamos con plazos de respuesta, y garantía legal de
conformidad de los bienes.

---

## 3. Decisión

El sistema **MUST** soportar, a nivel de datos y de flujo, los elementos que permiten cumplir estas
leyes. Los textos legales en sí (redacción de Términos y Condiciones, Política de Privacidad) quedan
fuera del alcance de código y deben producirse/revisarse aparte.

### 3.1 Detalle de la decisión — requisitos que el sistema MUST soportar

| Requisito legal | Soporte técnico requerido |
|---|---|
| Términos y condiciones visibles antes de comprar | Página/ruta pública `/terminos-y-condiciones` en `perfumeria-tienda`, enlazada desde el checkout |
| Plazo de entrega informado (si no se indica, el Código de Comercio asume 24h — riesgo a evitar) | Campo de plazo de entrega estimado visible en el checkout, antes de confirmar el pago |
| Derecho de retracto de 8 días hábiles | Estado `Pedido.Estado = "Cancelado"` debe ser alcanzable por el cliente dentro de una ventana de 8 días desde `FechaPedido`; el endpoint de cancelación debe validar esa ventana |
| Comprobante de pago tras transacción exitosa | Al marcar `Pedido.Estado = "Pagado"` (`ADR-004`), el sistema MUST generar/enviar un comprobante (email o vista de confirmación con número de pedido, monto, fecha) |
| Precio final visible en colones antes de pagar | Todo `Precio` se muestra en ₡ (colones) en el carrito y checkout, sin cargos ocultos agregados después |
| Mecanismo de quejas/reclamos | Página `/contacto` o `/reclamos` en `perfumeria-tienda`, con un canal de contacto y un plazo de respuesta indicado |
| Consentimiento de tratamiento de datos personales (Ley 8968) | Checkbox explícito de aceptación de la Política de Privacidad en el registro de cliente (`POST /api/auth/registro-cliente`), no marcado por defecto |
| Garantía legal de conformidad de los bienes | Mención en Términos y Condiciones (texto legal, no requiere campo de datos) |

### 3.2 Alcance

- **Aplica a:** todo el flujo de registro de cliente, catálogo, checkout y post-venta en
  `perfumeria-tienda`.
- **No aplica a:** facturación electrónica ante el Ministerio de Hacienda (fuera de alcance de esta
  versión, ver `docs/vision.md` §7) ni a la redacción final de los textos legales (responsabilidad de
  un profesional en Derecho, no de Claude Code).

---

## 4. Alternativas evaluadas

| Alternativa | Descripción | Pros | Contras | ¿Descartada por? |
|---|---|---|---|---|
| A. Ignorar el cumplimiento legal en esta versión | Enfocarse solo en funcionalidad | Menos trabajo inicial | Incumple un requisito explícito del negocio y expone a sanciones de la Comisión Nacional del Consumidor | Descartada — requisito explícito |
| **B. Soportar los elementos técnicos mínimos, delegar la redacción legal a un profesional** ✅ | El código habilita el cumplimiento; el contenido legal se produce aparte | Balance realista entre alcance de un proyecto de software y la necesidad de asesoría legal real | Requiere coordinación con un profesional en Derecho antes de producción | **Elegida** |
| C. Generar los textos legales completos con IA sin revisión humana | Máxima rapidez | — | Alto riesgo: un texto legal mal redactado puede ser peor que no tenerlo | Descartada — Claude Code no sustituye asesoría legal |

---

## 5. Consecuencias

### 5.1 Positivas
- El modelo de datos ya contempla los campos necesarios para cumplir (`Pedido.Estado`,
  `Pedido.FechaPedido`, comprobantes) sin retrabajo posterior.
- Reduce el riesgo de tener que rediseñar el checkout más adelante por un requisito legal olvidado.

### 5.2 Negativas / Costos
- No elimina la necesidad de contratar/consultar a un profesional en Derecho para los textos finales.

### 5.3 Neutrales / Observaciones
- Costa Rica es, según fuentes del sector, el país centroamericano con mayor proporción de compras en
  línea — el cumplimiento cuidadoso es también una ventaja competitiva, no solo una obligación.

---

## 6. Cumplimiento y verificación

- Prueba obligatoria: un cliente no puede cancelar (`derecho de retracto`) un pedido con más de 8
  días desde `FechaPedido`.
- Revisión manual: el checkout no permite confirmar el pago sin que el checkbox de Política de
  Privacidad esté marcado.
- Checklist manual antes de publicar: Términos y Condiciones y Política de Privacidad revisados por
  un profesional en Derecho.

---

## 7. Excepciones

- Ninguna sin nuevo ADR — el cumplimiento legal no admite excepciones ad-hoc.

---

## 8. Referencias

- Ley N.° 7472 — *Ley de Promoción de la Competencia y Defensa Efectiva del Consumidor*.
- Decreto Ejecutivo N.° 40703-MEIC — Capítulo X, Reglamento a la Ley N.° 7472 (comercio electrónico).
- Ley N.° 8968 — *Protección de la Persona frente al Tratamiento de sus Datos Personales*.
- Ministerio de Economía, Industria y Comercio (MEIC) — Comisión Nacional del Consumidor.
- ADRs relacionados: `ADR-004` (pasarela de pago), `ADR-005` (stock dual).

---

## 9. Historial

| Fecha | Estado | Autor | Cambio |
|---|---|---|---|
| 2026-07-21 | aceptado | Camila | Versión inicial, basada en investigación normativa de julio 2026. No sustituye asesoría legal profesional. |
