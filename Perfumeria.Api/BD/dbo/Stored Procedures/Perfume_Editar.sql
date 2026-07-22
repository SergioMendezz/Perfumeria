CREATE PROCEDURE [dbo].[Perfume_Editar]
    @Id           UNIQUEIDENTIFIER,
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

    BEGIN TRANSACTION;

    UPDATE [dbo].[Perfumes]
    SET [IdMarca] = @IdMarca,
        [Nombre] = @Nombre,
        [CodigoBarras] = @CodigoBarras,
        [Genero] = @Genero,
        [Categoria] = @Categoria,
        [ImagenUrl] = @ImagenUrl,
        [Descripcion] = @Descripcion
    WHERE [Id] = @Id;

    COMMIT TRANSACTION;

    SELECT @Id AS Id;
END
