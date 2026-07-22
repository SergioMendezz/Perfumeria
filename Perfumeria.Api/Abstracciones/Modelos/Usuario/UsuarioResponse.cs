namespace Abstracciones.Modelos.Usuario;

public class UsuarioResponse : UsuarioBase
{
    public Guid Id { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? UltimoAcceso { get; set; }
}
