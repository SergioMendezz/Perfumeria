using Abstracciones.Excepciones;
using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos.Compartido;
using Abstracciones.Modelos.Perfume;
using Flujo.Normalizacion;

namespace Flujo;

public class PerfumeFlujo : IPerfumeFlujo
{
    private readonly IPerfumeDA _perfumeDA;
    private readonly IMarcaDA _marcaDA;

    public PerfumeFlujo(IPerfumeDA perfumeDA, IMarcaDA marcaDA)
    {
        _perfumeDA = perfumeDA;
        _marcaDA = marcaDA;
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
        return _perfumeDA.BuscarPorCodigoBarras(codigo);
    }

    public async Task<PerfumeResponse> Crear(PerfumeRequest request)
    {
        await ValidarYNormalizarPerfume(request, idExcluido: null);
        return await _perfumeDA.Crear(request);
    }

    public async Task<PerfumeResponse> Editar(Guid id, PerfumeRequest request)
    {
        await ValidarExistencia(id);
        await ValidarYNormalizarPerfume(request, idExcluido: id);
        return await _perfumeDA.Editar(id, request);
    }

    private async Task ValidarExistencia(Guid id)
    {
        var perfumeExistente = await _perfumeDA.ObtenerPorId(id);
        if (perfumeExistente is null)
        {
            throw new PerfumeNoEncontradoException(id);
        }
    }

    private async Task ValidarYNormalizarPerfume(PerfumeRequest request, Guid? idExcluido)
    {
        await ValidarCodigoBarrasNoDuplicado(request.CodigoBarras, idExcluido);
        ValidarCategoria(request.Categoria);
        request.Nombre = NormalizadorNombreProducto.Normalizar(request.Nombre);
        await _marcaDA.ObtenerPorId(request.IdMarca);
    }

    private async Task ValidarCodigoBarrasNoDuplicado(string codigoBarras, Guid? idExcluido)
    {
        var coincidencias = await _perfumeDA.BuscarPorCodigoBarras(codigoBarras);
        if (coincidencias.Any(p => p.CodigoBarras == codigoBarras && p.Id != idExcluido))
        {
            throw new CodigoBarrasDuplicadoException(codigoBarras);
        }
    }

    private static void ValidarCategoria(string categoria)
    {
        if (!CategoriasPerfume.Validas.Contains(categoria))
        {
            throw new CategoriaInvalidaException(categoria);
        }
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
