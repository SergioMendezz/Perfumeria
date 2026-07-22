namespace Abstracciones.Modelos.Desodorante;

public class DesodoranteBase
{
    public Guid IdMarca { get; set; }
    public string CodigoBarras { get; set; } = string.Empty;
    public Guid? IdPerfumeDerivado { get; set; }
    public string ImagenUrl { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
}
