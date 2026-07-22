CREATE PROCEDURE [dbo].[Perfume_Crear]
    @IdMarca      UNIQUEIDENTIFIER,
    @Nombre       VARCHAR(200),
    @CodigoBarras VARCHAR(50),
    @Genero       VARCHAR(20),
    @Categoria    VARCHAR(50),
    @ImagenUrl    VARCHAR(500),
    @Descripcion  TEXT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Id UNIQUEIDENTIFIER = NEWID();

    BEGIN TRANSACTION;

    INSERT INTO [dbo].[Perfumes]
        ([Id], [IdMarca], [Nombre], [CodigoBarras], [Genero], [Categoria], [ImagenUrl], [Descripcion])
    VALUES
        (@Id, @IdMarca, @Nombre, @CodigoBarras, @Genero, @Categoria, @ImagenUrl, @Descripcion);

    COMMIT TRANSACTION;

    SELECT @Id AS Id;
END
