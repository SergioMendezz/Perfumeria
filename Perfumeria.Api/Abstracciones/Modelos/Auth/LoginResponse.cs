namespace Abstracciones.Modelos.Auth;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
}
