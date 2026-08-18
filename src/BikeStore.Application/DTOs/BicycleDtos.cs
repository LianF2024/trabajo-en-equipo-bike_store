using System.ComponentModel.DataAnnotations;

namespace BikeStore.Application.DTOs;

public sealed record BicycleDto(
    int Id,
    int CategoryId,
    string Category,
    string Brand,
    string Model,
    decimal Price,
    int Stock,
    string Status);

public sealed class SaveBicycleRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione una categoría válida.")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "La marca es obligatoria.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "La marca debe tener entre 2 y 100 caracteres.")]
    public string Brand { get; set; } = string.Empty;

    [Required(ErrorMessage = "El modelo es obligatorio.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "El modelo debe tener entre 1 y 100 caracteres.")]
    public string Model { get; set; } = string.Empty;

    public decimal Price { get; set; }

    [Range(0, 100000, ErrorMessage = "El stock debe estar entre 0 y 100 000.")]
    public int Stock { get; set; }
}

public sealed class BicycleFilter
{
    // El requisito denomina este filtro "nombre"; en el modelo mínimo
    // se aplica sobre Marca y Modelo.
    public string? Name { get; set; }
    public int? CategoryId { get; set; }
    public string? Category { get; set; }
    public string? Brand { get; set; }
    public bool LowStock { get; set; }
    public bool OutOfStock { get; set; }
    [Range(1, 1000)] public int LowStockThreshold { get; set; } = 5;
}
