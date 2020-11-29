IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE TABLE [Payment] (
    [Id] bigint NOT NULL IDENTITY,
    [CreateDate] datetime2 NOT NULL,
    [UpdateDate] datetime2 NULL,
    [CardName] nvarchar(200) NULL,
    [CardNumber] varchar(200) NULL,
    [Expiration] varchar(200) NULL,
    [CVV] varchar(200) NULL,
    [PaymentMethod] int NOT NULL,
    CONSTRAINT [PK_Payment] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Order] (
    [Id] bigint NOT NULL IDENTITY,
    [CreateDate] datetime2 NOT NULL,
    [UpdateDate] datetime2 NULL,
    [CustomerId] nvarchar(200) NULL,
    [TotalAmount] decimal(5, 2) NOT NULL,
    [Status] int NOT NULL,
    [PaymentId] bigint NULL,
    CONSTRAINT [PK_Order] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Order_Payment_PaymentId] FOREIGN KEY ([PaymentId]) REFERENCES [Payment] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [OrderLine] (
    [Id] bigint NOT NULL IDENTITY,
    [CreateDate] datetime2 NOT NULL,
    [UpdateDate] datetime2 NULL,
    [OrderId] bigint NOT NULL,
    [ProductId] nvarchar(200) NULL,
    [Name] nvarchar(200) NULL,
    [Quantity] int NOT NULL,
    [UnitPrice] decimal(5, 2) NOT NULL,
    CONSTRAINT [PK_OrderLine] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrderLine_Order_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Order] ([Id]) ON DELETE CASCADE
);

GO

CREATE INDEX [IX_Order_PaymentId] ON [Order] ([PaymentId]);

GO

CREATE INDEX [IX_OrderLine_OrderId] ON [OrderLine] ([OrderId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201022085647_InitDatabase', N'3.1.9');

GO

