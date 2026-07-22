namespace Abstracciones.Modelos.ItemPedido;

public class ItemPedidoResponse : ItemPedidoBase
{
    public Guid Id { get; set; }
    public decimal PrecioUnitario { get; set; }
}
