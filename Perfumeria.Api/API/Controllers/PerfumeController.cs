using Abstracciones.Excepciones;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos.Perfume;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
[ApiController]
[Route("api/perfume")]
public class PerfumeController : ControllerBase
{
    private readonly IPerfumeFlujo _perfumeFlujo;

    public PerfumeController(IPerfumeFlujo perfumeFlujo)
    {
        _perfumeFlujo = perfumeFlujo;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos(int pagina, int tamano, string? genero, string? categoria, Guid? idMarca)
    {
        var resultado = await _perfumeFlujo.ObtenerTodos(pagina, tamano, genero, categoria, idMarca);
        return Ok(resultado);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(Guid id)
    {
        var resultado = await _perfumeFlujo.ObtenerPorId(id);
        return resultado is null ? NotFound() : Ok(resultado);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("buscar-codigo-barras")]
    public async Task<IActionResult> BuscarPorCodigoBarras(string codigo)
    {
        var resultado = await _perfumeFlujo.BuscarPorCodigoBarras(codigo);
        return Ok(resultado);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Crear(PerfumeRequest request)
    {
        try
        {
            var resultado = await _perfumeFlujo.Crear(request);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = resultado.Id }, resultado);
        }
        catch (CodigoBarrasDuplicadoException excepcion)
        {
            return Conflict(excepcion.Message);
        }
        catch (CategoriaInvalidaException excepcion)
        {
            return BadRequest(excepcion.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Editar(Guid id, PerfumeRequest request)
    {
        try
        {
            var resultado = await _perfumeFlujo.Editar(id, request);
            return Ok(resultado);
        }
        catch (PerfumeNoEncontradoException excepcion)
        {
            return NotFound(excepcion.Message);
        }
        catch (CodigoBarrasDuplicadoException excepcion)
        {
            return Conflict(excepcion.Message);
        }
    }
}
