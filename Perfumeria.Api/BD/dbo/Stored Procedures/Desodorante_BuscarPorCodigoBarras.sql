CREATE PROCEDURE [dbo].[Desodorante_BuscarPorCodigoBarras]
    @Codigo VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT D.[Id], M.[Nombre] AS Marca, D.[CodigoBarras], D.[IdPerfumeDerivado], D.[ImagenUrl],
           D.[Nombre], D.[Precio], D.[StockTienda], D.[StockVirtual], D.[Activo]
    FROM [dbo].[Desodorantes] D
    INNER JOIN [dbo].[Marcas] M ON M.[Id] = D.[IdMarca]
    WHERE D.[CodigoBarras] = @Codigo
       OR D.[CodigoBarras] LIKE '%' + @Codigo;
END
