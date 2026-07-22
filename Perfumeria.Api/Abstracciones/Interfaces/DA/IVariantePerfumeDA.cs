using Abstracciones.Modelos.VariantePerfume;

namespace Abstracciones.Interfaces.DA;

public interface IVariantePerfumeDA
{
    Task<VarianteResponse> Agregar(VariantePerfumeRequest request);
    Task<VarianteResponse?> ObtenerPorId(Guid id);
    Task AjustarStockTienda(Guid id, int cantidad);
    Task AjustarStockVirtual(Guid id, int cantidad);
}
