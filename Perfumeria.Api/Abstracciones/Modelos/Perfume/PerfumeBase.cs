namespace Abstracciones.Modelos.Perfume;

public class PerfumeBase
{
    public Guid IdMarca { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string CodigoBarras { get; set; } = string.Empty;
    public string Genero { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string ImagenUrl { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}
