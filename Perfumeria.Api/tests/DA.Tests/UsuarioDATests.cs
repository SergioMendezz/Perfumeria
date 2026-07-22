using System.Data;
using Abstracciones.Interfaces.DA;
using DA;
using FluentAssertions;
using NSubstitute;

namespace DA.Tests;

public class UsuarioDATests
{
    // AC-01 (US-004): Login exitoso con credenciales válidas de Admin
    [Fact]
    public async Task ObtenerCredencialesPorEmail_CorreoRegistradoDeAdminActivo_DevuelveCredencialesConHashYRolAdmin()
    {
        // Arrange
        const string email = "admin@perfumeria.cr";
        var columnas = new[]
        {
            "Id", "NombreUsuario", "Email", "PasswordHash", "PasswordSalt", "Rol", "Activo", "FechaCreacion", "UltimoAcceso"
        };
        var valoresFila = new object[]
        {
            Guid.NewGuid(), "Admin Tienda", email, "$2a$11$hashSimulado", "", "Admin", true, DateTime.UtcNow, DateTime.UtcNow
        };

        var filaActual = -1;
        var lector = Substitute.For<IDataReader>();
        lector.FieldCount.Returns(columnas.Length);
        lector.GetName(Arg.Any<int>()).Returns(ci => columnas[ci.Arg<int>()]);
        lector.Read().Returns(_ => { filaActual++; return filaActual == 0; });
        lector.GetValue(Arg.Any<int>()).Returns(ci => valoresFila[ci.Arg<int>()]);
        lector.IsDBNull(Arg.Any<int>()).Returns(false);

        var comando = Substitute.For<IDbCommand>();
        comando.Parameters.Returns(Substitute.For<IDataParameterCollection>());
        comando.CreateParameter().Returns(_ => Substitute.For<IDbDataParameter>());
        comando.ExecuteReader(Arg.Any<CommandBehavior>()).Returns(lector);

        var conexion = Substitute.For<IDbConnection>();
        conexion.CreateCommand().Returns(comando);

        var repositorioDapper = Substitute.For<IRepositorioDapper>();
        repositorioDapper.ObtenerRepositorio().Returns(conexion);

        var sut = new UsuarioDA(repositorioDapper);

        // Act
        var resultado = await sut.ObtenerCredencialesPorEmail(email);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Email.Should().Be(email);
        resultado.PasswordHash.Should().Be("$2a$11$hashSimulado");
        resultado.Rol.Should().Be("Admin");
        resultado.Activo.Should().BeTrue();
        repositorioDapper.Received(1).ObtenerRepositorio();
    }

    // AC-05 (US-004): Actualiza el último acceso en un login exitoso
    [Fact]
    public async Task ActualizarUltimoAcceso_IdDeUsuarioValido_EjecutaActualizacionViaRepositorio()
    {
        // Arrange
        var idUsuario = Guid.NewGuid();

        var comando = Substitute.For<IDbCommand>();
        comando.Parameters.Returns(Substitute.For<IDataParameterCollection>());
        comando.CreateParameter().Returns(_ => Substitute.For<IDbDataParameter>());
        comando.ExecuteNonQuery().Returns(1);

        var conexion = Substitute.For<IDbConnection>();
        conexion.CreateCommand().Returns(comando);

        var repositorioDapper = Substitute.For<IRepositorioDapper>();
        repositorioDapper.ObtenerRepositorio().Returns(conexion);

        var sut = new UsuarioDA(repositorioDapper);

        // Act
        await sut.ActualizarUltimoAcceso(idUsuario);

        // Assert
        repositorioDapper.Received(1).ObtenerRepositorio();
        comando.Received(1).ExecuteNonQuery();
    }
}
