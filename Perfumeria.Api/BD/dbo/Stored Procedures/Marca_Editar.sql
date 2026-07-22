CREATE PROCEDURE [dbo].[Marca_Editar]
    @Id          UNIQUEIDENTIFIER,
    @Nombre      VARCHAR(200),
    @PaisOrigen  VARCHAR(100) = NULL,
    @Descripcion TEXT = NULL,
    @LogoUrl     VARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    UPDATE [dbo].[Marcas]
    SET [Nombre] = @Nombre,
        [PaisOrigen] = @PaisOrigen,
        [Descripcion] = @Descripcion,
        [LogoUrl] = @LogoUrl
    WHERE [Id] = @Id;

    COMMIT TRANSACTION;

    SELECT @Id AS Id;
END
