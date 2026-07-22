using Abstracciones.Modelos.ItemPedido;

namespace Abstracciones.Modelos.Pedido;

public class PedidoResponse : PedidoBase
{
    public Guid Id { get; set; }
    public DateTime FechaPedido { get; set; }
    public string Estado { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string? IdTransaccionPasarela { get; set; }
    public ItemPedidoResponse[] Items { get; set; } = [];
}
