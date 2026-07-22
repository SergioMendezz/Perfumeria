namespace Abstracciones.Modelos.VariantePerfume;

public class VarianteResponse
{
    public Guid Id { get; set; }
    public decimal Mililitros { get; set; }
    public decimal Precio { get; set; }
    public int StockTienda { get; set; }
    public int StockVirtual { get; set; }
    public bool Activo { get; set; }
}
