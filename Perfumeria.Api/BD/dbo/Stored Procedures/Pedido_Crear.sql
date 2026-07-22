CREATE PROCEDURE [dbo].[Pedido_Crear]
    @IdCliente   UNIQUEIDENTIFIER,
    @Total       DECIMAL(10,2),
    @MetodoPago  VARCHAR(50),
    @ItemsJson   NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Id UNIQUEIDENTIFIER = NEWID();

    BEGIN TRANSACTION;

    INSERT INTO [dbo].[Pedidos] ([Id], [IdCliente], [Total], [MetodoPago])
    VALUES (@Id, @IdCliente, @Total, @MetodoPago);

    INSERT INTO [dbo].[ItemsPedido] ([Id], [IdPedido], [TipoProducto], [IdProducto], [IdVariante], [Cantidad], [PrecioUnitario])
    SELECT NEWID(), @Id, [TipoProducto], [IdProducto], [IdVariante], [Cantidad], [PrecioUnitario]
    FROM OPENJSON(@ItemsJson)
    WITH (
        [TipoProducto]   VARCHAR(30)      '$.TipoProducto',
        [IdProducto]     UNIQUEIDENTIFIER '$.IdProducto',
        [IdVariante]     UNIQUEIDENTIFIER '$.IdVariante',
        [Cantidad]       INT              '$.Cantidad',
        [PrecioUnitario] DECIMAL(10,2)    '$.PrecioUnitario'
    );

    COMMIT TRANSACTION;

    SELECT @Id AS Id;
END
