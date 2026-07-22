using System.Data;
using Abstracciones.Interfaces.DA;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DA;

public class RepositorioDapper : IRepositorioDapper
{
    private readonly string _cadenaConexion;

    public RepositorioDapper(IConfiguration configuracion)
    {
        _cadenaConexion = configuracion.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'DefaultConnection'.");
    }

    public IDbConnection ObtenerRepositorio() => new SqlConnection(_cadenaConexion);
}
