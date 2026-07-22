using System.Reflection;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos.Compartido;
using Abstracciones.Modelos.Perfume;
using API.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace API.Tests;

public class PerfumeControllerTests
{
    // AC-03: Sin necesidad de autenticación
    [Theory]
    [InlineData(nameof(PerfumeController.ObtenerTodos))]
    [InlineData(nameof(PerfumeController.ObtenerPorId))]
    public void Endpoint_SinHeaderAuthorization_PermiteAccesoAnonimo(string nombreMetodo)
    {
        // Arrange
        var metodo = typeof(PerfumeController).GetMethod(nombreMetodo);

        // Act
        var permiteAnonimo = metodo!.GetCustomAttribute<AllowAnonymousAttribute>() is not null;

        // Assert
        permiteAnonimo.Should().BeTrue();
    }

    // AC-01: Listado paginado de perfumes activos
    [Fact]
    public async Task ObtenerTodos_ParametrosRecibidos_InvocaPerfumeFlujoYDevuelveOkConElResultado()
    {
        // Arrange
        var perfumeFlujo = Substitute.For<IPerfumeFlujo>();
        var resultadoEsperado = new ResultadoPaginado<PerfumeResponse>
        {
            Items = [new PerfumeResponse { Nombre = "Chanel N. 5" }],
            Total = 25
        };
        perfumeFlujo.ObtenerTodos(1, 20, "Mujer", "Eau de Parfum", null)
            .Returns(resultadoEsperado);
        var sut = new PerfumeController(perfumeFlujo);

        // Act
        var respuesta = await sut.ObtenerTodos(1, 20, "Mujer", "Eau de Parfum", null);

        // Assert
        var resultadoOk = respuesta.Should().BeOfType<OkObjectResult>().Subject;
        resultadoOk.Value.Should().BeSameAs(resultadoEsperado);
        await perfumeFlujo.Received(1).ObtenerTodos(1, 20, "Mujer", "Eau de Parfum", null);
    }

    // AC-04 (US-002): Requiere autenticación de Admin
    [Fact]
    public void BuscarPorCodigoBarras_SinTokenDeRolAdmin_ExigeRolAdmin()
    {
        // Arrange
        var metodo = typeof(PerfumeController).GetMethod(nameof(PerfumeController.BuscarPorCodigoBarras));

        // Act
        var atributoAutorizacion = metodo!.GetCustomAttribute<AuthorizeAttribute>();

        // Assert
        atributoAutorizacion.Should().NotBeNull();
        atributoAutorizacion!.Roles.Should().Be("Admin");
    }
}
