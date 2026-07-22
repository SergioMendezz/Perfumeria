CREATE PROCEDURE [dbo].[Usuario_ObtenerPorEmail]
    @Email VARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [NombreUsuario], [Email], [PasswordHash], [PasswordSalt], [Rol], [Activo],
           [FechaCreacion], [UltimoAcceso]
    FROM [dbo].[Usuarios]
    WHERE [Email] = @Email;
END
