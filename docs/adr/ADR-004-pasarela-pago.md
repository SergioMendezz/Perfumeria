---
adr_id: ADR-004
title: Integración con pasarela de pago costarricense (ONVO Pay como principal, Tilopay como alternativa)
status: propuesto
date: 2026-07-21
authors: [Camila]
reviewers: [Por definir]
tags: [pagos, seguridad, backend, critico]
supersedes: []
superseded_by: []
priority: critica
---

# ADR-004 · Integración con pasarela de pago

## 1. Estado

`propuesto` — requiere confirmar cuenta comercial real antes de pasar a `aceptado`. El diseño técnico
puede implementarse en modo sandbox mientras tanto.

---

## 2. Contexto

Perfumeria necesita cobrar en línea a clientes costarricenses. Se investigaron las opciones
disponibles en el mercado costarricense (julio 2026):

| Pasarela | Comisión con tarjeta | Comisión SINPE Móvil | Notas |
|---|---|---|---|
| **ONVO Pay** | 3.9% + $0.25 | 1.5% | Checkout prearmado o SDK/API; sin mensualidad; liquidación diaria con mínimo de $30/₡20.000; supervisada por SUGEF solo en materia de legitimación de capitales, no en solvencia financiera |
| **Tilopay** | 4.25% + $0.35 | 2% + $0.35 | Apoyada en PowerTranz (3D Secure 2.0); plugin oficial WooCommerce; comisión total más alta que ONVO Pay |

Ambas ofrecen: checkout hospedado (sin tocar datos de tarjeta desde el propio servidor), soporte
para SINPE Móvil (método muy usado en Costa Rica), y cumplimiento PCI DSS delegado al proveedor.

Ninguna pasarela costarricense reemplaza la responsabilidad legal del comercio: el sitio sigue
obligado por la Ley 7472 y su reglamento de comercio electrónico (`ADR-006`) independientemente de
cuál pasarela se use.

> ⚠️ Esta comparación se basa en información pública de julio 2026; las comisiones cambian con
> frecuencia. Antes de integrar en producción, confirmar tarifas vigentes directamente con el
> proveedor.

---

## 3. Decisión

Se adopta **ONVO Pay** como pasarela principal (mejor estructura de comisiones para el volumen
esperado de un e-commerce de perfumería), con **Tilopay** documentado como alternativa de respaldo si
ONVO Pay no está disponible para el tipo de cuenta comercial de la propietaria del proyecto.

### 3.1 Detalle de la decisión

- **Modelo de integración:** checkout hospedado / SDK de la pasarela (ver `ADR-Frontend-002`), nunca
  un formulario propio de tarjeta. Esto mantiene al backend y al SPA fuera de alcance PCI (SAQ-A).
- **Flujo:**
  1. Cliente confirma el carrito → `POST /api/pedido` crea el `Pedido` en estado `Pendiente`.
  2. `POST /api/pago/iniciar` genera la sesión/link de cobro contra la API de la pasarela y devuelve
     la URL/token al SPA.
  3. El SPA redirige al checkout de la pasarela (o monta su SDK embebido).
  4. La pasarela notifica el resultado por **webhook** a `POST /api/pago/webhook`.
  5. `Perfumeria.Servicios.PasarelaPagoService` valida la firma/autenticidad del webhook (nunca
     confiar en un webhook sin verificar firma).
  6. Solo si la firma es válida y el estado es "aprobado": `Flujo` marca el `Pedido` como `Pagado`,
     guarda `IdTransaccionPasarela`, y descuenta `StockVirtual` de cada `ItemPedido` (ver `ADR-005`)
     dentro de una única transacción SQL.
  7. Si el webhook llega duplicado (reintento de la pasarela), la operación debe ser **idempotente**:
     verificar primero si `IdTransaccionPasarela` ya fue procesado antes de descontar stock de nuevo.
