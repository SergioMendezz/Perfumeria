CREATE TABLE [dbo].[Marcas]
(
    [Id]          UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [Nombre]      VARCHAR(200)     NOT NULL,
    [PaisOrigen]  VARCHAR(100)     NULL,
    [Descripcion] TEXT             NULL,
    [LogoUrl]     VARCHAR(500)     NULL,
    [Activo]      BIT              DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Marcas] PRIMARY KEY CLUSTERED ([Id])
);
