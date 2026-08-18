using BikeStore.Application.Common;
using BikeStore.Application.Contracts;
using BikeStore.Application.DTOs;
using BikeStore.Application.Services;
using BikeStore.Domain.Entities;

namespace BikeStore.Application.Tests;

public sealed class CatalogCrudServiceTests
{
    [Fact]
    public async Task CategoryService_CompletesCreateUpdateAndDelete()
    {
        var repository = new CrudRepositoryStub();
        var service = new CategoryService(repository);

        var created = await service.CreateAsync(new SaveCategoryRequest
        {
            Name = "  Montaña  ",
            Description = "Terreno irregular",
            Active = true
        }, CancellationToken.None);
        Assert.True(created.Id > 0);

        await service.UpdateAsync(created.Id, new SaveCategoryRequest
        {
            Name = "Montaña Pro",
            Description = "Competencia",
            Active = true
        }, CancellationToken.None);
        Assert.Equal("Montaña Pro", repository.Categories.Single().Name);

        await service.DeleteAsync(created.Id, CancellationToken.None);
        Assert.False(repository.Categories.Single().Active);
    }

    [Fact]
    public async Task CustomerService_CompletesCreateUpdateAndDelete()
    {
        var repository = new CrudRepositoryStub();
        var service = new CustomerService(repository);

        var created = await service.CreateAsync(new SaveCustomerRequest
        {
            Identification = "0102030405",
            FirstNames = "Ana María",
            LastNames = "Vera",
            Phone = "0990000000",
            Email = "ANA@EJEMPLO.COM"
        }, CancellationToken.None);
        Assert.True(created.Id > 0);

        await service.UpdateAsync(created.Id, new SaveCustomerRequest
        {
            Identification = "0102030405",
            FirstNames = "Ana",
            LastNames = "Vera López",
            Phone = null,
            Email = "ana@ejemplo.com"
        }, CancellationToken.None);
        Assert.Equal("Vera López", repository.Customers.Single().LastNames);
        Assert.Null(repository.Customers.Single().Phone);

        await service.DeleteAsync(created.Id, CancellationToken.None);
        Assert.Empty(repository.Customers);
    }

    private sealed class CrudRepositoryStub : IStoreRepository
    {
        public List<Category> Categories { get; } = [];
        public List<Bicycle> Bicycles { get; } = [];
        public List<Customer> Customers { get; } = [];

        public Task<IReadOnlyList<Category>> GetCategoriesAsync(string? search, bool includeInactive, CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<Category>>(Categories);
        public Task<Category?> GetCategoryAsync(int id, CancellationToken cancellationToken)
            => Task.FromResult(Categories.SingleOrDefault(item => item.Id == id));
        public Task<bool> CategoryNameExistsAsync(string name, int? exceptId, CancellationToken cancellationToken)
            => Task.FromResult(Categories.Any(item => item.Name == name && (!exceptId.HasValue || item.Id != exceptId.Value)));
        public void AddCategory(Category category)
        {
            category.Id = Categories.Count + 1;
            Categories.Add(category);
        }

        public Task<IReadOnlyList<Bicycle>> GetBicyclesAsync(BicycleFilter filter, CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<Bicycle>>(Bicycles);
        public Task<Bicycle?> GetBicycleAsync(int id, CancellationToken cancellationToken)
            => Task.FromResult(Bicycles.SingleOrDefault(item => item.Id == id));
        public Task<IReadOnlyList<Bicycle>> GetBicyclesByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<Bicycle>>(Bicycles.Where(item => ids.Contains(item.Id)).ToList());
        public void AddBicycle(Bicycle bicycle) => Bicycles.Add(bicycle);
        public void RemoveBicycle(Bicycle bicycle) => Bicycles.Remove(bicycle);

        public Task<IReadOnlyList<Customer>> GetCustomersAsync(string? identification, string? lastName, CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<Customer>>(Customers);
        public Task<Customer?> GetCustomerAsync(int id, CancellationToken cancellationToken)
            => Task.FromResult(Customers.SingleOrDefault(item => item.Id == id));
        public Task<bool> CustomerIdentificationExistsAsync(string identification, int? exceptId, CancellationToken cancellationToken)
            => Task.FromResult(Customers.Any(item => item.Identification == identification && (!exceptId.HasValue || item.Id != exceptId.Value)));
        public void AddCustomer(Customer customer)
        {
            customer.Id = Customers.Count + 1;
            Customers.Add(customer);
        }
        public void RemoveCustomer(Customer customer) => Customers.Remove(customer);

        public Task<IReadOnlyList<Sale>> GetSalesAsync(int? customerId, DateTime? from, DateTime? to, CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<Sale>>([]);
        public Task<Sale?> GetSaleAsync(int id, CancellationToken cancellationToken)
            => Task.FromResult<Sale?>(null);
        public void AddSale(Sale sale) { }
        public Task<IAppTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
            => throw new NotSupportedException();
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
            => Task.FromResult(1);
    }
}
