CREATE PROCEDURE [dbo].[Usuario_Editar]
    @Id            UNIQUEIDENTIFIER,
    @NombreUsuario VARCHAR(100),
    @Email         VARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    UPDATE [dbo].[Usuarios]
    SET [NombreUsuario] = @NombreUsuario,
        [Email] = @Email
    WHERE [Id] = @Id;

    COMMIT TRANSACTION;

    SELECT @Id AS Id;
END
