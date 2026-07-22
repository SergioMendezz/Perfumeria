namespace Abstracciones.Modelos.Usuario;

public class UsuarioRequest : UsuarioBase
{
    public string Password { get; set; } = string.Empty;
}
