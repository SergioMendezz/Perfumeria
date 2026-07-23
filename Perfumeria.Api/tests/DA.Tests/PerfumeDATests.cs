using System.Data;
using Abstracciones.Interfaces.DA;
using Abstracciones.Modelos.Perfume;
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

    // AC-01: Alta exitosa de un perfume nuevo
    [Fact]
    public async Task Crear_DatosValidos_InvocaStoredProcedureYRetornaPerfumeConIdYActivoTrue()
    {
        // Arrange
        var idMarca = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
        var idGenerado = Guid.NewGuid();
        var request = new PerfumeRequest
        {
            IdMarca = idMarca,
            Nombre = "Dior Sauvage",
            CodigoBarras = "7501234567890",
            Genero = "Hombre",
            Categoria = "Eau de Parfum",
            ImagenUrl = "https://cdn.example.com/dior-sauvage.jpg"
        };

        var columnas = new[] { "Id", "Marca", "Nombre", "CodigoBarras", "Genero", "Categoria", "ImagenUrl", "Descripcion", "Activo" };
        var valoresFila = new object[]
        {
            idGenerado, "Dior", request.Nombre, request.CodigoBarras, request.Genero, request.Categoria, request.ImagenUrl, DBNull.Value, true
        };

        var filaActual = -1;
        var lector = Substitute.For<IDataReader>();
        lector.FieldCount.Returns(columnas.Length);
        lector.GetName(Arg.Any<int>()).Returns(ci => columnas[ci.Arg<int>()]);
        lector.Read().Returns(_ => { filaActual++; return filaActual == 0; });
        lector.GetValue(Arg.Any<int>()).Returns(ci => valoresFila[ci.Arg<int>()]);
        lector.IsDBNull(Arg.Any<int>()).Returns(ci => valoresFila[ci.Arg<int>()] == DBNull.Value);
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
        var resultado = await sut.Crear(request);

        // Assert
        resultado.Id.Should().NotBeEmpty();
        resultado.Activo.Should().BeTrue();
        resultado.Nombre.Should().Be("Dior Sauvage");
        resultado.CodigoBarras.Should().Be("7501234567890");
        resultado.Genero.Should().Be("Hombre");
        resultado.Categoria.Should().Be("Eau de Parfum");
        resultado.ImagenUrl.Should().Be("https://cdn.example.com/dior-sauvage.jpg");
        repositorioDapper.Received(1).ObtenerRepositorio();
    }
}
