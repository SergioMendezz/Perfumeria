namespace Abstracciones.Modelos.CremaCorporal;

public class CremaCorporalResponse
{
    public Guid Id { get; set; }
    public string Marca { get; set; } = string.Empty;
    public string CodigoBarras { get; set; } = string.Empty;
    public Guid? IdPerfumeDerivado { get; set; }
    public string ImagenUrl { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int StockTienda { get; set; }
    public int StockVirtual { get; set; }
    public bool Activo { get; set; }
}
