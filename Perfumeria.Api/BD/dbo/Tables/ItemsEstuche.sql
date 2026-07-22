CREATE TABLE [dbo].[ItemsEstuche]
(
    [Id]           UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [IdEstuche]    UNIQUEIDENTIFIER NOT NULL,
    [TipoProducto] VARCHAR(30)      NOT NULL,
    [IdProducto]   UNIQUEIDENTIFIER NOT NULL,
    [IdVariante]   UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_ItemsEstuche] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [FK_ItemsEstuche_Estuches] FOREIGN KEY ([IdEstuche]) REFERENCES [dbo].[Estuches] ([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_ItemsEstuche_IdEstuche] ON [dbo].[ItemsEstuche] ([IdEstuche]);
