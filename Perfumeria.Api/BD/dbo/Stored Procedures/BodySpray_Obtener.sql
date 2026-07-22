CREATE PROCEDURE [dbo].[BodySpray_Obtener]
    @Pagina  INT,
    @Tamano  INT,
    @IdMarca UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT B.[Id], M.[Nombre] AS Marca, B.[CodigoBarras], B.[IdPerfumeDerivado], B.[ImagenUrl],
           B.[Nombre], B.[Precio], B.[StockTienda], B.[StockVirtual], B.[Activo],
           COUNT(*) OVER() AS Total
    FROM [dbo].[BodySprays] B
    INNER JOIN [dbo].[Marcas] M ON M.[Id] = B.[IdMarca]
    WHERE B.[Activo] = 1
      AND (@IdMarca IS NULL OR B.[IdMarca] = @IdMarca)
    ORDER BY B.[Nombre]
    OFFSET (@Pagina - 1) * @Tamano ROWS
    FETCH NEXT @Tamano ROWS ONLY;
END
