using Abstracciones.Modelos.ItemPedido;

namespace Abstracciones.Modelos.Pedido;

public class PedidoRequest : PedidoBase
{
    public ItemPedidoRequest[] Items { get; set; } = [];
}
