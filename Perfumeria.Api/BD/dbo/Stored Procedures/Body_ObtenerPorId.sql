CREATE PROCEDURE [dbo].[Body_ObtenerPorId]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT B.[Id], M.[Nombre] AS Marca, B.[CodigoBarras], B.[IdPerfumeDerivado], B.[ImagenUrl],
           B.[Nombre], B.[Precio], B.[StockTienda], B.[StockVirtual], B.[Activo]
    FROM [dbo].[Bodys] B
    INNER JOIN [dbo].[Marcas] M ON M.[Id] = B.[IdMarca]
    WHERE B.[Id] = @Id;
END
