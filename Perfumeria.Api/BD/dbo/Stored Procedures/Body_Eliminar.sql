CREATE PROCEDURE [dbo].[Body_Eliminar]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    UPDATE [dbo].[Bodys]
    SET [Activo] = 0
    WHERE [Id] = @Id;

    COMMIT TRANSACTION;

    SELECT @Id AS Id;
END
