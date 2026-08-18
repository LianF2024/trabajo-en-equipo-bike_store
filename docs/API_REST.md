# Especificación de la API REST

Base local: `https://localhost:7101`. Swagger: `https://localhost:7101/swagger`.

| Método | Endpoint | Descripción |
|---|---|---|
| GET | `/api/categorias` | Listar y buscar categorías |
| GET | `/api/categorias/{id}` | Obtener una categoría |
| POST | `/api/categorias` | Registrar una categoría |
| PUT | `/api/categorias/{id}` | Actualizar una categoría |
| DELETE | `/api/categorias/{id}` | Desactivar una categoría |
| GET | `/api/bicicletas` | Listar y filtrar bicicletas |
| GET | `/api/bicicletas/{id}` | Obtener una bicicleta |
| GET | `/api/bicicletas/stock-bajo?limite=5` | Consultar stock entre 1 y el límite |
| GET | `/api/bicicletas/agotadas` | Consultar stock igual a cero |
| POST | `/api/bicicletas` | Registrar una bicicleta |
| PUT | `/api/bicicletas/{id}` | Actualizar una bicicleta |
| DELETE | `/api/bicicletas/{id}` | Eliminar una bicicleta sin ventas relacionadas |
| GET | `/api/clientes` | Listar o buscar clientes |
| GET | `/api/clientes/{id}` | Obtener un cliente |
| POST | `/api/clientes` | Registrar un cliente |
| PUT | `/api/clientes/{id}` | Actualizar un cliente |
| DELETE | `/api/clientes/{id}` | Eliminar un cliente sin ventas relacionadas |
| GET | `/api/ventas` | Consultar historial y aplicar filtros |
| GET | `/api/ventas/cliente/{clienteId}` | Consultar ventas de un cliente |
| GET | `/api/ventas/{id}` | Obtener una venta y sus detalles |
| POST | `/api/ventas` | Registrar una venta múltiple |

## POST de bicicleta

```json
{
  "categoryId": 1,
  "brand": "Trek",
  "model": "Marlin 7",
  "price": 1125.50,
  "stock": 8
}
```

Respuesta: `201 Created`.

## PUT de bicicleta

```json
{
  "categoryId": 1,
  "brand": "Trek",
  "model": "Marlin 7",
  "price": 1199.00,
  "stock": 10
}
```

Respuesta: `204 No Content`.

## POST de venta

```json
{
  "customerId": 1,
  "items": [
    { "bicycleId": 1, "quantity": 1 },
    { "bicycleId": 5, "quantity": 2 }
  ]
}
```

La API consulta precios y stock en SQL Server, calcula subtotal, IVA y total, guarda `Venta` y `DetalleVenta`, y descuenta inventario dentro de la misma transacción.

## Errores

Las respuestas de error utilizan `ProblemDetails` con `status`, `title`, `detail`, `instance` y `traceId`. Un registro relacionado con una venta no se elimina y devuelve `409 Conflict`.
