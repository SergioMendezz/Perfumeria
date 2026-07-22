using Abstracciones.Modelos.Marca;

namespace Abstracciones.Interfaces.Flujo;

public interface IMarcaFlujo
{
    Task<IEnumerable<MarcaResponse>> ObtenerTodos(int pagina, int tamano);
    Task<MarcaResponse?> ObtenerPorId(Guid id);
    Task<MarcaResponse> Crear(MarcaRequest request);
    Task<MarcaResponse> Editar(Guid id, MarcaRequest request);
    Task Eliminar(Guid id);
    Task Activar(Guid id);
}
