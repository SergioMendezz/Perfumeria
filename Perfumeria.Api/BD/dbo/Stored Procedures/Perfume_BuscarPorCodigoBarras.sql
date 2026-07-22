CREATE PROCEDURE [dbo].[Perfume_BuscarPorCodigoBarras]
    @Codigo VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT P.[Id], M.[Nombre] AS Marca, P.[Nombre], P.[CodigoBarras], P.[Genero], P.[Categoria],
           P.[ImagenUrl], P.[Descripcion], P.[Activo]
    FROM [dbo].[Perfumes] P
    INNER JOIN [dbo].[Marcas] M ON M.[Id] = P.[IdMarca]
    WHERE P.[CodigoBarras] = @Codigo
       OR P.[CodigoBarras] LIKE '%' + @Codigo;
END
