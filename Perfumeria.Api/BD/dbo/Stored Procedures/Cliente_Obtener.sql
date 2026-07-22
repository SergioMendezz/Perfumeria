CREATE PROCEDURE [dbo].[Cliente_Obtener]
    @Pagina INT,
    @Tamano INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [NombreUsuario], [Email], [Activo], [FechaCreacion], [UltimoAcceso],
           COUNT(*) OVER() AS Total
    FROM [dbo].[Usuarios]
    WHERE [Rol] = 'Cliente'
    ORDER BY [NombreUsuario]
    OFFSET (@Pagina - 1) * @Tamano ROWS
    FETCH NEXT @Tamano ROWS ONLY;
END
