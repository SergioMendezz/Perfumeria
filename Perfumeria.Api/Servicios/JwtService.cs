using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Abstracciones.Interfaces.Servicios;
using Abstracciones.Modelos.Usuario;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Servicios;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuracion;

    public JwtService(IConfiguration configuracion)
    {
        _configuracion = configuracion;
    }

    public string GenerarToken(UsuarioResponse usuario)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Name, usuario.NombreUsuario),
            new Claim(ClaimTypes.Role, usuario.Rol)
        };

        var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuracion["Jwt:Key"]!));
        var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);
        var expiraHoras = double.Parse(_configuracion["Jwt:ExpiraHoras"]!);

        var token = new JwtSecurityToken(
            issuer: _configuracion["Jwt:Issuer"],
            audience: _configuracion["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expiraHoras),
            signingCredentials: credenciales);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public Task RevocarToken(string token, Guid idUsuario, DateTime fechaExpira)
    {
        throw new NotImplementedException();
    }

    public Task<bool> EstaRevocado(string token)
    {
        throw new NotImplementedException();
    }
}
