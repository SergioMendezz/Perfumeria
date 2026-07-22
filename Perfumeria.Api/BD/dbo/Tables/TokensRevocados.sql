CREATE TABLE [dbo].[TokensRevocados]
(
    [Id]           UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [Token]        VARCHAR(1000)    NOT NULL,
    [IdUsuario]    UNIQUEIDENTIFIER NOT NULL,
    [FechaExpira]  DATETIME         NOT NULL,
    CONSTRAINT [PK_TokensRevocados] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [FK_TokensRevocados_Usuarios] FOREIGN KEY ([IdUsuario]) REFERENCES [dbo].[Usuarios] ([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_TokensRevocados_Token] ON [dbo].[TokensRevocados] ([Token]);
