CREATE PROCEDURE [dbo].[Estuche_Obtener]
    @Pagina INT,
    @Tamano INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [Nombre], [ImagenUrl], [Precio], [StockTienda], [StockVirtual], [Activo],
           COUNT(*) OVER() AS Total
    FROM [dbo].[Estuches]
    WHERE [Activo] = 1
    ORDER BY [Nombre]
    OFFSET (@Pagina - 1) * @Tamano ROWS
    FETCH NEXT @Tamano ROWS ONLY;
END
