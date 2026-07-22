CREATE PROCEDURE [dbo].[Usuario_ObtenerPorId]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [NombreUsuario], [Email], [PasswordHash], [PasswordSalt], [Rol], [Activo],
           [FechaCreacion], [UltimoAcceso]
    FROM [dbo].[Usuarios]
    WHERE [Id] = @Id;
END
