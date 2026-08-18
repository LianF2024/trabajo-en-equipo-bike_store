using BikeStore.Application.Common;
using BikeStore.Application.Contracts;
using BikeStore.Application.DTOs;
using BikeStore.Domain.Entities;
using BikeStore.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BikeStore.Infrastructure.Repositories;

public sealed class StoreRepository(BikeStoreDbContext context) : IStoreRepository
{
    public async Task<IReadOnlyList<Category>> GetCategoriesAsync(string? search, bool includeInactive, CancellationToken cancellationToken)
    {
        var query = context.Categories.AsNoTracking().AsQueryable();
        if (!includeInactive) query = query.Where(x => x.Active);
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.Name.Contains(search.Trim()));
        return await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);
    }

    public Task<Category?> GetCategoryAsync(int id, CancellationToken cancellationToken)
        => context.Categories.Include(x => x.Bicycles).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> CategoryNameExistsAsync(string name, int? exceptId, CancellationToken cancellationToken)
        => context.Categories.AnyAsync(x => x.Name == name && (!exceptId.HasValue || x.Id != exceptId), cancellationToken);

    public void AddCategory(Category category) => context.Categories.Add(category);

    public async Task<IReadOnlyList<Bicycle>> GetBicyclesAsync(BicycleFilter filter, CancellationToken cancellationToken)
    {
        var query = context.Bicycles.AsNoTracking().Include(x => x.Category).AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter.Name)) query = query.Where(x => x.Brand.Contains(filter.Name.Trim()) || x.Model.Contains(filter.Name.Trim()));
        if (filter.CategoryId.HasValue) query = query.Where(x => x.CategoryId == filter.CategoryId.Value);
        if (!string.IsNullOrWhiteSpace(filter.Category)) query = query.Where(x => x.Category.Name.Contains(filter.Category.Trim()));
        if (!string.IsNullOrWhiteSpace(filter.Brand)) query = query.Where(x => x.Brand.Contains(filter.Brand.Trim()));
        if (filter.OutOfStock) query = query.Where(x => x.Stock == 0);
        else if (filter.LowStock) query = query.Where(x => x.Stock > 0 && x.Stock <= filter.LowStockThreshold);
        return await query.OrderBy(x => x.Brand).ThenBy(x => x.Model).ToListAsync(cancellationToken);
    }

    public Task<Bicycle?> GetBicycleAsync(int id, CancellationToken cancellationToken)
        => context.Bicycles.Include(x => x.Category).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Bicycle>> GetBicyclesByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken)
        => await context.Bicycles.Include(x => x.Category).Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken);

    public void AddBicycle(Bicycle bicycle) => context.Bicycles.Add(bicycle);
    public void RemoveBicycle(Bicycle bicycle) => context.Bicycles.Remove(bicycle);

    public async Task<IReadOnlyList<Customer>> GetCustomersAsync(string? identification, string? lastName, CancellationToken cancellationToken)
    {
        var query = context.Customers.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(identification)) query = query.Where(x => x.Identification.Contains(identification.Trim()));
        if (!string.IsNullOrWhiteSpace(lastName)) query = query.Where(x => x.LastNames.Contains(lastName.Trim()));
        return await query.OrderBy(x => x.LastNames).ThenBy(x => x.FirstNames).ToListAsync(cancellationToken);
    }

    public Task<Customer?> GetCustomerAsync(int id, CancellationToken cancellationToken)
        => context.Customers.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> CustomerIdentificationExistsAsync(string identification, int? exceptId, CancellationToken cancellationToken)
        => context.Customers.AnyAsync(x => x.Identification == identification && (!exceptId.HasValue || x.Id != exceptId), cancellationToken);

    public void AddCustomer(Customer customer) => context.Customers.Add(customer);
    public void RemoveCustomer(Customer customer) => context.Customers.Remove(customer);

    public async Task<IReadOnlyList<Sale>> GetSalesAsync(int? customerId, DateTime? from, DateTime? to, CancellationToken cancellationToken)
    {
        var query = context.Sales.AsNoTracking().Include(x => x.Customer).Include(x => x.Details).ThenInclude(x => x.Bicycle).AsQueryable();
        if (customerId.HasValue) query = query.Where(x => x.CustomerId == customerId.Value);
        if (from.HasValue) query = query.Where(x => x.Date >= from.Value.Date);
        if (to.HasValue) query = query.Where(x => x.Date < to.Value.Date.AddDays(1));
        return await query.OrderByDescending(x => x.Date).ThenByDescending(x => x.Id).ToListAsync(cancellationToken);
    }

    public Task<Sale?> GetSaleAsync(int id, CancellationToken cancellationToken)
        => context.Sales.AsNoTracking().Include(x => x.Customer).Include(x => x.Details).ThenInclude(x => x.Bicycle).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public void AddSale(Sale sale) => context.Sales.Add(sale);

    public async Task<IAppTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
        => new EfAppTransaction(await context.Database.BeginTransactionAsync(cancellationToken));

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try { return await context.SaveChangesAsync(cancellationToken); }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictException("Los datos fueron modificados por otro usuario. Recargue la información e intente nuevamente.");
        }
        catch (DbUpdateException exception) when (
            exception.InnerException is SqlException { Number: 2601 or 2627 } ||
            exception.InnerException?.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) == true ||
            exception.InnerException?.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true)
        {
            throw new ConflictException("El registro duplica un valor que debe ser único.");
        }
        catch (DbUpdateException exception) when (
            exception.InnerException is SqlException { Number: 547 } ||
            exception.InnerException?.Message.Contains("REFERENCE", StringComparison.OrdinalIgnoreCase) == true ||
            exception.InnerException?.Message.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase) == true)
        {
            throw new ConflictException("No se puede eliminar el registro porque está relacionado con otras operaciones.");
        }
    }

    private sealed class EfAppTransaction(IDbContextTransaction transaction) : IAppTransaction
    {
        public Task CommitAsync(CancellationToken cancellationToken) => transaction.CommitAsync(cancellationToken);
        public Task RollbackAsync(CancellationToken cancellationToken) => transaction.RollbackAsync(cancellationToken);
        public ValueTask DisposeAsync() => transaction.DisposeAsync();
    }
}
