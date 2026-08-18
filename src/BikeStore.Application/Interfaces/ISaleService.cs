using BikeStore.Application.DTOs;

namespace BikeStore.Application.Interfaces;

public interface ISaleService
{
    Task<IReadOnlyList<SaleDto>> GetAllAsync(int? customerId, DateTime? from, DateTime? to, CancellationToken cancellationToken);
    Task<SaleDto> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<SaleDto> CreateAsync(CreateSaleRequest request, CancellationToken cancellationToken);
}
