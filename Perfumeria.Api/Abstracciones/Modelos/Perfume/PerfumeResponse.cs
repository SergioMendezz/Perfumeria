using Abstracciones.Modelos.VariantePerfume;

namespace Abstracciones.Modelos.Perfume;

public class PerfumeResponse
{
    public Guid Id { get; set; }
    public string Marca { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string CodigoBarras { get; set; } = string.Empty;
    public string Genero { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string ImagenUrl { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
    public VarianteResponse[] Variantes { get; set; } = [];
}
