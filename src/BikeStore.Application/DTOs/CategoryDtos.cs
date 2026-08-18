using System.ComponentModel.DataAnnotations;

namespace BikeStore.Application.DTOs;

public sealed record CategoryDto(int Id, string Name, string? Description, bool Active);

public sealed class SaveCategoryRequest
{
    [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(250, ErrorMessage = "La descripción no puede superar los 250 caracteres.")]
    public string? Description { get; set; }

    public bool Active { get; set; } = true;
}
