namespace BikeStore.Domain.Entities;

public sealed class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool Active { get; set; } = true;
    public ICollection<Bicycle> Bicycles { get; set; } = new List<Bicycle>();
}
