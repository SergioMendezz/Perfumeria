using Abstracciones.Modelos.CremaCorporal;

namespace Abstracciones.Interfaces.Flujo;

public interface ICremaCorporalFlujo
{
    Task<IEnumerable<CremaCorporalResponse>> ObtenerTodos(int pagina, int tamano, Guid? idMarca);
    Task<CremaCorporalResponse?> ObtenerPorId(Guid id);
    Task<CremaCorporalResponse> Crear(CremaCorporalRequest request);
    Task<CremaCorporalResponse> Editar(Guid id, CremaCorporalRequest request);
    Task Eliminar(Guid id);
    Task Activar(Guid id);
}
