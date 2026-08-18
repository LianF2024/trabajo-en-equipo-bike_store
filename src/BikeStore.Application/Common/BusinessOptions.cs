namespace BikeStore.Application.Common;

public sealed class BusinessOptions
{
    public decimal VatRate { get; init; } = 0.15m;
    public int LowStockThreshold { get; init; } = 5;
}
