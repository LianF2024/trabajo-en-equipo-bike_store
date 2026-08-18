using BikeStore.Application.DTOs;

namespace BikeStore.Application.Interfaces;

public interface IBicycleService
{
    Task<IReadOnlyList<BicycleDto>> GetAllAsync(BicycleFilter filter, CancellationToken cancellationToken);
    Task<BicycleDto> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<BicycleDto> CreateAsync(SaveBicycleRequest request, CancellationToken cancellationToken);
    Task UpdateAsync(int id, SaveBicycleRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
