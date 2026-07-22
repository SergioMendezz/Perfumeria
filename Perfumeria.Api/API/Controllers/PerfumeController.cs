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
    public Task<IActionResult> ObtenerPorId(Guid id)
    {
        throw new NotImplementedException();
    }
}
