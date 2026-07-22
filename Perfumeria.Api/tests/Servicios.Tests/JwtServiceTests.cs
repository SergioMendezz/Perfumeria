using Abstracciones.Modelos.Usuario;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Servicios;

namespace Servicios.Tests;

public class JwtServiceTests
{
    // AC-01 (US-004): Login exitoso con credenciales válidas de Admin
    [Fact]
    public void GenerarToken_UsuarioAdminActivo_DevuelveTokenJwtNoVacioParaRolAdmin()
    {
        // Arrange
        var configuracion = Substitute.For<IConfiguration>();
        configuracion["Jwt:Key"].Returns("clave-secreta-de-pruebas-suficientemente-larga-1234567890");
        configuracion["Jwt:Issuer"].Returns("Perfumeria.Api");
        configuracion["Jwt:Audience"].Returns("Perfumeria.Clientes");
        configuracion["Jwt:ExpiraHoras"].Returns("8");

        var usuario = new UsuarioResponse
        {
            Id = Guid.NewGuid(),
            NombreUsuario = "Admin Tienda",
            Email = "admin@perfumeria.cr",
            Rol = "Admin",
            Activo = true
        };

        var sut = new JwtService(configuracion);

        // Act
        var token = sut.GenerarToken(usuario);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Split('.').Should().HaveCount(3);
    }
}
