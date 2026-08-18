using BikeStore.Domain.Enums;

namespace BikeStore.Domain.Entities;

public sealed class Bicycle
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public BicycleStatus Status { get; set; } = BicycleStatus.Disponible;
    public Category Category { get; set; } = null!;
    public ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
}
