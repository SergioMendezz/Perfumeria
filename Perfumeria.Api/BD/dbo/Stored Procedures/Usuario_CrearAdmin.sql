CREATE PROCEDURE [dbo].[Usuario_CrearAdmin]
    @NombreUsuario VARCHAR(100),
    @Email         VARCHAR(200),
    @PasswordHash  VARCHAR(500),
    @PasswordSalt  VARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Id UNIQUEIDENTIFIER = NEWID();

    BEGIN TRANSACTION;

    INSERT INTO [dbo].[Usuarios] ([Id], [NombreUsuario], [Email], [PasswordHash], [PasswordSalt], [Rol])
    VALUES (@Id, @NombreUsuario, @Email, @PasswordHash, @PasswordSalt, 'Admin');

    COMMIT TRANSACTION;

    SELECT @Id AS Id;
END
