using BikeStore.Application.DTOs;

namespace BikeStore.Application.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetAllAsync(string? search, bool includeInactive, CancellationToken cancellationToken);
    Task<CategoryDto> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<CategoryDto> CreateAsync(SaveCategoryRequest request, CancellationToken cancellationToken);
    Task UpdateAsync(int id, SaveCategoryRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
