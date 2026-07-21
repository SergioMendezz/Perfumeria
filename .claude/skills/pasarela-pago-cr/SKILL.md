---
name: pasarela-pago-cr
description: Conocimiento de referencia para integrar ONVO Pay o Tilopay (pasarelas de pago costarricenses) en el checkout de perfumeria-tienda y en el webhook de confirmación del backend, manteniendo el proyecto fuera de alcance PCI DSS completo. Se activa automáticamente en cualquier trabajo sobre pagos, checkout, o los servicios PasarelaPagoService.
---

# Skill · Integración con pasarela de pago (Costa Rica)

Ver `docs/adr/ADR-004-pasarela-pago.md` para la decisión completa. Esta skill es el "cómo", el ADR
es el "qué se decidió y por qué".

## Principio no negociable
El backend y el SPA de Perfumeria **nunca** tocan el número de tarjeta, CVV ni fecha de expiración.
Toda esa información viaja directamente del navegador del cliente al servidor de la pasarela
(checkout hospedado o SDK embebido) — esto mantiene al proyecto en alcance **PCI SAQ-A**, el más
liviano posible.

## Comparación rápida de las dos opciones investigadas (julio 2026)

| | ONVO Pay (principal) | Tilopay (respaldo) |
|---|---|---|
| Comisión tarjeta | 3.9% + $0.25 | 4.25% + $0.35 |
| Comisión SINPE Móvil | 1.5% | 2% + $0.35 |
| Mensualidad | Ninguna | Ninguna |
| Liquidación | Diaria, mínimo $30/₡20.000 | Diaria (según fuente), sin monto mínimo especificado públicamente |
| Métodos | Tarjeta, transferencia SINPE IBAN, SINPE Móvil | Tarjeta, SINPE Móvil (apoyada en PowerTranz) |
| Integración | Checkout hospedado, links de pago, SDK/API | Checkout, plugin WooCommerce, API |

> Confirmar tarifas vigentes directamente con el proveedor antes de integrar en producción — las
> comisiones cambian con el tiempo.

## Flujo de integración (backend — `Perfumeria.Servicios.PasarelaPagoService`)

```csharp
public interface IPasarelaPagoService
{
    Task<SesionPagoResponse> IniciarPago(Pedido pedido);
    bool ValidarFirmaWebhook(string payloadCrudo, string firmaRecibida, string secretKey);
}
```

1. `IniciarPago`: llama a la API de la pasarela para crear una sesión/link de cobro por el `Total`
   del `Pedido`, en colones (₡). Devuelve la URL/token que el SPA usa para redirigir o montar el SDK.
2. El webhook (`POST /api/pago/webhook`, `[AllowAnonymous]`) recibe la notificación asíncrona del
   resultado. **Antes de cualquier otra lógica:**
   ```csharp
   if (!_pasarelaPagoService.ValidarFirmaWebhook(payloadCrudo, firmaHeader, secretKey))
       return Unauthorized();
   ```
3. Solo si la firma es válida: extraer `IdTransaccionPasarela` y el estado del pago, y delegar en
   `Flujo` para confirmar el pedido (ver Skill `control-stock-dual` para el descuento de stock
   asociado).

## Flujo de integración (frontend — `perfumeria-tienda/features/checkout`)

1. El SPA llama a `POST /api/pago/iniciar` con el `idPedido`.
2. Con la respuesta, **redirige** al checkout hospedado de la pasarela (opción más simple y la que
   mantiene el alcance PCI mínimo) o monta su **SDK embebido** si se necesita una experiencia sin
   salir del sitio.
3. Al volver del checkout (o al detectar el evento de éxito del SDK), el SPA **MUST** consultar
   `GET /api/pedido/{id}` para confirmar el estado real — **nunca** confiar en un parámetro de la URL
   de retorno (`?estado=exitoso`) como fuente de verdad, porque puede manipularse.

## Manejo de dinero — reglas duras
- Todos los montos se manejan en colones (₡), como enteros o `decimal` sin arrastre de errores de
  punto flotante — nunca `float`/`double` para dinero.
- El monto que se envía a la pasarela **MUST** calcularse en el backend a partir de los
  `ItemPedido.PrecioUnitario * Cantidad` ya persistidos — nunca confiar en un total que el SPA calculó
  y envió (un cliente podría manipular el total en el navegador).

## Pruebas obligatorias para cualquier código que toque esta skill
- Firma de webhook inválida → rechazo sin efectos secundarios.
- Webhook duplicado (mismo `IdTransaccionPasarela`) → sin doble descuento de stock ni doble pedido
  pagado.
- Monto manipulado por el cliente → el backend recalcula desde `ItemPedido`, ignora cualquier total
  que venga del SPA.

## Referencias
- `docs/adr/ADR-004-pasarela-pago.md`
- ONVO Pay — `https://onvopay.com/devs`
- Tilopay — `https://www.tilopay.com/`
