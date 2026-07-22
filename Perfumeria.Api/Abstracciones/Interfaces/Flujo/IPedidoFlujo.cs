using Abstracciones.Modelos.Pedido;

namespace Abstracciones.Interfaces.Flujo;

public interface IPedidoFlujo
{
    Task<PedidoResponse> Crear(PedidoRequest request);
    Task<IEnumerable<PedidoResponse>> ObtenerMisPedidos(Guid idCliente);
    Task<PedidoResponse?> ObtenerPorId(Guid id);
}
