using System.Data;
using Abstracciones.Interfaces.DA;
using Abstracciones.Modelos.Usuario;
using Dapper;

namespace DA;

public class UsuarioDA : IUsuarioDA
{
    private readonly IRepositorioDapper _repositorioDapper;

    public UsuarioDA(IRepositorioDapper repositorioDapper)
    {
        _repositorioDapper = repositorioDapper;
    }

    public Task<IEnumerable<UsuarioResponse>> ObtenerTodos(int pagina, int tamano)
    {
        throw new NotImplementedException();
    }

    public Task<UsuarioResponse?> ObtenerPorEmail(string email)
    {
        throw new NotImplementedException();
    }

    public Task<UsuarioCredenciales?> ObtenerCredencialesPorEmail(string email)
    {
        using var conexion = _repositorioDapper.ObtenerRepositorio();
        var filas = conexion.Query(
            "Usuario_ObtenerPorEmail",
            new { Email = email },
            commandType: CommandType.StoredProcedure).ToList();

        if (filas.Count == 0)
        {
            return Task.FromResult<UsuarioCredenciales?>(null);
        }

        var fila = filas[0];
        return Task.FromResult<UsuarioCredenciales?>(new UsuarioCredenciales
        {
            Id = fila.Id,
            NombreUsuario = fila.NombreUsuario,
            Email = fila.Email,
            PasswordHash = fila.PasswordHash,
            Rol = fila.Rol,
            Activo = fila.Activo
        });
    }

    public Task<UsuarioResponse?> ObtenerPorId(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<UsuarioResponse> Crear(UsuarioRequest request, string passwordHash)
    {
        throw new NotImplementedException();
    }

    public Task<UsuarioResponse> Editar(Guid id, UsuarioRequest request)
    {
        throw new NotImplementedException();
    }

    public Task Activar(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task ActualizarUltimoAcceso(Guid id)
    {
        using var conexion = _repositorioDapper.ObtenerRepositorio();
        conexion.Execute("Usuario_ActualizarUltimoAcceso", new { Id = id }, commandType: CommandType.StoredProcedure);
        return Task.CompletedTask;
    }
}