- **Nunca** almacenar PAN, CVV ni fecha de expiración de tarjeta en `Perfumeria.Api` ni en
  `perfumeria-tienda`.
- Las llaves de la API de la pasarela (secret key) viven en variables de entorno /
  `appsettings.Development.json` **no versionado** — nunca en `appsettings.json` versionado
  (Constitution §8.1).

### 3.2 Alcance

- **Aplica a:** el flujo de checkout de `perfumeria-tienda`.
- **No aplica a:** ventas presenciales en la tienda física (fuera de alcance de este proyecto, ver
  `docs/vision.md` §7).

---

## 4. Alternativas evaluadas

| Alternativa | Descripción | Pros | Contras | ¿Descartada por? |
|---|---|---|---|---|
| **A. ONVO Pay** ✅ | Fintech costarricense, checkout + SDK | Comisión más baja del mercado local, SINPE Móvil, liquidación diaria | Requiere cuenta comercial activa | **Elegida como principal** |
| **B. Tilopay** | Fintech costarricense, apoyada en PowerTranz | Muy documentada, plugin WooCommerce, 3DS 2.0 | Comisión total más alta | Elegida como **alternativa de respaldo** |
| C. Formulario propio de tarjeta (sin pasarela hospedada) | Captura directa de PAN/CVV en el propio formulario | Control total del diseño del checkout | Alcance PCI DSS completo (SAQ-D), riesgo de seguridad y cumplimiento inasumible para un proyecto académico | Descartada — viola Constitution §8.5 |
| D. Pasarela internacional (Stripe/PayPal solamente) | Sin presencia local | Documentación excelente | Sin soporte nativo de SINPE Móvil, método de pago muy usado en Costa Rica | Descartada como única opción; podría evaluarse como complemento futuro |

---

## 5. Consecuencias

### 5.1 Positivas
- El proyecto nunca queda en alcance PCI DSS completo — la carga de seguridad de tarjeta la asume el
  proveedor.
- Soporte de SINPE Móvil, relevante para clientela costarricense.
- Diseño idempotente del webhook previene doble descuento de stock ante reintentos de red.

### 5.2 Negativas / Costos
- Depender de la disponibilidad y tiempos de respuesta de un proveedor externo.
- Comisión por transacción reduce el margen (documentado, no oculto).

### 5.3 Neutrales / Observaciones
- Si en el futuro se abre una cuenta comercial con otra pasarela, este ADR debe actualizarse (nuevo
  ADR que lo supersede) — no editarse en sitio.

---

## 6. Cumplimiento y verificación

- Prueba de idempotencia obligatoria (`ADR-002` §3.1.6): reenviar el mismo webhook dos veces no debe
  descontar stock dos veces ni duplicar el `Pedido` en estado `Pagado`.
- Prueba de firma inválida: un webhook sin firma válida MUST responder rechazo y no debe tocar stock
  ni el estado del pedido.
- Revisión de código verifica que ningún commit contenga la secret key de la pasarela.

---

## 7. Excepciones

- Cambiar de pasarela principal requiere un nuevo ADR.

---

## 8. Referencias

- ONVO Pay — `https://onvopay.com/checkout`, `https://onvopay.com/devs`.
- Tilopay — `https://www.tilopay.com/`.
- El Colectivo 506 — *Pasarelas de pago en Costa Rica: todo lo que debo saber* (comparativa de
  comisiones, 2026).
- GInIEm — *Pasarelas de Pago en Costa Rica: Guía Completa* (2026).
- PCI Security Standards Council — *SAQ A* (alcance para comercios que redirigen el pago).
- ADRs relacionados: `ADR-005` (stock dual), `ADR-006` (cumplimiento legal CR).

---

## 9. Historial

| Fecha | Estado | Autor | Cambio |
|---|---|---|---|
| 2026-07-21 | propuesto | Camila | Versión inicial, basada en investigación de mercado de julio 2026. Pendiente de aceptación formal al confirmar cuenta comercial. |
