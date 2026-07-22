CREATE PROCEDURE [dbo].[Token_VerificarRevocado]
    @Token VARCHAR(1000)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CASE WHEN EXISTS (
        SELECT 1 FROM [dbo].[TokensRevocados] WHERE [Token] = @Token
    ) THEN 1 ELSE 0 END AS EstaRevocado;
END
