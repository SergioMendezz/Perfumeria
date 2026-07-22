CREATE TABLE [dbo].[Estuches]
(
    [Id]           UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [Nombre]       VARCHAR(200)     NOT NULL,
    [ImagenUrl]    VARCHAR(500)     NOT NULL,
    [Precio]       DECIMAL(10,2)    NOT NULL,
    [StockTienda]  INT              DEFAULT ((0)) NOT NULL,
    [StockVirtual] INT              DEFAULT ((0)) NOT NULL,
    [Activo]       BIT              DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Estuches] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [CHK_Estuches_Precio] CHECK ([Precio] >= 0),
    CONSTRAINT [CHK_Estuches_StockTienda] CHECK ([StockTienda] >= 0),
    CONSTRAINT [CHK_Estuches_StockVirtual] CHECK ([StockVirtual] >= 0)
);
