CREATE TABLE [dbo].[VariantesPerfume]
(
    [Id]            UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [IdPerfume]     UNIQUEIDENTIFIER NOT NULL,
    [Mililitros]    DECIMAL(10,2)    NOT NULL,
    [Precio]        DECIMAL(10,2)    NOT NULL,
    [StockTienda]   INT              DEFAULT ((0)) NOT NULL,
    [StockVirtual]  INT              DEFAULT ((0)) NOT NULL,
    [Activo]        BIT              DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_VariantesPerfume] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [FK_VariantesPerfume_Perfumes] FOREIGN KEY ([IdPerfume]) REFERENCES [dbo].[Perfumes] ([Id]),
    CONSTRAINT [CHK_VariantesPerfume_Mililitros] CHECK ([Mililitros] > 0),
    CONSTRAINT [CHK_VariantesPerfume_Precio] CHECK ([Precio] >= 0),
    CONSTRAINT [CHK_VariantesPerfume_StockTienda] CHECK ([StockTienda] >= 0),
    CONSTRAINT [CHK_VariantesPerfume_StockVirtual] CHECK ([StockVirtual] >= 0)
);
GO

CREATE NONCLUSTERED INDEX [IX_VariantesPerfume_IdPerfume] ON [dbo].[VariantesPerfume] ([IdPerfume]);
