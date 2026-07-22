CREATE PROCEDURE [dbo].[ShowerGel_ObtenerPorId]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT S.[Id], M.[Nombre] AS Marca, S.[CodigoBarras], S.[IdPerfumeDerivado], S.[ImagenUrl],
           S.[Nombre], S.[Precio], S.[StockTienda], S.[StockVirtual], S.[Activo]
    FROM [dbo].[ShowerGeles] S
    INNER JOIN [dbo].[Marcas] M ON M.[Id] = S.[IdMarca]
    WHERE S.[Id] = @Id;
END
