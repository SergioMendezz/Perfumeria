using Abstracciones.Excepciones;
using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Interfaces.Servicios;
using Abstracciones.Modelos.Auth;
using Abstracciones.Modelos.Usuario;

namespace Flujo;

public class AuthFlujo : IAuthFlujo
{
    private readonly IUsuarioDA _usuarioDA;
    private readonly IJwtService _jwtService;

    public AuthFlujo(IUsuarioDA usuarioDA, IJwtService jwtService)
    {
        _usuarioDA = usuarioDA;
        _jwtService = jwtService;
    }

    public async Task<LoginResponse> Login(LoginRequest request)
    {
        var credenciales = await _usuarioDA.ObtenerCredencialesPorEmail(request.Email);

        if (credenciales is null
            || !BCrypt.Net.BCrypt.Verify(request.Password, credenciales.PasswordHash)
            || !credenciales.Activo)
        {
            throw new CredencialesInvalidasException();
        }

        await _usuarioDA.ActualizarUltimoAcceso(credenciales.Id);

        var usuario = new UsuarioResponse
        {
            Id = credenciales.Id,
            NombreUsuario = credenciales.NombreUsuario,
            Email = credenciales.Email,
            Rol = credenciales.Rol,
            Activo = credenciales.Activo
        };

        return new LoginResponse
        {
            Token = _jwtService.GenerarToken(usuario),
            Rol = credenciales.Rol
        };
    }
}
