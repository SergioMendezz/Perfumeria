CREATE TABLE [dbo].[Perfumes]
(
    [Id]           UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [IdMarca]      UNIQUEIDENTIFIER NOT NULL,
    [Nombre]       VARCHAR(200)     NOT NULL,
    [CodigoBarras] VARCHAR(50)      NOT NULL,
    [Genero]       VARCHAR(20)      NOT NULL,
    [Categoria]    VARCHAR(50)      NOT NULL,
    [ImagenUrl]    VARCHAR(500)     NOT NULL,
    [Descripcion]  TEXT             NULL,
    [Activo]       BIT              DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Perfumes] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [FK_Perfumes_Marcas] FOREIGN KEY ([IdMarca]) REFERENCES [dbo].[Marcas] ([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_Perfumes_CodigoBarras] ON [dbo].[Perfumes] ([CodigoBarras]);
GO

CREATE NONCLUSTERED INDEX [IX_Perfumes_IdMarca] ON [dbo].[Perfumes] ([IdMarca]);
