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
}
