using Abstracciones.Modelos.Body;

namespace Abstracciones.Interfaces.DA;

public interface IBodyDA
{
    Task<IEnumerable<BodyResponse>> ObtenerTodos(int pagina, int tamano, Guid? idMarca);
    Task<BodyResponse?> ObtenerPorId(Guid id);
    Task<BodyResponse> Crear(BodyRequest request);
    Task<BodyResponse> Editar(Guid id, BodyRequest request);
    Task Eliminar(Guid id);
    Task Activar(Guid id);
}
