using Abstracciones.Excepciones;
using Abstracciones.Interfaces.DA;
using Abstracciones.Modelos.Compartido;
using Abstracciones.Modelos.Marca;
using Abstracciones.Modelos.Perfume;
using FluentAssertions;
using global::Flujo;
using NSubstitute;

namespace Flujo.Tests;

public class PerfumeFlujoTests
{
    // AC-01: Listado paginado de perfumes activos
    [Fact]
    public async Task ObtenerTodos_VeinticincoPerfumesActivosPaginaUnoTamanoVeinte_DevuelveVeintePerfumesConTotalVeinticinco()
    {
        // Arrange
        var perfumeDA = Substitute.For<IPerfumeDA>();
        var paginaEsperada = Enumerable.Range(1, 20)
            .Select(_ => new PerfumeResponse())
            .ToList();
        perfumeDA.ObtenerTodos(1, 20, null, null, null)
            .Returns(new ResultadoPaginado<PerfumeResponse> { Items = paginaEsperada, Total = 25 });
        var marcaDA = Substitute.For<IMarcaDA>();
        var sut = new PerfumeFlujo(perfumeDA, marcaDA);

        // Act
        var resultado = await sut.ObtenerTodos(1, 20, null, null, null);

        // Assert
        resultado.Items.Should().HaveCount(20);
        resultado.Total.Should().Be(25);
        await perfumeDA.Received(1).ObtenerTodos(1, 20, null, null, null);
    }

    // AC-04: Categoría con nombre completo, nunca sigla
    [Fact]
    public async Task ObtenerPorId_PerfumeConCategoriaEauDeParfumEnBaseDeDatos_DevuelveCategoriaCompletaSinSigla()
    {
        // Arrange
        var id = Guid.NewGuid();
        var perfumeDA = Substitute.For<IPerfumeDA>();
        perfumeDA.ObtenerPorId(id)
            .Returns(new PerfumeResponse { Id = id, Categoria = "Eau de Parfum" });
        var marcaDA = Substitute.For<IMarcaDA>();
        var sut = new PerfumeFlujo(perfumeDA, marcaDA);

        // Act
        var resultado = await sut.ObtenerPorId(id);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Categoria.Should().Be("Eau de Parfum");
        resultado.Categoria.Should().NotBe("EDP");
        await perfumeDA.Received(1).ObtenerPorId(id);
    }

    // AC-01 (US-002): Búsqueda por código de barras completo
    [Fact]
    public async Task BuscarPorCodigoBarras_CodigoCompletoCoincideConUnPerfumeExistente_DevuelvePerfumeExacto()
    {
        // Arrange
        const string codigoBarras = "7501234567890";
        var perfumeDA = Substitute.For<IPerfumeDA>();
        var perfumeEsperado = new PerfumeResponse { CodigoBarras = codigoBarras, Nombre = "Chanel N. 5" };
        perfumeDA.BuscarPorCodigoBarras(codigoBarras)
            .Returns(new List<PerfumeResponse> { perfumeEsperado });
        var marcaDA = Substitute.For<IMarcaDA>();
        var sut = new PerfumeFlujo(perfumeDA, marcaDA);

        // Act
        var resultado = await sut.BuscarPorCodigoBarras(codigoBarras);

        // Assert
        resultado.Should().ContainSingle()
            .Which.CodigoBarras.Should().Be(codigoBarras);
        await perfumeDA.Received(1).BuscarPorCodigoBarras(codigoBarras);
    }

    // AC-01: Alta exitosa de un perfume nuevo
    [Fact]
    public async Task Crear_DatosValidosYMarcaActiva_CreaPerfumeConIdYActivoTrue()
    {
        // Arrange
        var idMarca = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
        var request = new PerfumeRequest
        {
            IdMarca = idMarca,
            Nombre = "Dior Sauvage",
            CodigoBarras = "7501234567890",
            Genero = "Hombre",
            Categoria = "Eau de Parfum",
            ImagenUrl = "https://cdn.example.com/dior-sauvage.jpg"
        };

        var marcaDA = Substitute.For<IMarcaDA>();
        marcaDA.ObtenerPorId(idMarca)
            .Returns(new MarcaResponse { Id = idMarca, Nombre = "Dior", Activo = true });

        var perfumeDA = Substitute.For<IPerfumeDA>();
        perfumeDA.Crear(Arg.Any<PerfumeRequest>()).Returns(callInfo =>
        {
            var recibido = callInfo.Arg<PerfumeRequest>();
            return new PerfumeResponse
            {
                Id = Guid.NewGuid(),
                Marca = "Dior",
                Nombre = recibido.Nombre,
                CodigoBarras = recibido.CodigoBarras,
                Genero = recibido.Genero,
                Categoria = recibido.Categoria,
                ImagenUrl = recibido.ImagenUrl,
                Activo = true
            };
        });

        var sut = new PerfumeFlujo(perfumeDA, marcaDA);

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
        await marcaDA.Received(1).ObtenerPorId(idMarca);
        await perfumeDA.Received(1).Crear(Arg.Is<PerfumeRequest>(r =>
            r.IdMarca == idMarca &&
            r.Nombre == "Dior Sauvage" &&
            r.CodigoBarras == "7501234567890"));
    }

