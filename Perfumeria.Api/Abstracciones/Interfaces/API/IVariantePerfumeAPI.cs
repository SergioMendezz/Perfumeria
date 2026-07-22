using Abstracciones.Modelos.VariantePerfume;

namespace Abstracciones.Interfaces.API;

public interface IVariantePerfumeAPI
{
    Task<VarianteResponse> Agregar(VariantePerfumeRequest request);
    Task AjustarStockTienda(Guid id, int cantidad);
    Task AjustarStockVirtual(Guid id, int cantidad);
}
