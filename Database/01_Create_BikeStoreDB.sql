USE master;
GO

IF DB_ID(N'BikeStoreDB') IS NULL
BEGIN
    CREATE DATABASE BikeStoreDB;
END;
GO

USE BikeStoreDB;
GO

IF OBJECT_ID(N'dbo.DetalleVenta', N'U') IS NOT NULL DROP TABLE dbo.DetalleVenta;
IF OBJECT_ID(N'dbo.Venta', N'U') IS NOT NULL DROP TABLE dbo.Venta;
IF OBJECT_ID(N'dbo.Bicicleta', N'U') IS NOT NULL DROP TABLE dbo.Bicicleta;
IF OBJECT_ID(N'dbo.Cliente', N'U') IS NOT NULL DROP TABLE dbo.Cliente;
IF OBJECT_ID(N'dbo.Categoria', N'U') IS NOT NULL DROP TABLE dbo.Categoria;
IF OBJECT_ID(N'dbo.DetalleVentas', N'U') IS NOT NULL DROP TABLE dbo.DetalleVentas;
IF OBJECT_ID(N'dbo.Ventas', N'U') IS NOT NULL DROP TABLE dbo.Ventas;
IF OBJECT_ID(N'dbo.Bicicletas', N'U') IS NOT NULL DROP TABLE dbo.Bicicletas;
IF OBJECT_ID(N'dbo.Clientes', N'U') IS NOT NULL DROP TABLE dbo.Clientes;
IF OBJECT_ID(N'dbo.Categorias', N'U') IS NOT NULL DROP TABLE dbo.Categorias;
GO

CREATE TABLE dbo.Categoria
(
    IdCategoria INT IDENTITY(1,1) NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(250) NULL,
    Activo BIT NOT NULL CONSTRAINT DF_Categoria_Activo DEFAULT (1),
    CONSTRAINT PK_Categoria PRIMARY KEY (IdCategoria),
    CONSTRAINT UQ_Categoria_Nombre UNIQUE (Nombre),
    CONSTRAINT CK_Categoria_Nombre CHECK (LEN(LTRIM(RTRIM(Nombre))) >= 2)
);
GO

CREATE TABLE dbo.Bicicleta
(
    IdBicicleta INT IDENTITY(1,1) NOT NULL,
    IdCategoria INT NOT NULL,
    Marca NVARCHAR(100) NOT NULL,
    Modelo NVARCHAR(100) NOT NULL,
    Precio DECIMAL(10,2) NOT NULL,
    Stock INT NOT NULL CONSTRAINT DF_Bicicleta_Stock DEFAULT (0),
    Estado NVARCHAR(20) NOT NULL CONSTRAINT DF_Bicicleta_Estado DEFAULT (N'Disponible'),
    CONSTRAINT PK_Bicicleta PRIMARY KEY (IdBicicleta),
    CONSTRAINT FK_Bicicleta_Categoria FOREIGN KEY (IdCategoria)
        REFERENCES dbo.Categoria (IdCategoria),
    CONSTRAINT CK_Bicicleta_Precio CHECK (Precio > 0),
    CONSTRAINT CK_Bicicleta_Stock CHECK (Stock >= 0),
    CONSTRAINT CK_Bicicleta_Estado CHECK
        (Estado IN (N'Disponible', N'Bajo stock', N'Agotado', N'Inactivo'))
);
GO

CREATE TABLE dbo.Cliente
(
    IdCliente INT IDENTITY(1,1) NOT NULL,
    Cedula NVARCHAR(20) NOT NULL,
    Nombres NVARCHAR(100) NOT NULL,
    Apellidos NVARCHAR(100) NOT NULL,
    Telefono NVARCHAR(20) NULL,
    Correo NVARCHAR(150) NULL,
    CONSTRAINT PK_Cliente PRIMARY KEY (IdCliente),
    CONSTRAINT UQ_Cliente_Cedula UNIQUE (Cedula),
    CONSTRAINT CK_Cliente_Cedula CHECK
        (Cedula NOT LIKE '%[^0-9]%' AND LEN(Cedula) BETWEEN 10 AND 13)
);
GO

CREATE TABLE dbo.Venta
(
    IdVenta INT IDENTITY(1,1) NOT NULL,
    Fecha DATETIME2 NOT NULL CONSTRAINT DF_Venta_Fecha DEFAULT (GETDATE()),
    IdCliente INT NOT NULL,
    Subtotal DECIMAL(12,2) NOT NULL CONSTRAINT DF_Venta_Subtotal DEFAULT (0),
    IVA DECIMAL(12,2) NOT NULL CONSTRAINT DF_Venta_IVA DEFAULT (0),
    Total DECIMAL(12,2) NOT NULL CONSTRAINT DF_Venta_Total DEFAULT (0),
    CONSTRAINT PK_Venta PRIMARY KEY (IdVenta),
    CONSTRAINT FK_Venta_Cliente FOREIGN KEY (IdCliente)
        REFERENCES dbo.Cliente (IdCliente),
    CONSTRAINT CK_Venta_Subtotal CHECK (Subtotal >= 0),
    CONSTRAINT CK_Venta_IVA CHECK (IVA >= 0),
    CONSTRAINT CK_Venta_Total CHECK (Total >= 0)
);
GO

CREATE TABLE dbo.DetalleVenta
(
    IdDetalle INT IDENTITY(1,1) NOT NULL,
    IdVenta INT NOT NULL,
    IdBicicleta INT NOT NULL,
    Cantidad INT NOT NULL,
    Precio DECIMAL(10,2) NOT NULL,
    Subtotal DECIMAL(12,2) NOT NULL,
    CONSTRAINT PK_DetalleVenta PRIMARY KEY (IdDetalle),
    CONSTRAINT FK_DetalleVenta_Venta FOREIGN KEY (IdVenta)
        REFERENCES dbo.Venta (IdVenta),
    CONSTRAINT FK_DetalleVenta_Bicicleta FOREIGN KEY (IdBicicleta)
        REFERENCES dbo.Bicicleta (IdBicicleta),
    CONSTRAINT CK_DetalleVenta_Cantidad CHECK (Cantidad > 0),
    CONSTRAINT CK_DetalleVenta_Precio CHECK (Precio > 0),
    CONSTRAINT CK_DetalleVenta_Subtotal CHECK (Subtotal > 0)
);
GO

CREATE INDEX IX_Bicicleta_Marca_Modelo ON dbo.Bicicleta (Marca, Modelo);
CREATE INDEX IX_Bicicleta_Stock ON dbo.Bicicleta (Stock);
CREATE INDEX IX_Cliente_Apellidos ON dbo.Cliente (Apellidos);
CREATE INDEX IX_Venta_Fecha ON dbo.Venta (Fecha DESC);
GO
