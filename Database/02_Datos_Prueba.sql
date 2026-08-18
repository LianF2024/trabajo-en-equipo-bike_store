USE BikeStoreDB;
GO

INSERT INTO dbo.Categoria (Nombre, Descripcion, Activo)
VALUES
    (N'Montaña', N'Bicicletas diseñadas para terrenos irregulares y montaña.', 1),
    (N'Ruta', N'Bicicletas ligeras diseñadas para carretera.', 1),
    (N'BMX', N'Bicicletas para trucos, saltos y circuitos BMX.', 1),
    (N'Eléctricas', N'Bicicletas con asistencia mediante motor eléctrico.', 1),
    (N'Infantiles', N'Bicicletas diseñadas para niños.', 1);
GO

INSERT INTO dbo.Bicicleta (IdCategoria, Marca, Modelo, Precio, Stock, Estado)
VALUES
    ((SELECT IdCategoria FROM dbo.Categoria WHERE Nombre = N'Montaña'), N'Trek', N'Marlin 5', 1250.00, 8, N'Disponible'),
    ((SELECT IdCategoria FROM dbo.Categoria WHERE Nombre = N'Ruta'), N'Specialized', N'Allez Elite', 2450.00, 5, N'Bajo stock'),
    ((SELECT IdCategoria FROM dbo.Categoria WHERE Nombre = N'BMX'), N'GT', N'Performer 20', 850.00, 0, N'Agotado'),
    ((SELECT IdCategoria FROM dbo.Categoria WHERE Nombre = N'Eléctricas'), N'Scott', N'E-Strike 20', 4950.00, 3, N'Bajo stock'),
    ((SELECT IdCategoria FROM dbo.Categoria WHERE Nombre = N'Infantiles'), N'Giant', N'Animator 24', 620.00, 7, N'Disponible');
GO

INSERT INTO dbo.Cliente (Cedula, Nombres, Apellidos, Telefono, Correo)
VALUES
    (N'0100000001', N'Juan', N'Pérez', N'0981111111', N'juan.perez@email.com'),
    (N'0100000002', N'María', N'González', N'0982222222', N'maria.gonzalez@email.com'),
    (N'0100000003', N'Carlos', N'Ramírez', N'0983333333', N'carlos.ramirez@email.com'),
    (N'0100000004', N'Laura', N'Medina', N'0984444444', N'laura.medina@email.com'),
    (N'0100000005', N'Andrés', N'Vargas', N'0985555555', N'andres.vargas@email.com');
GO
