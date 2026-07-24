using System.Reflection;
using Abstracciones.Excepciones;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos.Compartido;
using Abstracciones.Modelos.Perfume;
using API.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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

    // AC-03: Requiere autenticación de Admin
    [Fact]
    public void Editar_SinTokenValidoDeAdmin_ExigeRolAdmin()
    {
        // Arrange
        var metodo = typeof(PerfumeController).GetMethod(nameof(PerfumeController.Editar));

        // Act
        var atributoAutorizacion = metodo!.GetCustomAttribute<AuthorizeAttribute>();

        // Assert
        atributoAutorizacion.Should().NotBeNull();
        atributoAutorizacion!.Roles.Should().Be("Admin");
    }

    // AC-01: Edición exitosa de un perfume existente
    [Fact]
    public async Task Editar_PerfumeExistente_Retorna200ConPerfumeActualizado()
    {
        // Arrange
        var perfumeFlujo = Substitute.For<IPerfumeFlujo>();
        var id = Guid.Parse("b2c1a900-1111-4a2b-9c3d-000000000001");
        var request = new PerfumeRequest
        {
            IdMarca = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            Nombre = "Dior Sauvage Elixir",
            CodigoBarras = "7501234567890",
            Genero = "Hombre",
            Categoria = "Eau de Parfum",
            ImagenUrl = "https://cdn.example.com/dior-sauvage-elixir.jpg"
        };
        var respuestaEsperada = new PerfumeResponse
        {
            Id = id,
            Marca = "Dior",
            Nombre = request.Nombre,
            CodigoBarras = request.CodigoBarras,
            Genero = request.Genero,
            Categoria = request.Categoria,
            ImagenUrl = request.ImagenUrl,
            Activo = true
        };
        perfumeFlujo.Editar(id, request).Returns(respuestaEsperada);
        var sut = new PerfumeController(perfumeFlujo);

        // Act
        var respuesta = await sut.Editar(id, request);

        // Assert
        var resultadoOk = respuesta.Should().BeOfType<OkObjectResult>().Subject;
        resultadoOk.StatusCode.Should().Be(200);
        var perfumeActualizado = resultadoOk.Value.Should().BeOfType<PerfumeResponse>().Subject;
        perfumeActualizado.Nombre.Should().Be("Dior Sauvage Elixir");
        perfumeActualizado.ImagenUrl.Should().Be("https://cdn.example.com/dior-sauvage-elixir.jpg");
        perfumeActualizado.Should().BeSameAs(respuestaEsperada);
        await perfumeFlujo.Received(1).Editar(id, request);
    }

    // AC-02: Perfume inexistente
    [Fact]
    public async Task Editar_PerfumeInexistente_Retorna404()
    {
        // Arrange
        var perfumeFlujo = Substitute.For<IPerfumeFlujo>();
        var id = Guid.Parse("00000000-0000-0000-0000-000000000000");
        var request = new PerfumeRequest
        {
            IdMarca = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            Nombre = "Dior Sauvage Elixir",
            CodigoBarras = "7501234567890",
            Genero = "Hombre",
            Categoria = "Eau de Parfum",
            ImagenUrl = "https://cdn.example.com/dior-sauvage-elixir.jpg"
        };
        perfumeFlujo.Editar(id, request).ThrowsAsync(new PerfumeNoEncontradoException(id));
        var sut = new PerfumeController(perfumeFlujo);

        // Act
        var respuesta = await sut.Editar(id, request);

        // Assert
        var resultadoNoEncontrado = respuesta.Should().BeOfType<NotFoundObjectResult>().Subject;
        resultadoNoEncontrado.StatusCode.Should().Be(404);
    }

    // AC-04: Código de barras duplicado con otro perfume
    [Fact]
    public async Task Editar_CodigoBarrasDuplicadoConOtroPerfume_Retorna409()
    {
        // Arrange
        var perfumeFlujo = Substitute.For<IPerfumeFlujo>();
        var idPerfumeB = Guid.Parse("b2c1a900-1111-4a2b-9c3d-000000000002");
        var request = new PerfumeRequest
        {
            IdMarca = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            Nombre = "Dior Sauvage",
            CodigoBarras = "7501234567890",
            Genero = "Hombre",
            Categoria = "Eau de Parfum",
            ImagenUrl = "https://cdn.example.com/dior-sauvage.jpg"
        };
        perfumeFlujo.Editar(idPerfumeB, request)
            .ThrowsAsync(new CodigoBarrasDuplicadoException(request.CodigoBarras));
        var sut = new PerfumeController(perfumeFlujo);

        // Act
        var respuesta = await sut.Editar(idPerfumeB, request);

        // Assert
        var resultadoConflicto = respuesta.Should().BeOfType<ConflictObjectResult>().Subject;
        resultadoConflicto.StatusCode.Should().Be(409);
    }

    // AC-02: Código de barras duplicado
    [Fact]
    public async Task Crear_CodigoBarrasDuplicado_Retorna409()
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
        perfumeFlujo.Crear(request)
            .ThrowsAsync(new CodigoBarrasDuplicadoException(request.CodigoBarras));
        var sut = new PerfumeController(perfumeFlujo);

        // Act
        var respuesta = await sut.Crear(request);

        // Assert
        var resultadoConflicto = respuesta.Should().BeOfType<ConflictObjectResult>().Subject;
        resultadoConflicto.StatusCode.Should().Be(409);
    }

    // AC-05: Categoría inválida o abreviada rechazada
    [Fact]
    public async Task Crear_CategoriaAbreviadaOInvalida_Retorna400()
    {
        // Arrange
        var perfumeFlujo = Substitute.For<IPerfumeFlujo>();
        var request = new PerfumeRequest
        {
            IdMarca = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            Nombre = "Dior Sauvage",
            CodigoBarras = "7501234567890",
            Genero = "Hombre",
            Categoria = "EDP",
            ImagenUrl = "https://cdn.example.com/dior-sauvage.jpg"
        };
        perfumeFlujo.Crear(request)
            .ThrowsAsync(new CategoriaInvalidaException(request.Categoria));
        var sut = new PerfumeController(perfumeFlujo);

        // Act
        var respuesta = await sut.Crear(request);

        // Assert
        var resultadoInvalido = respuesta.Should().BeOfType<BadRequestObjectResult>().Subject;
        resultadoInvalido.StatusCode.Should().Be(400);
    }
}
