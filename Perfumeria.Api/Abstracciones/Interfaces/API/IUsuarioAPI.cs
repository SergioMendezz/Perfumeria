using Abstracciones.Modelos.Usuario;

namespace Abstracciones.Interfaces.API;

public interface IUsuarioAPI
{
    Task<IEnumerable<UsuarioResponse>> ObtenerTodos(int pagina, int tamano);
    Task<UsuarioResponse> CrearAdmin(UsuarioRequest request);
    Task<UsuarioResponse> Editar(Guid id, UsuarioRequest request);
    Task Activar(Guid id);
}
