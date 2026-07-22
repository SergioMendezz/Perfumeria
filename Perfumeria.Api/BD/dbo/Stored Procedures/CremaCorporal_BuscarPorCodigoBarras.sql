CREATE PROCEDURE [dbo].[CremaCorporal_BuscarPorCodigoBarras]
    @Codigo VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT C.[Id], M.[Nombre] AS Marca, C.[CodigoBarras], C.[IdPerfumeDerivado], C.[ImagenUrl],
           C.[Nombre], C.[Precio], C.[StockTienda], C.[StockVirtual], C.[Activo]
    FROM [dbo].[CremasCorporales] C
    INNER JOIN [dbo].[Marcas] M ON M.[Id] = C.[IdMarca]
    WHERE C.[CodigoBarras] = @Codigo
       OR C.[CodigoBarras] LIKE '%' + @Codigo;
END
