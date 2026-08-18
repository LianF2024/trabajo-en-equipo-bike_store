using BikeStore.Application.Common;
using BikeStore.Application.Contracts;
using BikeStore.Application.DTOs;
using BikeStore.Application.Interfaces;
using BikeStore.Domain.Entities;

namespace BikeStore.Application.Services;

public sealed class CategoryService(IStoreRepository repository) : ICategoryService
{
    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(string? search, bool includeInactive, CancellationToken cancellationToken)
        => (await repository.GetCategoriesAsync(search, includeInactive, cancellationToken)).Select(Map).ToList();

    public async Task<CategoryDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        => Map(await FindAsync(id, cancellationToken));

    public async Task<CategoryDto> CreateAsync(SaveCategoryRequest request, CancellationToken cancellationToken)
    {
        Validate(request);
        var name = request.Name.Trim();
        if (await repository.CategoryNameExistsAsync(name, null, cancellationToken))
            throw new ConflictException("Ya existe una categoría con ese nombre.");

        var entity = new Category { Name = name, Description = Clean(request.Description), Active = request.Active };
        repository.AddCategory(entity);
        await repository.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task UpdateAsync(int id, SaveCategoryRequest request, CancellationToken cancellationToken)
    {
        Validate(request);
        var entity = await FindAsync(id, cancellationToken);
        var name = request.Name.Trim();
        if (await repository.CategoryNameExistsAsync(name, id, cancellationToken))
            throw new ConflictException("Ya existe una categoría con ese nombre.");

        entity.Name = name;
        entity.Description = Clean(request.Description);
        entity.Active = request.Active;
        await repository.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await FindAsync(id, cancellationToken);
        entity.Active = false;
        await repository.SaveChangesAsync(cancellationToken);
    }

    private async Task<Category> FindAsync(int id, CancellationToken cancellationToken)
        => await repository.GetCategoryAsync(id, cancellationToken)
           ?? throw new NotFoundException("Categoría no encontrada.");

    private static CategoryDto Map(Category category) => new(category.Id, category.Name, category.Description, category.Active);
    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static void Validate(SaveCategoryRequest request)
    {
        var nameLength = request.Name?.Trim().Length ?? 0;
        if (nameLength < 2 || nameLength > 100)
            throw new BusinessException("El nombre debe tener entre 2 y 100 caracteres.");
        if (request.Description?.Trim().Length > 250)
            throw new BusinessException("La descripción no puede superar los 250 caracteres.");
    }
}
