using Abstracciones.Excepciones;
using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Servicios;
using Abstracciones.Modelos.Auth;
using Abstracciones.Modelos.Usuario;
using FluentAssertions;
using global::Flujo;
using NSubstitute;

namespace Flujo.Tests;

public class AuthFlujoTests
{
    // AC-01: Login exitoso con credenciales válidas de Admin
    [Fact]
    public async Task Login_CredencialesValidasDeAdminActivo_DevuelveTokenConRolAdmin()
    {
        // Arrange
        const string email = "admin@perfumeria.cr";
        const string password = "Clave123!";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var usuarioDA = Substitute.For<IUsuarioDA>();
        usuarioDA.ObtenerCredencialesPorEmail(email).Returns(new UsuarioCredenciales
        {
            Id = Guid.NewGuid(),
            NombreUsuario = "Admin Tienda",
            Email = email,
            PasswordHash = passwordHash,
            Rol = "Admin",
            Activo = true
        });

        var jwtService = Substitute.For<IJwtService>();
        jwtService.GenerarToken(Arg.Any<UsuarioResponse>()).Returns("token-jwt-simulado");

        var sut = new AuthFlujo(usuarioDA, jwtService);

        // Act
        var resultado = await sut.Login(new LoginRequest { Email = email, Password = password });

        // Assert
        resultado.Token.Should().NotBeNullOrEmpty();
        resultado.Rol.Should().Be("Admin");
        await usuarioDA.Received(1).ObtenerCredencialesPorEmail(email);
        jwtService.Received(1).GenerarToken(Arg.Any<UsuarioResponse>());
    }

    // AC-02: Contraseña incorrecta
    [Fact]
    public async Task Login_ContrasenaIncorrecta_LanzaCredencialesInvalidasSinGenerarToken()
    {
        // Arrange
        const string email = "admin@perfumeria.cr";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Clave123!");

        var usuarioDA = Substitute.For<IUsuarioDA>();
        usuarioDA.ObtenerCredencialesPorEmail(email).Returns(new UsuarioCredenciales
        {
            Id = Guid.NewGuid(),
            NombreUsuario = "Admin Tienda",
            Email = email,
            PasswordHash = passwordHash,
            Rol = "Admin",
            Activo = true
        });

        var jwtService = Substitute.For<IJwtService>();
        var sut = new AuthFlujo(usuarioDA, jwtService);

        // Act
        var accion = () => sut.Login(new LoginRequest { Email = email, Password = "ContraseñaIncorrecta" });

        // Assert
        await accion.Should().ThrowAsync<CredencialesInvalidasException>();
        await usuarioDA.Received(1).ObtenerCredencialesPorEmail(email);
        jwtService.DidNotReceive().GenerarToken(Arg.Any<UsuarioResponse>());
    }

    // AC-03: Cuenta inactiva no puede iniciar sesión
    [Fact]
    public async Task Login_CuentaInactivaConContrasenaCorrecta_LanzaCredencialesInvalidasSinGenerarToken()
    {
        // Arrange
        const string email = "exempleado@perfumeria.cr";
        const string password = "Clave123!";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var usuarioDA = Substitute.For<IUsuarioDA>();
        usuarioDA.ObtenerCredencialesPorEmail(email).Returns(new UsuarioCredenciales
        {
            Id = Guid.NewGuid(),
            NombreUsuario = "Ex Empleado",
            Email = email,
            PasswordHash = passwordHash,
            Rol = "Admin",
            Activo = false
        });

        var jwtService = Substitute.For<IJwtService>();
        var sut = new AuthFlujo(usuarioDA, jwtService);

        // Act
        var accion = () => sut.Login(new LoginRequest { Email = email, Password = password });

        // Assert
        await accion.Should().ThrowAsync<CredencialesInvalidasException>();
        await usuarioDA.Received(1).ObtenerCredencialesPorEmail(email);
        jwtService.DidNotReceive().GenerarToken(Arg.Any<UsuarioResponse>());
    }

    // AC-04: Correo no registrado
    [Fact]
    public async Task Login_CorreoNoRegistrado_LanzaCredencialesInvalidasSinGenerarToken()
    {
        // Arrange
        const string email = "inexistente@perfumeria.cr";

        var usuarioDA = Substitute.For<IUsuarioDA>();
        usuarioDA.ObtenerCredencialesPorEmail(email).Returns((UsuarioCredenciales?)null);

        var jwtService = Substitute.For<IJwtService>();
        var sut = new AuthFlujo(usuarioDA, jwtService);

        // Act
        var accion = () => sut.Login(new LoginRequest { Email = email, Password = "CualquierContraseña" });

        // Assert
        await accion.Should().ThrowAsync<CredencialesInvalidasException>();
        await usuarioDA.Received(1).ObtenerCredencialesPorEmail(email);
        jwtService.DidNotReceive().GenerarToken(Arg.Any<UsuarioResponse>());
    }

    // AC-05: Actualiza el último acceso en un login exitoso
    [Fact]
    public async Task Login_LoginExitoso_ActualizaUltimoAccesoDelUsuario()
    {
        // Arrange
        const string email = "admin@perfumeria.cr";
        const string password = "Clave123!";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        var idUsuario = Guid.NewGuid();

        var usuarioDA = Substitute.For<IUsuarioDA>();
        usuarioDA.ObtenerCredencialesPorEmail(email).Returns(new UsuarioCredenciales
        {
            Id = idUsuario,
            NombreUsuario = "Admin Tienda",
            Email = email,
            PasswordHash = passwordHash,
            Rol = "Admin",
            Activo = true
        });

        var jwtService = Substitute.For<IJwtService>();
        jwtService.GenerarToken(Arg.Any<UsuarioResponse>()).Returns("token-jwt-simulado");

        var sut = new AuthFlujo(usuarioDA, jwtService);

        // Act
        await sut.Login(new LoginRequest { Email = email, Password = password });

        // Assert
        await usuarioDA.Received(1).ActualizarUltimoAcceso(idUsuario);
    }
}
