using Abstracciones.Modelos.Estuche;

namespace Abstracciones.Interfaces.API;

public interface IEstucheAPI
{
    Task<IEnumerable<EstucheResponse>> ObtenerTodos(int pagina, int tamano);
    Task<EstucheResponse?> ObtenerPorId(Guid id);
    Task<EstucheResponse> Crear(EstucheRequest request);
    Task<EstucheResponse> Editar(Guid id, EstucheRequest request);
    Task Eliminar(Guid id);
    Task Activar(Guid id);
}
