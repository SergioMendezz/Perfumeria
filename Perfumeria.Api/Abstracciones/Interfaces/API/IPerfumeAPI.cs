using Abstracciones.Modelos.Compartido;
using Abstracciones.Modelos.Perfume;

namespace Abstracciones.Interfaces.API;

public interface IPerfumeAPI
{
    Task<ResultadoPaginado<PerfumeResponse>> ObtenerTodos(int pagina, int tamano, string? genero, string? categoria, Guid? idMarca);
    Task<PerfumeResponse?> ObtenerPorId(Guid id);
    Task<IEnumerable<PerfumeResponse>> BuscarPorCodigoBarras(string codigo);
    Task<PerfumeResponse> Crear(PerfumeRequest request);
    Task<PerfumeResponse> Editar(Guid id, PerfumeRequest request);
    Task Eliminar(Guid id);
    Task Activar(Guid id);
}
