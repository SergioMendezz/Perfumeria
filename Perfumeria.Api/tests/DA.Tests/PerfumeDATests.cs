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

    // AC-01 (US-002): Búsqueda por código de barras completo
    [Fact]
    public async Task BuscarPorCodigoBarras_CodigoCompletoCoincideConPerfumeExistente_DevuelvePerfumeExacto()
    {
        // Arrange
        const string codigoBarras = "7501234567890";
        var columnas = new[] { "Id", "Marca", "Nombre", "CodigoBarras", "Genero", "Categoria", "ImagenUrl", "Descripcion", "Activo" };
        var valoresFila = new object[]
        {
            Guid.NewGuid(), "Chanel", "Chanel N. 5", codigoBarras, "Mujer", "Eau de Parfum", "http://imagen", "Descripcion", true
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

        var sut = new PerfumeDA(repositorioDapper);

        // Act
        var resultado = await sut.BuscarPorCodigoBarras(codigoBarras);

        // Assert
        resultado.Should().ContainSingle()
            .Which.CodigoBarras.Should().Be(codigoBarras);
        repositorioDapper.Received(1).ObtenerRepositorio();
    }

    // AC-04 (US-001): Categoría con nombre completo, nunca sigla
    [Fact]
    public async Task ObtenerPorId_PerfumeConCategoriaEauDeParfumEnBaseDeDatos_DevuelveCategoriaCompletaSinSigla()
    {
        // Arrange
        var id = Guid.NewGuid();
        var columnas = new[] { "Id", "Marca", "Nombre", "CodigoBarras", "Genero", "Categoria", "ImagenUrl", "Descripcion", "Activo" };
        var valoresFila = new object[]
        {
            id, "Chanel", "Chanel N. 5", "7501234567890", "Mujer", "Eau de Parfum", "http://imagen", "Descripcion", true
        };

        var filaActual = -1;
        var lector = Substitute.For<IDataReader>();
        lector.FieldCount.Returns(columnas.Length);
        lector.GetName(Arg.Any<int>()).Returns(ci => columnas[ci.Arg<int>()]);
        lector.Read().Returns(_ => { filaActual++; return filaActual == 0; });
        lector.GetValue(Arg.Any<int>()).Returns(ci => valoresFila[ci.Arg<int>()]);
        lector.IsDBNull(Arg.Any<int>()).Returns(false);
        lector.NextResult().Returns(false);

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
        var resultado = await sut.ObtenerPorId(id);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Categoria.Should().Be("Eau de Parfum");
        resultado.Categoria.Should().NotBe("EDP");
        repositorioDapper.Received(1).ObtenerRepositorio();
    }
}
