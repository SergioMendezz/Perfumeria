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

    SELECT P.[Id], M.[Nombre] AS Marca, P.[Nombre], P.[CodigoBarras], P.[Genero], P.[Categoria],
           P.[ImagenUrl], P.[Descripcion], P.[Activo]
    FROM [dbo].[Perfumes] P
    INNER JOIN [dbo].[Marcas] M ON M.[Id] = P.[IdMarca]
    WHERE P.[Id] = @Id;
END
