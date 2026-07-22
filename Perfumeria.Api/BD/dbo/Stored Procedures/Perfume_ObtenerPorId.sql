CREATE PROCEDURE [dbo].[Perfume_ObtenerPorId]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT P.[Id], M.[Nombre] AS Marca, P.[Nombre], P.[CodigoBarras], P.[Genero], P.[Categoria],
           P.[ImagenUrl], P.[Descripcion], P.[Activo]
    FROM [dbo].[Perfumes] P
    INNER JOIN [dbo].[Marcas] M ON M.[Id] = P.[IdMarca]
    WHERE P.[Id] = @Id;

    SELECT [Id], [Mililitros], [Precio], [StockTienda], [StockVirtual], [Activo]
    FROM [dbo].[VariantesPerfume]
    WHERE [IdPerfume] = @Id;
END
