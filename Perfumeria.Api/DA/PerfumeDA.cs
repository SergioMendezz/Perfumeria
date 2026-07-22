using System.Data;
using Abstracciones.Interfaces.DA;
using Abstracciones.Modelos.Compartido;
using Abstracciones.Modelos.Perfume;
using Dapper;

namespace DA;

public class PerfumeDA : IPerfumeDA
{
    private readonly IRepositorioDapper _repositorioDapper;

    public PerfumeDA(IRepositorioDapper repositorioDapper)
    {
        _repositorioDapper = repositorioDapper;
    }

    public Task<ResultadoPaginado<PerfumeResponse>> ObtenerTodos(int pagina, int tamano, string? genero, string? categoria, Guid? idMarca)
    {
        using var conexion = _repositorioDapper.ObtenerRepositorio();
        var parametros = new { Pagina = pagina, Tamano = tamano, Genero = genero, Categoria = categoria, IdMarca = idMarca };
        var filas = conexion.Query("Perfume_Obtener", parametros, commandType: CommandType.StoredProcedure).ToList();

        var items = filas.Select(fila => new PerfumeResponse
        {
            Id = fila.Id,
            Marca = fila.Marca,
            Nombre = fila.Nombre,
            CodigoBarras = fila.CodigoBarras,
            Genero = fila.Genero,
            Categoria = fila.Categoria,
            ImagenUrl = fila.ImagenUrl,
            Descripcion = fila.Descripcion,
            Activo = fila.Activo
        }).ToList();

        var total = filas.Count > 0 ? (int)filas[0].Total : 0;

        return Task.FromResult(new ResultadoPaginado<PerfumeResponse> { Items = items, Total = total });
    }

    public Task<PerfumeResponse?> ObtenerPorId(Guid id)
    {
        throw new NotImplementedException();
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
