using Abstracciones.Modelos.Usuario;

namespace Abstracciones.Interfaces.DA;

public interface IUsuarioDA
{
    Task<IEnumerable<UsuarioResponse>> ObtenerTodos(int pagina, int tamano);
    Task<UsuarioResponse?> ObtenerPorEmail(string email);
    Task<UsuarioResponse?> ObtenerPorId(Guid id);
    Task<UsuarioResponse> Crear(UsuarioRequest request, string passwordHash);
    Task<UsuarioResponse> Editar(Guid id, UsuarioRequest request);
    Task Activar(Guid id);
    Task ActualizarUltimoAcceso(Guid id);
}
