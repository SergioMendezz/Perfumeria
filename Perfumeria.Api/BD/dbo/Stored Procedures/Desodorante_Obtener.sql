CREATE PROCEDURE [dbo].[Desodorante_Obtener]
    @Pagina  INT,
    @Tamano  INT,
    @IdMarca UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT D.[Id], M.[Nombre] AS Marca, D.[CodigoBarras], D.[IdPerfumeDerivado], D.[ImagenUrl],
           D.[Nombre], D.[Precio], D.[StockTienda], D.[StockVirtual], D.[Activo],
           COUNT(*) OVER() AS Total
    FROM [dbo].[Desodorantes] D
    INNER JOIN [dbo].[Marcas] M ON M.[Id] = D.[IdMarca]
    WHERE D.[Activo] = 1
      AND (@IdMarca IS NULL OR D.[IdMarca] = @IdMarca)
    ORDER BY D.[Nombre]
    OFFSET (@Pagina - 1) * @Tamano ROWS
    FETCH NEXT @Tamano ROWS ONLY;
END
