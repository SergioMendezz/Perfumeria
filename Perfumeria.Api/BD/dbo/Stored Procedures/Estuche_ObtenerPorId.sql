CREATE PROCEDURE [dbo].[Estuche_ObtenerPorId]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [Nombre], [ImagenUrl], [Precio], [StockTienda], [StockVirtual], [Activo]
    FROM [dbo].[Estuches]
    WHERE [Id] = @Id;

    SELECT [Id], [TipoProducto], [IdProducto], [IdVariante]
    FROM [dbo].[ItemsEstuche]
    WHERE [IdEstuche] = @Id;
END
