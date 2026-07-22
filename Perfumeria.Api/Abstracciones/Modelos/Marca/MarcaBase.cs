namespace Abstracciones.Modelos.Marca;

public class MarcaBase
{
    public string Nombre { get; set; } = string.Empty;
    public string? PaisOrigen { get; set; }
    public string? Descripcion { get; set; }
    public string? LogoUrl { get; set; }
}
