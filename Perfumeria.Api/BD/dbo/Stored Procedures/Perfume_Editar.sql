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

    SELECT P.[Id], M.[Nombre] AS Marca, P.[Nombre], P.[CodigoBarras], P.[Genero], P.[Categoria],
           P.[ImagenUrl], P.[Descripcion], P.[Activo]
    FROM [dbo].[Perfumes] P
    INNER JOIN [dbo].[Marcas] M ON M.[Id] = P.[IdMarca]
    WHERE P.[Id] = @Id;
END
