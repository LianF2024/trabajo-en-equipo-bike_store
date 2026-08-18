using System.ComponentModel.DataAnnotations;

namespace BikeStore.Web.Models;

public sealed record CategoriaVm(int Id, string Name, string? Description, bool Active);

public sealed class CategoriaFormVm
{
    public int Id { get; set; }
    [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    [Display(Name = "Nombre")]
    public string Name { get; set; } = string.Empty;
    [StringLength(250, ErrorMessage = "La descripción no puede superar los 250 caracteres."), Display(Name = "Descripción")]
    public string? Description { get; set; }
    [Display(Name = "Activa")] public bool Active { get; set; } = true;
}

public sealed record BicicletaVm(int Id, int CategoryId, string Category, string Brand, string Model, decimal Price, int Stock, string Status)
{
    public string DisplayName => $"{Brand} {Model}";
}

public sealed class BicicletaFormVm
{
    public int Id { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione una categoría válida."), Display(Name = "Categoría")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "La marca es obligatoria.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "La marca debe tener entre 2 y 100 caracteres.")]
    [Display(Name = "Marca")]
    public string Brand { get; set; } = string.Empty;

    [Required(ErrorMessage = "El modelo es obligatorio.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "El modelo debe tener entre 1 y 100 caracteres.")]
    [Display(Name = "Modelo")]
    public string Model { get; set; } = string.Empty;
    [Display(Name = "Precio")]
    public decimal Price { get; set; }
    [Range(0, 100000, ErrorMessage = "El stock debe estar entre 0 y 100 000."), Display(Name = "Stock")]
    public int Stock { get; set; }
}

public sealed record ClienteVm(int Id, string Identification, string FirstNames, string LastNames, string? Phone, string? Email)
{
    public string FullName => $"{FirstNames} {LastNames}";
}

public sealed class ClienteFormVm
{
    public int Id { get; set; }
    [Required(ErrorMessage = "La cédula/RUC es obligatoria.")]
    [RegularExpression("^[0-9]{10,13}$", ErrorMessage = "La cédula/RUC debe tener entre 10 y 13 dígitos.")]
    [Display(Name = "Cédula/RUC")]
    public string Identification { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Los nombres deben tener entre 2 y 100 caracteres.")]
    [Display(Name = "Nombres")]
    public string FirstNames { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los apellidos son obligatorios.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Los apellidos deben tener entre 2 y 100 caracteres.")]
    [Display(Name = "Apellidos")]
    public string LastNames { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Ingrese un teléfono válido.")]
    [StringLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres.")]
    [Display(Name = "Teléfono")]
    public string? Phone { get; set; }

    [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
    [StringLength(150, ErrorMessage = "El correo no puede superar los 150 caracteres.")]
    [Display(Name = "Correo")]
    public string? Email { get; set; }
}

public sealed record VentaDetalleVm(int Id, int BicycleId, string Bicycle, int Quantity, decimal UnitPrice, decimal Subtotal);
public sealed record VentaVm(int Id, DateTime Date, int CustomerId, string Customer, decimal Subtotal, decimal Vat, decimal Total, IReadOnlyCollection<VentaDetalleVm> Details);

public sealed class VentaFormVm
{
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un cliente válido."), Display(Name = "Cliente")]
    public int CustomerId { get; set; }
    public List<VentaLineaFormVm> Items { get; set; } = [new()];
}

public sealed class VentaLineaFormVm
{
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione una bicicleta válida."), Display(Name = "Bicicleta")]
    public int BicycleId { get; set; }
    [Range(1, 1000, ErrorMessage = "La cantidad debe estar entre 1 y 1000."), Display(Name = "Cantidad")]
    public int Quantity { get; set; } = 1;
}

public sealed class DashboardVm
{
    public int Bicycles { get; init; }
    public int LowStock { get; init; }
    public int Customers { get; init; }
    public int TodaySales { get; init; }
    public decimal TodayRevenue { get; init; }
    public IReadOnlyList<VentaVm> RecentSales { get; init; } = [];
}
