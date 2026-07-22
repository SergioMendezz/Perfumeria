CREATE PROCEDURE [dbo].[ShowerGel_BuscarPorCodigoBarras]
    @Codigo VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT S.[Id], M.[Nombre] AS Marca, S.[CodigoBarras], S.[IdPerfumeDerivado], S.[ImagenUrl],
           S.[Nombre], S.[Precio], S.[StockTienda], S.[StockVirtual], S.[Activo]
    FROM [dbo].[ShowerGeles] S
    INNER JOIN [dbo].[Marcas] M ON M.[Id] = S.[IdMarca]
    WHERE S.[CodigoBarras] = @Codigo
       OR S.[CodigoBarras] LIKE '%' + @Codigo;
END
