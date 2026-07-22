CREATE PROCEDURE [dbo].[Estuche_Crear]
    @Nombre       VARCHAR(200),
    @ImagenUrl    VARCHAR(500),
    @Precio       DECIMAL(10,2),
    @StockTienda  INT,
    @StockVirtual INT,
    @ItemsJson    NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Id UNIQUEIDENTIFIER = NEWID();

    BEGIN TRANSACTION;

    INSERT INTO [dbo].[Estuches] ([Id], [Nombre], [ImagenUrl], [Precio], [StockTienda], [StockVirtual])
    VALUES (@Id, @Nombre, @ImagenUrl, @Precio, @StockTienda, @StockVirtual);

    INSERT INTO [dbo].[ItemsEstuche] ([Id], [IdEstuche], [TipoProducto], [IdProducto], [IdVariante])
    SELECT NEWID(), @Id, [TipoProducto], [IdProducto], [IdVariante]
    FROM OPENJSON(@ItemsJson)
    WITH (
        [TipoProducto] VARCHAR(30)      '$.TipoProducto',
        [IdProducto]   UNIQUEIDENTIFIER '$.IdProducto',
        [IdVariante]   UNIQUEIDENTIFIER '$.IdVariante'
    );

    COMMIT TRANSACTION;

    SELECT @Id AS Id;
END
