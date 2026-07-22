CREATE PROCEDURE [dbo].[Token_Revocar]
    @Token       VARCHAR(1000),
    @IdUsuario   UNIQUEIDENTIFIER,
    @FechaExpira DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Id UNIQUEIDENTIFIER = NEWID();

    BEGIN TRANSACTION;

    INSERT INTO [dbo].[TokensRevocados] ([Id], [Token], [IdUsuario], [FechaExpira])
    VALUES (@Id, @Token, @IdUsuario, @FechaExpira);

    COMMIT TRANSACTION;

    SELECT @Id AS Id;
END
