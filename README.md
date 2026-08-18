# BikeStore - Sistemas Cliente Servidor

Solución completa en .NET 10 adaptada a la nueva propuesta visual y al modelo mínimo de BikeStore. Incluye una aplicación ASP.NET Core MVC, una API RESTful, arquitectura por capas, Entity Framework Core y SQL Server.

## Arquitectura

```text
Navegador
   ↓
BikeStore.Web (MVC)
   ↓ HTTPS + JSON
BikeStore.Api (REST)
   ↓
BikeStore.Application
   ↓
BikeStore.Infrastructure / Entity Framework Core
   ↓
SQL Server - BikeStoreDB
```

La aplicación Web no referencia Entity Framework Core, el contexto de datos ni SQL Server. Toda operación se realiza mediante `IBikeStoreApiClient` y los endpoints REST.

## Proyectos

| Proyecto | Responsabilidad |
|---|---|
| `BikeStore.Domain` | Entidades y estados del negocio |
| `BikeStore.Application` | DTO, validaciones, contratos y servicios |
| `BikeStore.Infrastructure` | DbContext, mapeos SQL Server y repositorio |
| `BikeStore.Api` | Servicios REST, Swagger y manejo de errores |
| `BikeStore.Web` | Interfaz MVC basada en la nueva propuesta visual |
| `BikeStore.Application.Tests` | Pruebas de inventario, IVA y ventas |

## Modelo de datos adaptado

- `Categoria(IdCategoria, Nombre, Descripcion, Activo)`
- `Bicicleta(IdBicicleta, IdCategoria, Marca, Modelo, Precio, Stock, Estado)`
- `Cliente(IdCliente, Cedula, Nombres, Apellidos, Telefono, Correo)`
- `Venta(IdVenta, Fecha, IdCliente, Subtotal, IVA, Total)`
- `DetalleVenta(IdDetalle, IdVenta, IdBicicleta, Cantidad, Precio, Subtotal)`

## Requisitos

- Visual Studio 2022 con la carga de trabajo **Desarrollo de ASP.NET y web**.
- SDK de .NET 10 instalado y reconocido por el equipo.
- SQL Server y SQL Server Management Studio.
- Instancia configurada en este proyecto: `DESKTOP-TJ53I29\MSSQLSERVER01`.

Si Visual Studio 2022 no reconoce `net10.0`, confirme con `dotnet --version` que existe un SDK `10.0.x`. La solución también incluye `BikeStore.slnx` para versiones recientes del entorno.

## Preparar la base de datos

En SQL Server Management Studio, conectarse a:

```text
DESKTOP-TJ53I29\MSSQLSERVER01
```

Usar autenticación de Windows y ejecutar, en este orden:

1. `Database/01_Create_BikeStoreDB.sql`
2. `Database/02_Datos_Prueba.sql`
3. Opcional: `Database/03_Consultas_Verificacion.sql`

El primer script vuelve a crear las tablas, por lo que elimina la información anterior de BikeStoreDB.

La cadena utilizada por la API está en `src/BikeStore.Api/appsettings.json`:

```json
"BikeStore": "Server=DESKTOP-TJ53I29\\MSSQLSERVER01;Database=BikeStoreDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

Si el nombre de la instancia cambia, modifique únicamente ese valor.

## Ejecutar en Visual Studio

1. Abrir `BikeStore.sln`.
2. Esperar la restauración de paquetes NuGet.
3. Seleccionar **Compilar > Recompilar solución**.
4. En las propiedades de la solución, elegir **Varios proyectos de inicio**.
5. Configurar `BikeStore.Api` y `BikeStore.Web` con la acción **Iniciar**.
6. Ejecutar con `F5`.

Direcciones predeterminadas:

- API: `https://localhost:7101`
- Swagger: `https://localhost:7101/swagger`
- Web: `https://localhost:7201`

## Corrección del error Microsoft.Data.SqlClient.SNI.dll

La API contiene una referencia directa a `Microsoft.Data.SqlClient 6.1.6` para que NuGet copie la biblioteca nativa de conexión con SQL Server.

Después de abrir esta versión por primera vez:

1. Cerrar las aplicaciones que estén ejecutándose.
2. Seleccionar **Compilar > Limpiar solución**.
3. Hacer clic derecho sobre la solución y seleccionar **Restaurar paquetes NuGet**.
4. Seleccionar **Compilar > Recompilar solución**.
5. Iniciar nuevamente `BikeStore.Api` y `BikeStore.Web`.

Si Visual Studio conserva archivos de una compilación anterior, cierre Visual Studio, elimine únicamente las carpetas `bin` y `obj` de los proyectos y vuelva a abrir la solución.

## Funcionalidades

- Dashboard con cantidades reales, ventas del día, stock bajo y accesos rápidos.
- CRUD de categorías.
- CRUD de bicicletas mediante GET, POST, PUT y DELETE.
- Búsqueda de bicicletas por marca/modelo, categoría y marca.
- Consultas de stock bajo y productos agotados.
- CRUD y búsqueda de clientes por cédula y apellido.
- Venta de varias bicicletas en una sola transacción.
- Cálculo de subtotal, IVA y total dentro de la API.
- Actualización automática del inventario.
- Historial general, por fechas y por cliente.
- Validaciones, `ProblemDetails` y mensajes controlados de conexión.
- Precio compatible con coma o punto decimal en la Web.
- Los formularios de creación y edición leen explícitamente `CategoryId`, `Brand`, `Model`, `Price` y `Stock`, por lo que el precio no se pierde por diferencias de configuración regional.
- Los errores esperados, como categoría repetida, cédula duplicada, stock insuficiente o registros relacionados, se muestran en la Web y no interrumpen la ejecución en el código fuente.

## Documentación incluida

- `docs/API_REST.md`
- `docs/DIAGRAMAS.md`
- `docs/PRUEBAS_FUNCIONALES.md`
- `docs/COMPATIBILIDAD_DOTNET10.md`
- `docs/VERIFICACION.md`
