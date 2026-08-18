USE BikeStoreDB;
GO

SELECT c.IdCategoria, c.Nombre, c.Descripcion, c.Activo
FROM dbo.Categoria AS c
ORDER BY c.Nombre;

SELECT b.IdBicicleta, c.Nombre AS Categoria, b.Marca, b.Modelo,
       b.Precio, b.Stock, b.Estado
FROM dbo.Bicicleta AS b
INNER JOIN dbo.Categoria AS c ON c.IdCategoria = b.IdCategoria
ORDER BY b.Marca, b.Modelo;

SELECT b.IdBicicleta, b.Marca, b.Modelo, b.Stock, b.Estado
FROM dbo.Bicicleta AS b
WHERE b.Stock BETWEEN 1 AND 5
ORDER BY b.Stock;

SELECT b.IdBicicleta, b.Marca, b.Modelo, b.Stock, b.Estado
FROM dbo.Bicicleta AS b
WHERE b.Stock = 0;

SELECT c.IdCliente, c.Cedula, c.Nombres, c.Apellidos, c.Telefono, c.Correo
FROM dbo.Cliente AS c
ORDER BY c.Apellidos, c.Nombres;

SELECT v.IdVenta, v.Fecha, c.Nombres + N' ' + c.Apellidos AS Cliente,
       v.Subtotal, v.IVA, v.Total
FROM dbo.Venta AS v
INNER JOIN dbo.Cliente AS c ON c.IdCliente = v.IdCliente
ORDER BY v.Fecha DESC;

SELECT d.IdDetalle, d.IdVenta, b.Marca + N' ' + b.Modelo AS Bicicleta,
       d.Cantidad, d.Precio, d.Subtotal
FROM dbo.DetalleVenta AS d
INNER JOIN dbo.Bicicleta AS b ON b.IdBicicleta = d.IdBicicleta
ORDER BY d.IdVenta DESC, d.IdDetalle;
GO
