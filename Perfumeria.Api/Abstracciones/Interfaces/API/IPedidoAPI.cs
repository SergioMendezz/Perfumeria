using Abstracciones.Modelos.Pedido;

namespace Abstracciones.Interfaces.API;

public interface IPedidoAPI
{
    Task<PedidoResponse> Crear(PedidoRequest request);
    Task<IEnumerable<PedidoResponse>> ObtenerMisPedidos(Guid idCliente);
    Task<PedidoResponse?> ObtenerPorId(Guid id);
}
