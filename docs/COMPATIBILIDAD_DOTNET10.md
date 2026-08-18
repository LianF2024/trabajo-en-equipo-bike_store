# Compatibilidad de .NET 10

- Todos los proyectos utilizan `net10.0`.
- El lenguaje configurado es C# 14.
- `global.json` solicita el SDK `10.0.100` y permite avanzar a una banda posterior de .NET 10.
- Entity Framework Core y el proveedor SQL Server utilizan la versión 10.

Comprobación:

```powershell
dotnet --version
dotnet restore BikeStore.sln
dotnet build BikeStore.sln
dotnet test BikeStore.sln
```

El primer comando debe mostrar `10.0.x`. Si Visual Studio 2022 no reconoce el framework, instale el SDK de .NET 10 y actualice la carga de trabajo ASP.NET; alternativamente, ejecute los comandos anteriores desde PowerShell.
