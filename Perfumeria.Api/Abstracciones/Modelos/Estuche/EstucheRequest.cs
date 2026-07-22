using Abstracciones.Modelos.ItemEstuche;

namespace Abstracciones.Modelos.Estuche;

public class EstucheRequest : EstucheBase
{
    public int StockTienda { get; set; }
    public int StockVirtual { get; set; }
    public ItemEstucheRequest[] Items { get; set; } = [];
}
