using System.ComponentModel.DataAnnotations;

namespace BikeStore.Application.DTOs;

public sealed record CustomerDto(
    int Id,
    string Identification,
    string FirstNames,
    string LastNames,
    string? Phone,
    string? Email)
{
    public string FullName => $"{FirstNames} {LastNames}";
}

public sealed class SaveCustomerRequest
{
    [Required(ErrorMessage = "La cédula/RUC es obligatoria.")]
    [RegularExpression("^[0-9]{10,13}$", ErrorMessage = "La cédula/RUC debe tener entre 10 y 13 dígitos.")]
    public string Identification { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Los nombres deben tener entre 2 y 100 caracteres.")]
    public string FirstNames { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los apellidos son obligatorios.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Los apellidos deben tener entre 2 y 100 caracteres.")]
    public string LastNames { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Ingrese un teléfono válido.")]
    [StringLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres.")]
    public string? Phone { get; set; }

    [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
    [StringLength(150, ErrorMessage = "El correo no puede superar los 150 caracteres.")]
    public string? Email { get; set; }
}
