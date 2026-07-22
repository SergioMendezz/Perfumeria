CREATE PROCEDURE [dbo].[Usuario_Obtener]
    @Pagina INT,
    @Tamano INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [NombreUsuario], [Email], [Rol], [Activo], [FechaCreacion], [UltimoAcceso],
           COUNT(*) OVER() AS Total
    FROM [dbo].[Usuarios]
    WHERE [Rol] = 'Admin'
    ORDER BY [NombreUsuario]
    OFFSET (@Pagina - 1) * @Tamano ROWS
    FETCH NEXT @Tamano ROWS ONLY;
END
