CREATE PROCEDURE [dbo].[VariantePerfume_Crear]
    @IdPerfume    UNIQUEIDENTIFIER,
    @Mililitros   DECIMAL(10,2),
    @Precio       DECIMAL(10,2),
    @StockTienda  INT,
    @StockVirtual INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Id UNIQUEIDENTIFIER = NEWID();

    BEGIN TRANSACTION;

    INSERT INTO [dbo].[VariantesPerfume]
        ([Id], [IdPerfume], [Mililitros], [Precio], [StockTienda], [StockVirtual])
    VALUES
        (@Id, @IdPerfume, @Mililitros, @Precio, @StockTienda, @StockVirtual);

    COMMIT TRANSACTION;

    SELECT @Id AS Id;
END
