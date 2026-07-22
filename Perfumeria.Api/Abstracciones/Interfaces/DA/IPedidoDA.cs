using Abstracciones.Modelos.Pedido;

namespace Abstracciones.Interfaces.DA;

public interface IPedidoDA
{
    Task<PedidoResponse> Crear(PedidoRequest request, decimal total);
    Task<IEnumerable<PedidoResponse>> ObtenerMisPedidos(Guid idCliente);
    Task<PedidoResponse?> ObtenerPorId(Guid id);
    Task ActualizarEstado(Guid id, string estado, string? idTransaccionPasarela);
}
