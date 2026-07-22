namespace Abstracciones.Interfaces.Servicios;

public interface IPasarelaPagoService
{
    Task<string> IniciarCobro(Guid idPedido, decimal total);
    bool ValidarFirmaWebhook(string payload, string firma);
}
