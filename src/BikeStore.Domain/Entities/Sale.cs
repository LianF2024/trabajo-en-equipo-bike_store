namespace BikeStore.Domain.Entities;

public sealed class Sale
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int CustomerId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Vat { get; set; }
    public decimal Total { get; set; }
    public Customer Customer { get; set; } = null!;
    public ICollection<SaleDetail> Details { get; set; } = new List<SaleDetail>();
}
