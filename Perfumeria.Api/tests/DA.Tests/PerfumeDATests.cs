using System.Data;
using Abstracciones.Interfaces.DA;
using DA;
using FluentAssertions;
using NSubstitute;

namespace DA.Tests;

public class PerfumeDATests
{
    // AC-02: Catálogo vacío
    [Fact]
    public async Task ObtenerTodos_SinPerfumesActivosRegistrados_DevuelveListaVaciaConTotalCero()
    {
        // Arrange
        var lector = Substitute.For<IDataReader>();
        lector.Read().Returns(false);
        lector.FieldCount.Returns(0);

        var comando = Substitute.For<IDbCommand>();
        comando.Parameters.Returns(Substitute.For<IDataParameterCollection>());
        comando.CreateParameter().Returns(_ => Substitute.For<IDbDataParameter>());
        comando.ExecuteReader(Arg.Any<CommandBehavior>()).Returns(lector);

        var conexion = Substitute.For<IDbConnection>();
        conexion.CreateCommand().Returns(comando);

        var repositorioDapper = Substitute.For<IRepositorioDapper>();
        repositorioDapper.ObtenerRepositorio().Returns(conexion);

        var sut = new PerfumeDA(repositorioDapper);

        // Act
        var resultado = await sut.ObtenerTodos(1, 20, null, null, null);

        // Assert
        resultado.Items.Should().BeEmpty();
        resultado.Total.Should().Be(0);
        repositorioDapper.Received(1).ObtenerRepositorio();
    }
}
