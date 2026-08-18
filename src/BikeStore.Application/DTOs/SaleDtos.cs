using System.ComponentModel.DataAnnotations;

namespace BikeStore.Application.DTOs;

public sealed class CreateSaleRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un cliente válido.")]
    public int CustomerId { get; set; }

    [Required, MinLength(1, ErrorMessage = "Agregue al menos una bicicleta.")]
    public List<CreateSaleItemRequest> Items { get; set; } = [];
}

public sealed class CreateSaleItemRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione una bicicleta válida.")]
    public int BicycleId { get; set; }
    [Range(1, 1000, ErrorMessage = "La cantidad debe estar entre 1 y 1000.")]
    public int Quantity { get; set; }
}

public sealed record SaleDetailDto(
    int Id,
    int BicycleId,
    string Bicycle,
    int Quantity,
    decimal UnitPrice,
    decimal Subtotal);

public sealed record SaleDto(
    int Id,
    DateTime Date,
    int CustomerId,
    string Customer,
    decimal Subtotal,
    decimal Vat,
    decimal Total,
    IReadOnlyCollection<SaleDetailDto> Details);
