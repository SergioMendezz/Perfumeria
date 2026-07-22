CREATE PROCEDURE [dbo].[Desodorante_Crear]
    @IdMarca           UNIQUEIDENTIFIER,
    @CodigoBarras      VARCHAR(50),
    @IdPerfumeDerivado UNIQUEIDENTIFIER = NULL,
    @ImagenUrl         VARCHAR(500),
    @Nombre            VARCHAR(200),
    @Precio            DECIMAL(10,2),
    @StockTienda       INT,
    @StockVirtual      INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Id UNIQUEIDENTIFIER = NEWID();

    BEGIN TRANSACTION;

    INSERT INTO [dbo].[Desodorantes]
        ([Id], [IdMarca], [CodigoBarras], [IdPerfumeDerivado], [ImagenUrl], [Nombre], [Precio], [StockTienda], [StockVirtual])
    VALUES
        (@Id, @IdMarca, @CodigoBarras, @IdPerfumeDerivado, @ImagenUrl, @Nombre, @Precio, @StockTienda, @StockVirtual);

    COMMIT TRANSACTION;

    SELECT @Id AS Id;
END
