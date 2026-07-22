using Abstracciones.Modelos.Usuario;

namespace Abstracciones.Interfaces.Servicios;

public interface IJwtService
{
    string GenerarToken(UsuarioResponse usuario);
    Task RevocarToken(string token, Guid idUsuario, DateTime fechaExpira);
    Task<bool> EstaRevocado(string token);
}
