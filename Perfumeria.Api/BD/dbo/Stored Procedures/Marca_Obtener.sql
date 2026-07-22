CREATE PROCEDURE [dbo].[Marca_Obtener]
    @Pagina INT,
    @Tamano INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [Nombre], [PaisOrigen], [Descripcion], [LogoUrl], [Activo],
           COUNT(*) OVER() AS Total
    FROM [dbo].[Marcas]
    WHERE [Activo] = 1
    ORDER BY [Nombre]
    OFFSET (@Pagina - 1) * @Tamano ROWS
    FETCH NEXT @Tamano ROWS ONLY;
END
