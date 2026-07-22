namespace Abstracciones.Modelos.ItemEstuche;

public class ItemEstucheBase
{
    public string TipoProducto { get; set; } = string.Empty;
    public Guid IdProducto { get; set; }
    public Guid? IdVariante { get; set; }
}
