using Abstracciones.Modelos.Estuche;

namespace Abstracciones.Interfaces.DA;

public interface IEstucheDA
{
    Task<IEnumerable<EstucheResponse>> ObtenerTodos(int pagina, int tamano);
    Task<EstucheResponse?> ObtenerPorId(Guid id);
    Task<EstucheResponse> Crear(EstucheRequest request);
    Task<EstucheResponse> Editar(Guid id, EstucheRequest request);
    Task Eliminar(Guid id);
    Task Activar(Guid id);
}
