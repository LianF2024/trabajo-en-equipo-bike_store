using BikeStore.Application.DTOs;
using BikeStore.Domain.Entities;

namespace BikeStore.Application.Contracts;

public interface IStoreRepository
{
    Task<IReadOnlyList<Category>> GetCategoriesAsync(string? search, bool includeInactive, CancellationToken cancellationToken);
    Task<Category?> GetCategoryAsync(int id, CancellationToken cancellationToken);
    Task<bool> CategoryNameExistsAsync(string name, int? exceptId, CancellationToken cancellationToken);
    void AddCategory(Category category);

    Task<IReadOnlyList<Bicycle>> GetBicyclesAsync(BicycleFilter filter, CancellationToken cancellationToken);
    Task<Bicycle?> GetBicycleAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Bicycle>> GetBicyclesByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken);
    void AddBicycle(Bicycle bicycle);
    void RemoveBicycle(Bicycle bicycle);

    Task<IReadOnlyList<Customer>> GetCustomersAsync(string? identification, string? lastName, CancellationToken cancellationToken);
    Task<Customer?> GetCustomerAsync(int id, CancellationToken cancellationToken);
    Task<bool> CustomerIdentificationExistsAsync(string identification, int? exceptId, CancellationToken cancellationToken);
    void AddCustomer(Customer customer);
    void RemoveCustomer(Customer customer);

    Task<IReadOnlyList<Sale>> GetSalesAsync(int? customerId, DateTime? from, DateTime? to, CancellationToken cancellationToken);
    Task<Sale?> GetSaleAsync(int id, CancellationToken cancellationToken);
    void AddSale(Sale sale);

    Task<IAppTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

public interface IAppTransaction : IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken);
    Task RollbackAsync(CancellationToken cancellationToken);
}
