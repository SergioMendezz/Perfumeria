CREATE TABLE [dbo].[BodySprays]
(
    [Id]                UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [IdMarca]           UNIQUEIDENTIFIER NOT NULL,
    [CodigoBarras]      VARCHAR(50)      NOT NULL,
    [IdPerfumeDerivado] UNIQUEIDENTIFIER NULL,
    [ImagenUrl]         VARCHAR(500)     NOT NULL,
    [Nombre]            VARCHAR(200)     NOT NULL,
    [Precio]            DECIMAL(10,2)    NOT NULL,
    [StockTienda]       INT              DEFAULT ((0)) NOT NULL,
    [StockVirtual]      INT              DEFAULT ((0)) NOT NULL,
    [Activo]            BIT              DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_BodySprays] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [FK_BodySprays_Marcas] FOREIGN KEY ([IdMarca]) REFERENCES [dbo].[Marcas] ([Id]),
    CONSTRAINT [FK_BodySprays_Perfumes] FOREIGN KEY ([IdPerfumeDerivado]) REFERENCES [dbo].[Perfumes] ([Id]),
    CONSTRAINT [CHK_BodySprays_Precio] CHECK ([Precio] >= 0),
    CONSTRAINT [CHK_BodySprays_StockTienda] CHECK ([StockTienda] >= 0),
    CONSTRAINT [CHK_BodySprays_StockVirtual] CHECK ([StockVirtual] >= 0)
);
GO

CREATE NONCLUSTERED INDEX [IX_BodySprays_CodigoBarras] ON [dbo].[BodySprays] ([CodigoBarras]);
GO

CREATE NONCLUSTERED INDEX [IX_BodySprays_IdMarca] ON [dbo].[BodySprays] ([IdMarca]);
