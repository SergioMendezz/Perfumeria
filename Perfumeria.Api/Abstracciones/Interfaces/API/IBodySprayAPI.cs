using Abstracciones.Modelos.BodySpray;

namespace Abstracciones.Interfaces.API;

public interface IBodySprayAPI
{
    Task<IEnumerable<BodySprayResponse>> ObtenerTodos(int pagina, int tamano, Guid? idMarca);
    Task<BodySprayResponse?> ObtenerPorId(Guid id);
    Task<BodySprayResponse> Crear(BodySprayRequest request);
    Task<BodySprayResponse> Editar(Guid id, BodySprayRequest request);
    Task Eliminar(Guid id);
    Task Activar(Guid id);
}
