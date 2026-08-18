namespace BikeStore.Domain.Entities;

public sealed class SaleDetail
{
    public int Id { get; set; }
    public int SaleId { get; set; }
    public int BicycleId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
    public Sale Sale { get; set; } = null!;
    public Bicycle Bicycle { get; set; } = null!;
}
