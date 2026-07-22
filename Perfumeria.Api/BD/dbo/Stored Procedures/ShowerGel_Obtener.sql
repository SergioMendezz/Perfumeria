CREATE PROCEDURE [dbo].[ShowerGel_Obtener]
    @Pagina  INT,
    @Tamano  INT,
    @IdMarca UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT S.[Id], M.[Nombre] AS Marca, S.[CodigoBarras], S.[IdPerfumeDerivado], S.[ImagenUrl],
           S.[Nombre], S.[Precio], S.[StockTienda], S.[StockVirtual], S.[Activo],
           COUNT(*) OVER() AS Total
    FROM [dbo].[ShowerGeles] S
    INNER JOIN [dbo].[Marcas] M ON M.[Id] = S.[IdMarca]
    WHERE S.[Activo] = 1
      AND (@IdMarca IS NULL OR S.[IdMarca] = @IdMarca)
    ORDER BY S.[Nombre]
    OFFSET (@Pagina - 1) * @Tamano ROWS
    FETCH NEXT @Tamano ROWS ONLY;
END
