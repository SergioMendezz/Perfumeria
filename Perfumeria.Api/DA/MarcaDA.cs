using System.Data;
using Abstracciones.Interfaces.DA;
using Abstracciones.Modelos.Marca;
using Dapper;

namespace DA;

public class MarcaDA : IMarcaDA
{
    private readonly IRepositorioDapper _repositorioDapper;

    public MarcaDA(IRepositorioDapper repositorioDapper)
    {
        _repositorioDapper = repositorioDapper;
    }

    public Task<IEnumerable<MarcaResponse>> ObtenerTodos(int pagina, int tamano)
    {
        using var conexion = _repositorioDapper.ObtenerRepositorio();
        var parametros = new { Pagina = pagina, Tamano = tamano };
        var filas = conexion.Query("Marca_Obtener", parametros, commandType: CommandType.StoredProcedure).ToList();

        IEnumerable<MarcaResponse> items = filas.Select(MapearFila).ToList();

        return Task.FromResult(items);
    }

    public Task<MarcaResponse?> ObtenerPorId(Guid id)
    {
        using var conexion = _repositorioDapper.ObtenerRepositorio();
        var fila = conexion.QueryFirstOrDefault(
            "Marca_ObtenerPorId",
            new { Id = id },
            commandType: CommandType.StoredProcedure);

        if (fila is null)
        {
            return Task.FromResult<MarcaResponse?>(null);
        }

        return Task.FromResult<MarcaResponse?>(MapearFila(fila));
    }

    public async Task<MarcaResponse> Crear(MarcaRequest request)
    {
        using var conexion = _repositorioDapper.ObtenerRepositorio();
        var parametros = new
        {
            request.Nombre,
            request.PaisOrigen,
            request.Descripcion,
            request.LogoUrl
        };
        var idCreado = await conexion.QueryFirstAsync<Guid>(
            "Marca_Crear",
            parametros,
            commandType: CommandType.StoredProcedure);

        return await ObtenerPorId(idCreado) ?? throw new InvalidOperationException("No se pudo obtener la marca creada.");
    }

    public async Task<MarcaResponse> Editar(Guid id, MarcaRequest request)
    {
        using var conexion = _repositorioDapper.ObtenerRepositorio();
        var parametros = new
        {
            Id = id,
            request.Nombre,
            request.PaisOrigen,
            request.Descripcion,
            request.LogoUrl
        };
        await conexion.QueryFirstAsync<Guid>(
            "Marca_Editar",
            parametros,
            commandType: CommandType.StoredProcedure);

        return await ObtenerPorId(id) ?? throw new InvalidOperationException("No se pudo obtener la marca editada.");
    }

    public Task Eliminar(Guid id)
    {
        using var conexion = _repositorioDapper.ObtenerRepositorio();
        return conexion.ExecuteAsync(
            "Marca_Eliminar",
            new { Id = id },
            commandType: CommandType.StoredProcedure);
    }

    public Task Activar(Guid id)
    {
        using var conexion = _repositorioDapper.ObtenerRepositorio();
        return conexion.ExecuteAsync(
            "Marca_Activar",
            new { Id = id },
            commandType: CommandType.StoredProcedure);
    }

    private static MarcaResponse MapearFila(dynamic fila) => new()
    {
        Id = fila.Id,
        Nombre = fila.Nombre,
        PaisOrigen = fila.PaisOrigen,
        Descripcion = fila.Descripcion,
        LogoUrl = fila.LogoUrl,
        Activo = fila.Activo
    };
}
