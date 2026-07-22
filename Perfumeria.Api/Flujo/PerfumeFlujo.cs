using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos.Compartido;
using Abstracciones.Modelos.Perfume;

namespace Flujo;

public class PerfumeFlujo : IPerfumeFlujo
{
    private readonly IPerfumeDA _perfumeDA;

    public PerfumeFlujo(IPerfumeDA perfumeDA)
    {
        _perfumeDA = perfumeDA;
    }

    public Task<ResultadoPaginado<PerfumeResponse>> ObtenerTodos(int pagina, int tamano, string? genero, string? categoria, Guid? idMarca)
    {
        return _perfumeDA.ObtenerTodos(pagina, tamano, genero, categoria, idMarca);
    }

    public Task<PerfumeResponse?> ObtenerPorId(Guid id)
    {
        return _perfumeDA.ObtenerPorId(id);
    }

    public Task<IEnumerable<PerfumeResponse>> BuscarPorCodigoBarras(string codigo)
    {
        throw new NotImplementedException();
    }

    public Task<PerfumeResponse> Crear(PerfumeRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<PerfumeResponse> Editar(Guid id, PerfumeRequest request)
    {
        throw new NotImplementedException();
    }

    public Task Eliminar(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task Activar(Guid id)
    {
        throw new NotImplementedException();
    }
}
