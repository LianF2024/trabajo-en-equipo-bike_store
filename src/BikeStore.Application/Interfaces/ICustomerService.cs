using BikeStore.Application.DTOs;

namespace BikeStore.Application.Interfaces;

public interface ICustomerService
{
    Task<IReadOnlyList<CustomerDto>> GetAllAsync(string? identification, string? lastName, CancellationToken cancellationToken);
    Task<CustomerDto> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<CustomerDto> CreateAsync(SaveCustomerRequest request, CancellationToken cancellationToken);
    Task UpdateAsync(int id, SaveCustomerRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
