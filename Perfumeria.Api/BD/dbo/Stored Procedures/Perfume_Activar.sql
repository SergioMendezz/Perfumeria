CREATE PROCEDURE [dbo].[Perfume_Activar]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    UPDATE [dbo].[Perfumes]
    SET [Activo] = 1
    WHERE [Id] = @Id;

    COMMIT TRANSACTION;

    SELECT @Id AS Id;
END
