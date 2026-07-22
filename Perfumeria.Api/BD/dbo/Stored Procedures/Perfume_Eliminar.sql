CREATE PROCEDURE [dbo].[Perfume_Eliminar]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    UPDATE [dbo].[Perfumes]
    SET [Activo] = 0
    WHERE [Id] = @Id;

    COMMIT TRANSACTION;

    SELECT @Id AS Id;
END
