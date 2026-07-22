using Abstracciones.Interfaces.Flujo;
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
}
