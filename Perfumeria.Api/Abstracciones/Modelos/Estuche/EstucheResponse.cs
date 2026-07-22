using Abstracciones.Modelos.ItemEstuche;

namespace Abstracciones.Modelos.Estuche;

public class EstucheResponse : EstucheBase
{
    public Guid Id { get; set; }
    public int StockTienda { get; set; }
    public int StockVirtual { get; set; }
    public bool Activo { get; set; }
    public ItemEstucheResponse[] Items { get; set; } = [];
}
