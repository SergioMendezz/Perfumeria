CREATE PROCEDURE [dbo].[Estuche_Editar]
    @Id           UNIQUEIDENTIFIER,
    @Nombre       VARCHAR(200),
    @ImagenUrl    VARCHAR(500),
    @Precio       DECIMAL(10,2),
    @StockTienda  INT,
    @StockVirtual INT,
    @ItemsJson    NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    UPDATE [dbo].[Estuches]
    SET [Nombre] = @Nombre,
        [ImagenUrl] = @ImagenUrl,
        [Precio] = @Precio,
        [StockTienda] = @StockTienda,
        [StockVirtual] = @StockVirtual
    WHERE [Id] = @Id;

    DELETE FROM [dbo].[ItemsEstuche] WHERE [IdEstuche] = @Id;

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
