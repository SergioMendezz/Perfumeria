CREATE PROCEDURE [dbo].[VariantePerfume_AjustarStockTienda]
    @Id       UNIQUEIDENTIFIER,
    @Cantidad INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    UPDATE [dbo].[VariantesPerfume]
    SET [StockTienda] = @Cantidad
    WHERE [Id] = @Id;

    COMMIT TRANSACTION;

    SELECT @Id AS Id;
END
