CREATE TABLE [dbo].[ItemsPedido]
(
    [Id]             UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [IdPedido]       UNIQUEIDENTIFIER NOT NULL,
    [TipoProducto]   VARCHAR(30)      NOT NULL,
    [IdProducto]     UNIQUEIDENTIFIER NOT NULL,
    [IdVariante]     UNIQUEIDENTIFIER NULL,
    [Cantidad]       INT              NOT NULL,
    [PrecioUnitario] DECIMAL(10,2)    NOT NULL,
    CONSTRAINT [PK_ItemsPedido] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [FK_ItemsPedido_Pedidos] FOREIGN KEY ([IdPedido]) REFERENCES [dbo].[Pedidos] ([Id]),
    CONSTRAINT [CHK_ItemsPedido_Cantidad] CHECK ([Cantidad] > 0),
    CONSTRAINT [CHK_ItemsPedido_PrecioUnitario] CHECK ([PrecioUnitario] >= 0)
);
GO

CREATE NONCLUSTERED INDEX [IX_ItemsPedido_IdPedido] ON [dbo].[ItemsPedido] ([IdPedido]);
