IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [DangKyDichVu] (
    [Id] int NOT NULL IDENTITY,
    [MSSV] nvarchar(20) NOT NULL,
    [SinhVien] nvarchar(100) NOT NULL,
    [Phong] nvarchar(20) NOT NULL,
    [DichVu] nvarchar(100) NOT NULL,
    [Gia] decimal(18,0) NOT NULL,
    [NgayBatDau] datetime2 NOT NULL,
    [NgayKetThuc] datetime2 NOT NULL,
    [TrangThai] int NOT NULL,
    CONSTRAINT [PK_DangKyDichVu] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Departments] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NULL,
    CONSTRAINT [PK_Departments] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [DeviceConditions] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_DeviceConditions] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Maintenances] (
    [Id] int NOT NULL IDENTITY,
    [Room] nvarchar(20) NOT NULL,
    [Reporter] nvarchar(100) NOT NULL,
    [Category] nvarchar(50) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    [Level] int NOT NULL,
    [ReportDate] datetime2 NOT NULL,
    [Status] int NOT NULL,
    CONSTRAINT [PK_Maintenances] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Roles] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Rooms] (
    [Id] int NOT NULL IDENTITY,
    [RoomName] nvarchar(20) NOT NULL,
    [Building] nvarchar(50) NOT NULL,
    [Floor] nvarchar(50) NOT NULL,
    [RoomType] nvarchar(50) NOT NULL,
    [CurrentOccupancy] int NOT NULL,
    [MaxCapacity] int NOT NULL,
    [DangO] int NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_Rooms] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Violations] (
    [Id] int NOT NULL IDENTITY,
    [StudentId] nvarchar(20) NOT NULL,
    [StudentName] nvarchar(100) NOT NULL,
    [Room] nvarchar(20) NOT NULL,
    [ViolationType] nvarchar(100) NOT NULL,
    [ViolationDate] datetime2 NOT NULL,
    [Level] int NOT NULL,
    [Fine] decimal(18,0) NOT NULL,
    [Status] int NOT NULL,
    CONSTRAINT [PK_Violations] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Devices] (
    [Id] int NOT NULL IDENTITY,
    [DeviceName] nvarchar(255) NOT NULL,
    [Room] nvarchar(50) NOT NULL,
    [Type] nvarchar(100) NOT NULL,
    [Quantity] int NOT NULL,
    [ConditionId] int NOT NULL,
    [PurchaseDate] datetime2 NOT NULL,
    [Value] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_Devices] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Devices_DeviceConditions_ConditionId] FOREIGN KEY ([ConditionId]) REFERENCES [DeviceConditions] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Staffs] (
    [Id] int NOT NULL IDENTITY,
    [StaffCode] nvarchar(max) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [Phone] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [HireDate] datetime2 NOT NULL,
    [Status] int NOT NULL,
    [DepartmentId] int NOT NULL,
    [RoleId] int NOT NULL,
    CONSTRAINT [PK_Staffs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Staffs_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Staffs_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Students] (
    [Id] int NOT NULL IDENTITY,
    [StudentCode] nvarchar(20) NOT NULL,
    [FullName] nvarchar(100) NOT NULL,
    [Email] nvarchar(100) NULL,
    [Gender] nvarchar(10) NOT NULL,
    [PhoneNumber] nvarchar(15) NOT NULL,
    [Major] nvarchar(100) NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [RoomId] int NULL,
    CONSTRAINT [PK_Students] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Students_Rooms_RoomId] FOREIGN KEY ([RoomId]) REFERENCES [Rooms] ([Id])
);
GO

CREATE TABLE [Contracts] (
    [Id] int NOT NULL IDENTITY,
    [ContractCode] nvarchar(max) NOT NULL,
    [StudentId] int NOT NULL,
    [RoomId] int NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [MonthlyFee] decimal(18,2) NOT NULL,
    [Deposit] decimal(18,2) NOT NULL,
    [Status] int NOT NULL,
    CONSTRAINT [PK_Contracts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Contracts_Rooms_RoomId] FOREIGN KEY ([RoomId]) REFERENCES [Rooms] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Contracts_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Invoices] (
    [Id] int NOT NULL IDENTITY,
    [StudentId] int NOT NULL,
    [RoomId] int NOT NULL,
    [Month] int NOT NULL,
    [Year] int NOT NULL,
    [RoomFee] decimal(18,2) NOT NULL,
    [UtilityFee] decimal(18,2) NOT NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    [DueDate] datetime2 NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_Invoices] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Invoices_Rooms_RoomId] FOREIGN KEY ([RoomId]) REFERENCES [Rooms] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Invoices_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [RoomTransactions] (
    [Id] int NOT NULL IDENTITY,
    [StudentId] int NOT NULL,
    [RoomId] int NOT NULL,
    [TransactionType] nvarchar(20) NOT NULL,
    [TransactionDate] datetime2 NOT NULL,
    [Note] nvarchar(255) NULL,
    [HandledBy] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_RoomTransactions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RoomTransactions_Rooms_RoomId] FOREIGN KEY ([RoomId]) REFERENCES [Rooms] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RoomTransactions_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_Contracts_RoomId] ON [Contracts] ([RoomId]);
GO

CREATE INDEX [IX_Contracts_StudentId] ON [Contracts] ([StudentId]);
GO

CREATE INDEX [IX_Devices_ConditionId] ON [Devices] ([ConditionId]);
GO

CREATE INDEX [IX_Invoices_RoomId] ON [Invoices] ([RoomId]);
GO

CREATE INDEX [IX_Invoices_StudentId] ON [Invoices] ([StudentId]);
GO

CREATE INDEX [IX_RoomTransactions_RoomId] ON [RoomTransactions] ([RoomId]);
GO

CREATE INDEX [IX_RoomTransactions_StudentId] ON [RoomTransactions] ([StudentId]);
GO

CREATE INDEX [IX_Staffs_DepartmentId] ON [Staffs] ([DepartmentId]);
GO

CREATE INDEX [IX_Staffs_RoleId] ON [Staffs] ([RoleId]);
GO

CREATE INDEX [IX_Students_RoomId] ON [Students] ([RoomId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260410033200_InitialCreate', N'8.0.13');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [DangKyDichVu] DROP CONSTRAINT [PK_DangKyDichVu];
GO

EXEC sp_rename N'[DangKyDichVu]', N'ServiceRegistrations';
GO

EXEC sp_rename N'[ServiceRegistrations].[TrangThai]', N'Status', N'COLUMN';
GO

EXEC sp_rename N'[ServiceRegistrations].[SinhVien]', N'StudentName', N'COLUMN';
GO

EXEC sp_rename N'[ServiceRegistrations].[Phong]', N'StudentId', N'COLUMN';
GO

EXEC sp_rename N'[ServiceRegistrations].[NgayKetThuc]', N'StartDate', N'COLUMN';
GO

EXEC sp_rename N'[ServiceRegistrations].[NgayBatDau]', N'EndDate', N'COLUMN';
GO

EXEC sp_rename N'[ServiceRegistrations].[MSSV]', N'Room', N'COLUMN';
GO

EXEC sp_rename N'[ServiceRegistrations].[Gia]', N'Price', N'COLUMN';
GO

EXEC sp_rename N'[ServiceRegistrations].[DichVu]', N'ServiceName', N'COLUMN';
GO

ALTER TABLE [ServiceRegistrations] ADD CONSTRAINT [PK_ServiceRegistrations] PRIMARY KEY ([Id]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260410033535_Init', N'8.0.13');
GO

COMMIT;
GO

