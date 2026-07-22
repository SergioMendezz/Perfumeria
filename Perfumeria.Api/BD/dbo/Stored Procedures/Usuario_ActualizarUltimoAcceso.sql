CREATE PROCEDURE [dbo].[Usuario_ActualizarUltimoAcceso]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    UPDATE [dbo].[Usuarios]
    SET [UltimoAcceso] = GETDATE()
    WHERE [Id] = @Id;

    COMMIT TRANSACTION;

    SELECT @Id AS Id;
END
