CREATE PROCEDURE [dbo].[Desodorante_Editar]
    @Id                UNIQUEIDENTIFIER,
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

    BEGIN TRANSACTION;

    UPDATE [dbo].[Desodorantes]
    SET [IdMarca] = @IdMarca,
        [CodigoBarras] = @CodigoBarras,
        [IdPerfumeDerivado] = @IdPerfumeDerivado,
        [ImagenUrl] = @ImagenUrl,
        [Nombre] = @Nombre,
        [Precio] = @Precio,
        [StockTienda] = @StockTienda,
        [StockVirtual] = @StockVirtual
    WHERE [Id] = @Id;

    COMMIT TRANSACTION;

    SELECT @Id AS Id;
END
