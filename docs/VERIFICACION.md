# Verificación de la solución adaptada

Controles realizados:

- Separación Web MVC, API REST, Application, Infrastructure y Domain.
- La Web no referencia Entity Framework Core ni SQL Server.
- Modelo EF Core alineado con `Categoria`, `Bicicleta`, `Cliente`, `Venta` y `DetalleVenta`.
- Contratos JSON alineados con las columnas de la nueva propuesta.
- CRUD REST de categorías, bicicletas y clientes.
- Registro transaccional e historial de ventas.
- Cálculo de IVA y actualización automática de stock.
- Diseño lateral azul y dashboard integrados en todos los módulos.
- Manejo flexible de precios con coma o punto decimal.
- Lectura explícita de los formularios Web para evitar que el enlace cultural convierta el precio a cero al crear o editar.
- Serialización explícita del tipo real de cada solicitud POST/PUT enviada a la API.
- Validaciones equivalentes en la Web y en los servicios de Application.
- Manejo global y controlado de errores de comunicación Web-API.
- Captura de errores de negocio dentro de los controladores de la API: duplicados, registros inexistentes, relaciones y stock insuficiente se convierten en respuestas `400`, `404` o `409` y se muestran en la interfaz.
- Pruebas automatizadas para crear, editar y eliminar/desactivar categorías, bicicletas y clientes.
- Cadena configurada para `DESKTOP-TJ53I29\MSSQLSERVER01`.
- Archivos JSON y XML revisados estructuralmente.
- ZIP verificado sin errores de compresión.

La compilación y la conexión real deben comprobarse en el equipo que dispone de Visual Studio, .NET 10 y SQL Server.
