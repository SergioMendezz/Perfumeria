using Abstracciones.Modelos.ShowerGel;

namespace Abstracciones.Interfaces.API;

public interface IShowerGelAPI
{
    Task<IEnumerable<ShowerGelResponse>> ObtenerTodos(int pagina, int tamano, Guid? idMarca);
    Task<ShowerGelResponse?> ObtenerPorId(Guid id);
    Task<ShowerGelResponse> Crear(ShowerGelRequest request);
    Task<ShowerGelResponse> Editar(Guid id, ShowerGelRequest request);
    Task Eliminar(Guid id);
    Task Activar(Guid id);
}
