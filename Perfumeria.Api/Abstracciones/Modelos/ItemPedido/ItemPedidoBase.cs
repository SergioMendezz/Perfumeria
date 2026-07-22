namespace Abstracciones.Modelos.ItemPedido;

public class ItemPedidoBase
{
    public string TipoProducto { get; set; } = string.Empty;
    public Guid IdProducto { get; set; }
    public Guid? IdVariante { get; set; }
    public int Cantidad { get; set; }
}
