CREATE PROCEDURE [dbo].[Pedido_ConfirmarPago]
    @IdPedido              UNIQUEIDENTIFIER,
    @IdTransaccionPasarela VARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    IF EXISTS (
        SELECT 1 FROM [dbo].[Pedidos]
        WHERE [Id] = @IdPedido AND [Estado] = 'Pagado' AND [IdTransaccionPasarela] = @IdTransaccionPasarela
    )
    BEGIN
        COMMIT TRANSACTION;
        SELECT 1 AS Confirmado, 'YaProcesado' AS Motivo;
        RETURN;
    END

    IF EXISTS (
        SELECT 1 FROM [dbo].[ItemsPedido] IP
        INNER JOIN [dbo].[VariantesPerfume] V ON V.[Id] = IP.[IdVariante]
        WHERE IP.[IdPedido] = @IdPedido AND IP.[TipoProducto] = 'Perfume' AND V.[StockVirtual] < IP.[Cantidad]
    )
    OR EXISTS (
        SELECT 1 FROM [dbo].[ItemsPedido] IP
        INNER JOIN [dbo].[Desodorantes] D ON D.[Id] = IP.[IdProducto]
        WHERE IP.[IdPedido] = @IdPedido AND IP.[TipoProducto] = 'Desodorante' AND D.[StockVirtual] < IP.[Cantidad]
    )
    OR EXISTS (
        SELECT 1 FROM [dbo].[ItemsPedido] IP
        INNER JOIN [dbo].[BodySprays] B ON B.[Id] = IP.[IdProducto]
        WHERE IP.[IdPedido] = @IdPedido AND IP.[TipoProducto] = 'BodySpray' AND B.[StockVirtual] < IP.[Cantidad]
    )
    OR EXISTS (
        SELECT 1 FROM [dbo].[ItemsPedido] IP
        INNER JOIN [dbo].[ShowerGeles] S ON S.[Id] = IP.[IdProducto]
        WHERE IP.[IdPedido] = @IdPedido AND IP.[TipoProducto] = 'ShowerGel' AND S.[StockVirtual] < IP.[Cantidad]
    )
    OR EXISTS (
        SELECT 1 FROM [dbo].[ItemsPedido] IP
        INNER JOIN [dbo].[CremasCorporales] C ON C.[Id] = IP.[IdProducto]
        WHERE IP.[IdPedido] = @IdPedido AND IP.[TipoProducto] = 'CremaCorporal' AND C.[StockVirtual] < IP.[Cantidad]
    )
    OR EXISTS (
        SELECT 1 FROM [dbo].[ItemsPedido] IP
        INNER JOIN [dbo].[Bodys] BO ON BO.[Id] = IP.[IdProducto]
        WHERE IP.[IdPedido] = @IdPedido AND IP.[TipoProducto] = 'Body' AND BO.[StockVirtual] < IP.[Cantidad]
    )
    BEGIN
        ROLLBACK TRANSACTION;
        SELECT 0 AS Confirmado, 'StockInsuficiente' AS Motivo;
        RETURN;
    END

    UPDATE V
    SET V.[StockVirtual] = V.[StockVirtual] - IP.[Cantidad]
    FROM [dbo].[VariantesPerfume] V
    INNER JOIN [dbo].[ItemsPedido] IP ON IP.[IdVariante] = V.[Id]
    WHERE IP.[IdPedido] = @IdPedido AND IP.[TipoProducto] = 'Perfume';

    UPDATE D
    SET D.[StockVirtual] = D.[StockVirtual] - IP.[Cantidad]
    FROM [dbo].[Desodorantes] D
    INNER JOIN [dbo].[ItemsPedido] IP ON IP.[IdProducto] = D.[Id]
    WHERE IP.[IdPedido] = @IdPedido AND IP.[TipoProducto] = 'Desodorante';

    UPDATE B
    SET B.[StockVirtual] = B.[StockVirtual] - IP.[Cantidad]
    FROM [dbo].[BodySprays] B
    INNER JOIN [dbo].[ItemsPedido] IP ON IP.[IdProducto] = B.[Id]
    WHERE IP.[IdPedido] = @IdPedido AND IP.[TipoProducto] = 'BodySpray';

    UPDATE S
    SET S.[StockVirtual] = S.[StockVirtual] - IP.[Cantidad]
    FROM [dbo].[ShowerGeles] S
    INNER JOIN [dbo].[ItemsPedido] IP ON IP.[IdProducto] = S.[Id]
    WHERE IP.[IdPedido] = @IdPedido AND IP.[TipoProducto] = 'ShowerGel';

    UPDATE C
    SET C.[StockVirtual] = C.[StockVirtual] - IP.[Cantidad]
    FROM [dbo].[CremasCorporales] C
    INNER JOIN [dbo].[ItemsPedido] IP ON IP.[IdProducto] = C.[Id]
    WHERE IP.[IdPedido] = @IdPedido AND IP.[TipoProducto] = 'CremaCorporal';

    UPDATE BO
    SET BO.[StockVirtual] = BO.[StockVirtual] - IP.[Cantidad]
    FROM [dbo].[Bodys] BO
    INNER JOIN [dbo].[ItemsPedido] IP ON IP.[IdProducto] = BO.[Id]
    WHERE IP.[IdPedido] = @IdPedido AND IP.[TipoProducto] = 'Body';

    UPDATE [dbo].[Pedidos]
    SET [Estado] = 'Pagado',
        [IdTransaccionPasarela] = @IdTransaccionPasarela
    WHERE [Id] = @IdPedido;

    COMMIT TRANSACTION;

    SELECT 1 AS Confirmado, 'Ok' AS Motivo;
END