    // AC-04: Nombre y Marca normalizados a mayúscula inicial tras cada espacio
    [Fact]
    public async Task Crear_NombreEnMinusculas_PersisteNombreConMayusculaInicialTrasCadaEspacio()
    {
        // Arrange
        var idMarca = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
        var request = new PerfumeRequest
        {
            IdMarca = idMarca,
            Nombre = "dior sauvage",
            CodigoBarras = "7501234567891",
            Genero = "Hombre",
            Categoria = "Eau de Parfum",
            ImagenUrl = "https://cdn.example.com/dior-sauvage.jpg"
        };

        var marcaDA = Substitute.For<IMarcaDA>();
        marcaDA.ObtenerPorId(idMarca)
            .Returns(new MarcaResponse { Id = idMarca, Nombre = "Dior", Activo = true });

        var perfumeDA = Substitute.For<IPerfumeDA>();
        perfumeDA.Crear(Arg.Any<PerfumeRequest>()).Returns(callInfo =>
        {
            var recibido = callInfo.Arg<PerfumeRequest>();
            return new PerfumeResponse
            {
                Id = Guid.NewGuid(),
                Marca = "Dior",
                Nombre = recibido.Nombre,
                CodigoBarras = recibido.CodigoBarras,
                Genero = recibido.Genero,
                Categoria = recibido.Categoria,
                ImagenUrl = recibido.ImagenUrl,
                Activo = true
            };
        });

        var sut = new PerfumeFlujo(perfumeDA, marcaDA);

        // Act
        await sut.Crear(request);

        // Assert
        await perfumeDA.Received(1).Crear(Arg.Is<PerfumeRequest>(r => r.Nombre == "Dior Sauvage"));
    }

    // AC-02: Código de barras duplicado
    [Fact]
    public async Task Crear_CodigoBarrasDuplicado_LanzaExcepcionYNoCreaPerfume()
    {
        // Arrange
        const string codigoBarras = "7501234567890";
        var idMarca = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
        var request = new PerfumeRequest
        {
            IdMarca = idMarca,
            Nombre = "Dior Sauvage",
            CodigoBarras = codigoBarras,
            Genero = "Hombre",
            Categoria = "Eau de Parfum",
            ImagenUrl = "https://cdn.example.com/dior-sauvage.jpg"
        };

        var marcaDA = Substitute.For<IMarcaDA>();
        marcaDA.ObtenerPorId(idMarca)
            .Returns(new MarcaResponse { Id = idMarca, Nombre = "Dior", Activo = true });

        var perfumeDA = Substitute.For<IPerfumeDA>();
        perfumeDA.BuscarPorCodigoBarras(codigoBarras)
            .Returns(new List<PerfumeResponse> { new() { CodigoBarras = codigoBarras } });

        var sut = new PerfumeFlujo(perfumeDA, marcaDA);

        // Act
        Func<Task> accion = () => sut.Crear(request);

        // Assert
        await accion.Should().ThrowAsync<CodigoBarrasDuplicadoException>();
        await perfumeDA.Received(0).Crear(Arg.Any<PerfumeRequest>());
    }

    // AC-05: Categoría inválida o abreviada rechazada
    [Fact]
    public async Task Crear_CategoriaAbreviadaOInvalida_LanzaExcepcionYNoCreaPerfume()
    {
        // Arrange
        var idMarca = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
        var request = new PerfumeRequest
        {
            IdMarca = idMarca,
            Nombre = "Dior Sauvage",
            CodigoBarras = "7501234567892",
            Genero = "Hombre",
            Categoria = "EDP",
            ImagenUrl = "https://cdn.example.com/dior-sauvage.jpg"
        };

        var marcaDA = Substitute.For<IMarcaDA>();
        marcaDA.ObtenerPorId(idMarca)
            .Returns(new MarcaResponse { Id = idMarca, Nombre = "Dior", Activo = true });

        var perfumeDA = Substitute.For<IPerfumeDA>();

        var sut = new PerfumeFlujo(perfumeDA, marcaDA);

        // Act
        Func<Task> accion = () => sut.Crear(request);

        // Assert
        await accion.Should().ThrowAsync<CategoriaInvalidaException>();
        await perfumeDA.Received(0).Crear(Arg.Any<PerfumeRequest>());
    }
}
