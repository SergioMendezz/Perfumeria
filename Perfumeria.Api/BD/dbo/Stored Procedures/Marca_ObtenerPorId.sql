CREATE PROCEDURE [dbo].[Marca_ObtenerPorId]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [Nombre], [PaisOrigen], [Descripcion], [LogoUrl], [Activo]
    FROM [dbo].[Marcas]
    WHERE [Id] = @Id;
END
