using Abstracciones.Modelos.VariantePerfume;

namespace Abstracciones.Interfaces.Flujo;

public interface IVariantePerfumeFlujo
{
    Task<VarianteResponse> Agregar(VariantePerfumeRequest request);
    Task AjustarStockTienda(Guid id, int cantidad);
    Task AjustarStockVirtual(Guid id, int cantidad);
}
