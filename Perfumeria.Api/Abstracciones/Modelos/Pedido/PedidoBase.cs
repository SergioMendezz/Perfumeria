namespace Abstracciones.Modelos.Pedido;

public class PedidoBase
{
    public Guid IdCliente { get; set; }
    public string MetodoPago { get; set; } = string.Empty;
}
