# Pruebas funcionales

| ID | Escenario | Resultado esperado |
|---|---|---|
| PF-01 | GET `/api/bicicletas` | 200 y listado completo |
| PF-02 | Crear y listar una categoría | POST 201 y categoría visible |
| PF-03 | Editar una categoría | PUT 204 y datos actualizados |
| PF-04 | Desactivar una categoría | DELETE 204 y estado inactivo |
| PF-05 | Crear bicicleta con precio `1250,50` | POST 201 y bicicleta visible |
| PF-06 | Editar precio con coma y stock | PUT 204 y valores actualizados, sin convertir el precio a cero |
| PF-07 | Editar precio con punto (`999.99`) | PUT 204 y valor actualizado |
| PF-08 | Filtrar por categoría y marca | Solo coincidencias |
| PF-09 | Consultar stock bajo | Solo existencias entre 1 y 5 |
| PF-10 | Consultar agotadas | Solo existencias en cero |
| PF-11 | Eliminar bicicleta sin ventas | DELETE 204 y registro eliminado |
| PF-12 | Eliminar bicicleta vendida | 409 y mensaje de relación existente |
| PF-13 | Crear un cliente | POST 201 y cliente visible |
| PF-14 | Editar un cliente | PUT 204 y datos actualizados |
| PF-15 | Eliminar cliente sin ventas | DELETE 204 y registro eliminado |
| PF-16 | Eliminar cliente con ventas | 409 y mensaje de relación existente |
| PF-17 | Venta con varios productos | POST 201 y totales correctos |
| PF-18 | Verificar inventario | Stock descontado según cantidades |
| PF-19 | Intentar venta sin stock | 400, sin venta ni descuento parcial |
| PF-20 | Historial por cliente y fechas | Solo ventas del filtro seleccionado |
| PF-21 | Detener la API y abrir cualquier módulo Web | Página de error controlado, sin acceso directo a SQL Server |
| PF-22 | Crear o renombrar una categoría con un nombre existente | Mensaje dentro del formulario: `Ya existe una categoría con ese nombre.` |
| PF-23 | Registrar o editar un cliente con cédula/RUC existente | Mensaje dentro del formulario, sin detener el depurador |
| PF-24 | Eliminar una bicicleta o cliente relacionado con ventas | Mensaje de conflicto dentro de la Web, sin salir al código fuente |
| PF-25 | Registrar una venta sin stock suficiente | Mensaje de stock disponible dentro del formulario y transacción revertida |

Pruebas automatizadas:

```powershell
dotnet test BikeStore.sln
```

Las pruebas automatizadas cubren creación, actualización y eliminación/desactivación
de los catálogos, asignación del estado de inventario y registro transaccional de ventas.
