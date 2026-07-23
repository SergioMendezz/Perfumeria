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

    // AC-03: Requiere autenticación de Admin
    [Fact]
    public void Crear_SinTokenValidoDeAdmin_ExigeRolAdmin()
    {
        // Arrange
        var metodo = typeof(PerfumeController).GetMethod(nameof(PerfumeController.Crear));

        // Act
        var atributoAutorizacion = metodo!.GetCustomAttribute<AuthorizeAttribute>();

        // Assert
        atributoAutorizacion.Should().NotBeNull();
        atributoAutorizacion!.Roles.Should().Be("Admin");
    }

    // AC-01: Alta exitosa de un perfume nuevo
    [Fact]
    public async Task Crear_DatosValidos_Retorna201ConPerfumeCreado()
    {
        // Arrange
        var perfumeFlujo = Substitute.For<IPerfumeFlujo>();
        var request = new PerfumeRequest
        {
            IdMarca = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            Nombre = "Dior Sauvage",
            CodigoBarras = "7501234567890",
            Genero = "Hombre",
            Categoria = "Eau de Parfum",
            ImagenUrl = "https://cdn.example.com/dior-sauvage.jpg"
        };
        var respuestaEsperada = new PerfumeResponse
        {
            Id = Guid.NewGuid(),
            Marca = "Dior",
            Nombre = request.Nombre,
            CodigoBarras = request.CodigoBarras,
            Genero = request.Genero,
            Categoria = request.Categoria,
            ImagenUrl = request.ImagenUrl,
            Activo = true
        };
        perfumeFlujo.Crear(request).Returns(respuestaEsperada);
        var sut = new PerfumeController(perfumeFlujo);

        // Act
        var respuesta = await sut.Crear(request);

        // Assert
        var resultadoCreado = respuesta.Should().BeOfType<CreatedAtActionResult>().Subject;
        resultadoCreado.StatusCode.Should().Be(201);
        var perfumeCreado = resultadoCreado.Value.Should().BeOfType<PerfumeResponse>().Subject;
        perfumeCreado.Id.Should().NotBeEmpty();
        perfumeCreado.Activo.Should().BeTrue();
        perfumeCreado.Should().BeSameAs(respuestaEsperada);
        await perfumeFlujo.Received(1).Crear(request);
    }
}
