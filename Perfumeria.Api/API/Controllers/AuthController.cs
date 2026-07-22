using Abstracciones.Excepciones;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthFlujo _authFlujo;

    public AuthController(IAuthFlujo authFlujo)
    {
        _authFlujo = authFlujo;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            var resultado = await _authFlujo.Login(request);
            return Ok(resultado);
        }
        catch (CredencialesInvalidasException excepcion)
        {
            return Unauthorized(new { mensaje = excepcion.Message });
        }
    }
}
