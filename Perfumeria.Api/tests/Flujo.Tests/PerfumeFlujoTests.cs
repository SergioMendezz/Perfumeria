using Abstracciones.Interfaces.DA;
using Abstracciones.Modelos.Compartido;
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
        var sut = new PerfumeFlujo(perfumeDA);

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
        var sut = new PerfumeFlujo(perfumeDA);

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
        var sut = new PerfumeFlujo(perfumeDA);

        // Act
        var resultado = await sut.BuscarPorCodigoBarras(codigoBarras);

        // Assert
        resultado.Should().ContainSingle()
            .Which.CodigoBarras.Should().Be(codigoBarras);
        await perfumeDA.Received(1).BuscarPorCodigoBarras(codigoBarras);
    }
}
