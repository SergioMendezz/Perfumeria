CREATE PROCEDURE [dbo].[VariantePerfume_AjustarStockVirtual]
    @Id       UNIQUEIDENTIFIER,
    @Cantidad INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    UPDATE [dbo].[VariantesPerfume]
    SET [StockVirtual] = @Cantidad
    WHERE [Id] = @Id;

    COMMIT TRANSACTION;

    SELECT @Id AS Id;
END
