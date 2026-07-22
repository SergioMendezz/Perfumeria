CREATE PROCEDURE [dbo].[Pedido_ObtenerPorId]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [IdCliente], [FechaPedido], [Estado], [Total], [MetodoPago], [IdTransaccionPasarela]
    FROM [dbo].[Pedidos]
    WHERE [Id] = @Id;

    SELECT [Id], [TipoProducto], [IdProducto], [IdVariante], [Cantidad], [PrecioUnitario]
    FROM [dbo].[ItemsPedido]
    WHERE [IdPedido] = @Id;
END
