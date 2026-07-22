CREATE PROCEDURE [dbo].[Desodorante_Eliminar]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    UPDATE [dbo].[Desodorantes]
    SET [Activo] = 0
    WHERE [Id] = @Id;

    COMMIT TRANSACTION;

    SELECT @Id AS Id;
END
