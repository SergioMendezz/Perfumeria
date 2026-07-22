CREATE PROCEDURE [dbo].[Pedido_ObtenerMisPedidos]
    @IdCliente UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [IdCliente], [FechaPedido], [Estado], [Total], [MetodoPago], [IdTransaccionPasarela]
    FROM [dbo].[Pedidos]
    WHERE [IdCliente] = @IdCliente
    ORDER BY [FechaPedido] DESC;
END
