namespace BikeStore.Domain.Entities;

public sealed class Customer
{
    public int Id { get; set; }
    public string Identification { get; set; } = string.Empty;
    public string FirstNames { get; set; } = string.Empty;
    public string LastNames { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
