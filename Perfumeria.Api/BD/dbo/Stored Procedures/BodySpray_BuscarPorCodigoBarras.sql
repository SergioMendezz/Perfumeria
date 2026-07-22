CREATE PROCEDURE [dbo].[BodySpray_BuscarPorCodigoBarras]
    @Codigo VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT B.[Id], M.[Nombre] AS Marca, B.[CodigoBarras], B.[IdPerfumeDerivado], B.[ImagenUrl],
           B.[Nombre], B.[Precio], B.[StockTienda], B.[StockVirtual], B.[Activo]
    FROM [dbo].[BodySprays] B
    INNER JOIN [dbo].[Marcas] M ON M.[Id] = B.[IdMarca]
    WHERE B.[CodigoBarras] = @Codigo
       OR B.[CodigoBarras] LIKE '%' + @Codigo;
END
