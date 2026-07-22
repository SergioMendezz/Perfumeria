CREATE TABLE [dbo].[Usuarios]
(
    [Id]             UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [NombreUsuario]  VARCHAR(100)     NOT NULL,
    [Email]          VARCHAR(200)     NOT NULL,
    [PasswordHash]   VARCHAR(500)     NOT NULL,
    [PasswordSalt]   VARCHAR(500)     NOT NULL,
    [Rol]            VARCHAR(20)      NOT NULL,
    [Activo]         BIT              DEFAULT ((1)) NOT NULL,
    [FechaCreacion]  DATETIME         DEFAULT (getdate()) NOT NULL,
    [UltimoAcceso]   DATETIME         NULL,
    CONSTRAINT [PK_Usuarios] PRIMARY KEY CLUSTERED ([Id])
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Usuarios_Email] ON [dbo].[Usuarios] ([Email]);
