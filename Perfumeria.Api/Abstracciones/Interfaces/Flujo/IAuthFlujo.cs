using Abstracciones.Modelos.Auth;

namespace Abstracciones.Interfaces.Flujo;

public interface IAuthFlujo
{
    Task<LoginResponse> Login(LoginRequest request);
}
