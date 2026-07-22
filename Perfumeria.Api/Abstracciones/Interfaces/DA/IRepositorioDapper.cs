using System.Data;

namespace Abstracciones.Interfaces.DA;

public interface IRepositorioDapper
{
    IDbConnection ObtenerRepositorio();
}
