CREATE TABLE [dbo].[ShowerGeles]
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
    CONSTRAINT [PK_ShowerGeles] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [FK_ShowerGeles_Marcas] FOREIGN KEY ([IdMarca]) REFERENCES [dbo].[Marcas] ([Id]),
    CONSTRAINT [FK_ShowerGeles_Perfumes] FOREIGN KEY ([IdPerfumeDerivado]) REFERENCES [dbo].[Perfumes] ([Id]),
    CONSTRAINT [CHK_ShowerGeles_Precio] CHECK ([Precio] >= 0),
    CONSTRAINT [CHK_ShowerGeles_StockTienda] CHECK ([StockTienda] >= 0),
    CONSTRAINT [CHK_ShowerGeles_StockVirtual] CHECK ([StockVirtual] >= 0)
);
GO

CREATE NONCLUSTERED INDEX [IX_ShowerGeles_CodigoBarras] ON [dbo].[ShowerGeles] ([CodigoBarras]);
GO

CREATE NONCLUSTERED INDEX [IX_ShowerGeles_IdMarca] ON [dbo].[ShowerGeles] ([IdMarca]);
