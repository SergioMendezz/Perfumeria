CREATE PROCEDURE [dbo].[Perfume_Obtener]
    @Pagina    INT,
    @Tamano    INT,
    @Genero    VARCHAR(20) = NULL,
    @Categoria VARCHAR(50) = NULL,
    @IdMarca   UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT P.[Id], M.[Nombre] AS Marca, P.[Nombre], P.[CodigoBarras], P.[Genero], P.[Categoria],
           P.[ImagenUrl], P.[Descripcion], P.[Activo],
           COUNT(*) OVER() AS Total
    FROM [dbo].[Perfumes] P
    INNER JOIN [dbo].[Marcas] M ON M.[Id] = P.[IdMarca]
    WHERE P.[Activo] = 1
      AND (@Genero IS NULL OR P.[Genero] = @Genero)
      AND (@Categoria IS NULL OR P.[Categoria] = @Categoria)
      AND (@IdMarca IS NULL OR P.[IdMarca] = @IdMarca)
    ORDER BY P.[Nombre]
    OFFSET (@Pagina - 1) * @Tamano ROWS
    FETCH NEXT @Tamano ROWS ONLY;
END
