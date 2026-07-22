CREATE TABLE [dbo].[Pedidos]
(
    [Id]                    UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [IdCliente]             UNIQUEIDENTIFIER NOT NULL,
    [FechaPedido]           DATETIME         DEFAULT (getdate()) NOT NULL,
    [Estado]                VARCHAR(20)      DEFAULT ('Pendiente') NOT NULL,
    [Total]                 DECIMAL(10,2)    NOT NULL,
    [MetodoPago]            VARCHAR(50)      NOT NULL,
    [IdTransaccionPasarela] VARCHAR(200)     NULL,
    CONSTRAINT [PK_Pedidos] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [FK_Pedidos_Usuarios] FOREIGN KEY ([IdCliente]) REFERENCES [dbo].[Usuarios] ([Id]),
    CONSTRAINT [CHK_Pedidos_Total] CHECK ([Total] >= 0)
);
GO

CREATE NONCLUSTERED INDEX [IX_Pedidos_IdCliente] ON [dbo].[Pedidos] ([IdCliente]);
GO

CREATE NONCLUSTERED INDEX [IX_Pedidos_IdTransaccionPasarela] ON [dbo].[Pedidos] ([IdTransaccionPasarela]);
