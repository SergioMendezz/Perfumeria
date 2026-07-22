CREATE PROCEDURE [dbo].[Marca_Crear]
    @Nombre      VARCHAR(200),
    @PaisOrigen  VARCHAR(100) = NULL,
    @Descripcion TEXT = NULL,
    @LogoUrl     VARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Id UNIQUEIDENTIFIER = NEWID();

    BEGIN TRANSACTION;

    INSERT INTO [dbo].[Marcas] ([Id], [Nombre], [PaisOrigen], [Descripcion], [LogoUrl])
    VALUES (@Id, @Nombre, @PaisOrigen, @Descripcion, @LogoUrl);

    COMMIT TRANSACTION;

    SELECT @Id AS Id;
END
