using Abstracciones.Modelos.Usuario;

namespace Abstracciones.Interfaces.Flujo;

public interface IUsuarioFlujo
{
    Task<IEnumerable<UsuarioResponse>> ObtenerTodos(int pagina, int tamano);
    Task<UsuarioResponse> RegistrarCliente(UsuarioRequest request);
    Task<UsuarioResponse> CrearAdmin(UsuarioRequest request);
    Task<UsuarioResponse> Editar(Guid id, UsuarioRequest request);
    Task Activar(Guid id);
}
