CREATE PROCEDURE [dbo].[CremaCorporal_Obtener]
    @Pagina  INT,
    @Tamano  INT,
    @IdMarca UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT C.[Id], M.[Nombre] AS Marca, C.[CodigoBarras], C.[IdPerfumeDerivado], C.[ImagenUrl],
           C.[Nombre], C.[Precio], C.[StockTienda], C.[StockVirtual], C.[Activo],
           COUNT(*) OVER() AS Total
    FROM [dbo].[CremasCorporales] C
    INNER JOIN [dbo].[Marcas] M ON M.[Id] = C.[IdMarca]
    WHERE C.[Activo] = 1
      AND (@IdMarca IS NULL OR C.[IdMarca] = @IdMarca)
    ORDER BY C.[Nombre]
    OFFSET (@Pagina - 1) * @Tamano ROWS
    FETCH NEXT @Tamano ROWS ONLY;
END
