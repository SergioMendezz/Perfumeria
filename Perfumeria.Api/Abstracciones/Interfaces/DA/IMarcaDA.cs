using Abstracciones.Modelos.Marca;

namespace Abstracciones.Interfaces.DA;

public interface IMarcaDA
{
    Task<IEnumerable<MarcaResponse>> ObtenerTodos(int pagina, int tamano);
    Task<MarcaResponse?> ObtenerPorId(Guid id);
    Task<MarcaResponse> Crear(MarcaRequest request);
    Task<MarcaResponse> Editar(Guid id, MarcaRequest request);
    Task Eliminar(Guid id);
    Task Activar(Guid id);
}
