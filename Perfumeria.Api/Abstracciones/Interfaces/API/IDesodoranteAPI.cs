using Abstracciones.Modelos.Desodorante;

namespace Abstracciones.Interfaces.API;

public interface IDesodoranteAPI
{
    Task<IEnumerable<DesodoranteResponse>> ObtenerTodos(int pagina, int tamano, Guid? idMarca);
    Task<DesodoranteResponse?> ObtenerPorId(Guid id);
    Task<DesodoranteResponse> Crear(DesodoranteRequest request);
    Task<DesodoranteResponse> Editar(Guid id, DesodoranteRequest request);
    Task Eliminar(Guid id);
    Task Activar(Guid id);
}
