using BikeStore.Application.Common;
using BikeStore.Application.Contracts;
using BikeStore.Application.DTOs;
using BikeStore.Application.Interfaces;
using BikeStore.Domain.Entities;
using BikeStore.Domain.Enums;

namespace BikeStore.Application.Services;

public sealed class BicycleService(IStoreRepository repository, BusinessOptions options) : IBicycleService
{
    public async Task<IReadOnlyList<BicycleDto>> GetAllAsync(BicycleFilter filter, CancellationToken cancellationToken)
        => (await repository.GetBicyclesAsync(filter, cancellationToken)).Select(Map).ToList();

    public async Task<BicycleDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        => Map(await FindAsync(id, cancellationToken));

    public async Task<BicycleDto> CreateAsync(SaveBicycleRequest request, CancellationToken cancellationToken)
    {
        Validate(request);
        await ValidateCategoryAsync(request.CategoryId, cancellationToken);
        var entity = new Bicycle();
        Apply(entity, request);
        repository.AddBicycle(entity);
        await repository.SaveChangesAsync(cancellationToken);
        entity = await FindAsync(entity.Id, cancellationToken);
        return Map(entity);
    }

    public async Task UpdateAsync(int id, SaveBicycleRequest request, CancellationToken cancellationToken)
    {
        Validate(request);
        var entity = await FindAsync(id, cancellationToken);
        await ValidateCategoryAsync(request.CategoryId, cancellationToken);
        Apply(entity, request);
        await repository.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await FindAsync(id, cancellationToken);
        repository.RemoveBicycle(entity);
        await repository.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateCategoryAsync(int id, CancellationToken cancellationToken)
    {
        var category = await repository.GetCategoryAsync(id, cancellationToken);
        if (category is null || !category.Active) throw new BusinessException("La categoría seleccionada no existe o está inactiva.");
    }

    private static void Validate(SaveBicycleRequest request)
    {
        if (request.CategoryId <= 0)
            throw new BusinessException("Seleccione una categoría válida.");
        ValidateText(request.Brand, 2, 100, "La marca debe tener entre 2 y 100 caracteres.");
        ValidateText(request.Model, 1, 100, "El modelo debe tener entre 1 y 100 caracteres.");
        if (request.Price < 0.01m || request.Price > 9999999.99m)
            throw new BusinessException("El precio debe estar entre 0,01 y 9 999 999,99.");
        if (request.Stock < 0 || request.Stock > 100000)
            throw new BusinessException("El stock debe estar entre 0 y 100 000.");
    }

    private static void ValidateText(string? value, int minimum, int maximum, string message)
    {
        var length = value?.Trim().Length ?? 0;
        if (length < minimum || length > maximum)
            throw new BusinessException(message);
    }

    private async Task<Bicycle> FindAsync(int id, CancellationToken cancellationToken)
        => await repository.GetBicycleAsync(id, cancellationToken)
           ?? throw new NotFoundException("Bicicleta no encontrada.");

    private void Apply(Bicycle entity, SaveBicycleRequest request)
    {
        entity.CategoryId = request.CategoryId;
        entity.Brand = request.Brand.Trim();
        entity.Model = request.Model.Trim();
        entity.Price = decimal.Round(request.Price, 2);
        entity.Stock = request.Stock;
        entity.Status = GetStatus(entity.Stock);
    }

    private BicycleStatus GetStatus(int stock)
        => stock == 0 ? BicycleStatus.Agotado : stock <= options.LowStockThreshold ? BicycleStatus.BajoStock : BicycleStatus.Disponible;

    private static BicycleDto Map(Bicycle entity) => new(
        entity.Id, entity.CategoryId, entity.Category?.Name ?? string.Empty, entity.Brand, entity.Model,
        entity.Price, entity.Stock, StatusText(entity.Status));

    private static string StatusText(BicycleStatus status) => status switch
    {
        BicycleStatus.BajoStock => "Bajo stock",
        BicycleStatus.Agotado => "Agotado",
        BicycleStatus.Inactivo => "Inactivo",
        _ => "Disponible"
    };
}
