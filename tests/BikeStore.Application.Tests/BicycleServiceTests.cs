using BikeStore.Application.Common;
using BikeStore.Application.Contracts;
using BikeStore.Application.DTOs;
using BikeStore.Application.Services;
using BikeStore.Domain.Entities;
using BikeStore.Domain.Enums;

namespace BikeStore.Application.Tests;

public sealed class BicycleServiceTests
{
    [Theory]
    [InlineData(0, BicycleStatus.Agotado)]
    [InlineData(3, BicycleStatus.BajoStock)]
    [InlineData(8, BicycleStatus.Disponible)]
    public async Task CreateAsync_AssignsStatusFromStock(int stock, BicycleStatus expected)
    {
        var repository = new BicycleRepositoryStub();
        var service = new BicycleService(repository, new BusinessOptions { LowStockThreshold = 5 });

        await service.CreateAsync(new SaveBicycleRequest
        {
            CategoryId = 1, Brand = "Marca", Model = "Modelo", Price = 100m, Stock = stock
        }, CancellationToken.None);

        Assert.Equal(expected, repository.Bicycles.Single().Status);
    }

    [Fact]
    public async Task UpdateAsync_ChangesPriceStockAndStatus()
    {
        var repository = new BicycleRepositoryStub();
        repository.Bicycles.Add(new Bicycle
        {
            Id = 1,
            CategoryId = 1,
            Category = repository.Category,
            Brand = "Marca anterior",
            Model = "Modelo anterior",
            Price = 100m,
            Stock = 9
        });
        var service = new BicycleService(repository, new BusinessOptions { LowStockThreshold = 5 });

        await service.UpdateAsync(1, new SaveBicycleRequest
        {
            CategoryId = 1,
            Brand = "Trek",
            Model = "Marlin 7",
            Price = 1250.50m,
            Stock = 3
        }, CancellationToken.None);

        var bicycle = repository.Bicycles.Single();
        Assert.Equal("Trek", bicycle.Brand);
        Assert.Equal("Marlin 7", bicycle.Model);
        Assert.Equal(1250.50m, bicycle.Price);
        Assert.Equal(3, bicycle.Stock);
        Assert.Equal(BicycleStatus.BajoStock, bicycle.Status);
    }

    [Fact]
    public async Task DeleteAsync_RemovesExistingBicycle()
    {
        var repository = new BicycleRepositoryStub();
        repository.Bicycles.Add(new Bicycle
        {
            Id = 1,
            CategoryId = 1,
            Category = repository.Category,
            Brand = "Trek",
            Model = "Marlin",
            Price = 100m
        });
        var service = new BicycleService(repository, new BusinessOptions());

        await service.DeleteAsync(1, CancellationToken.None);

        Assert.Empty(repository.Bicycles);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10000000)]
    public async Task CreateAsync_InvalidPrice_Throws(decimal price)
    {
        var service = new BicycleService(new BicycleRepositoryStub(), new BusinessOptions());

        var exception = await Assert.ThrowsAsync<BusinessException>(() => service.CreateAsync(new SaveBicycleRequest
        {
            CategoryId = 1,
            Brand = "Trek",
            Model = "Marlin",
            Price = price,
            Stock = 1
        }, CancellationToken.None));

        Assert.Contains("precio", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class BicycleRepositoryStub : IStoreRepository
    {
        public Category Category { get; } = new() { Id = 1, Name = "Ruta", Active = true };
        public List<Bicycle> Bicycles { get; } = [];
        public Task<Category?> GetCategoryAsync(int id, CancellationToken cancellationToken) => Task.FromResult<Category?>(id == 1 ? Category : null);
        public void AddBicycle(Bicycle bicycle) { bicycle.Id = Bicycles.Count + 1; bicycle.Category = Category; Bicycles.Add(bicycle); }
        public void RemoveBicycle(Bicycle bicycle) => Bicycles.Remove(bicycle);
        public Task<Bicycle?> GetBicycleAsync(int id, CancellationToken cancellationToken) => Task.FromResult(Bicycles.SingleOrDefault(x => x.Id == id));
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken) => Task.FromResult(1);
        public Task<IReadOnlyList<Category>> GetCategoriesAsync(string? search, bool includeInactive, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<Category>>([Category]);
        public Task<bool> CategoryNameExistsAsync(string name, int? exceptId, CancellationToken cancellationToken) => Task.FromResult(false);
        public void AddCategory(Category category) { }
        public Task<IReadOnlyList<Bicycle>> GetBicyclesAsync(BicycleFilter filter, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<Bicycle>>(Bicycles);
        public Task<IReadOnlyList<Bicycle>> GetBicyclesByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<Bicycle>>(Bicycles);
        public Task<IReadOnlyList<Customer>> GetCustomersAsync(string? identification, string? lastName, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<Customer>>([]);
        public Task<Customer?> GetCustomerAsync(int id, CancellationToken cancellationToken) => Task.FromResult<Customer?>(null);
        public Task<bool> CustomerIdentificationExistsAsync(string identification, int? exceptId, CancellationToken cancellationToken) => Task.FromResult(false);
        public void AddCustomer(Customer customer) { }
        public void RemoveCustomer(Customer customer) { }
        public Task<IReadOnlyList<Sale>> GetSalesAsync(int? customerId, DateTime? from, DateTime? to, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<Sale>>([]);
        public Task<Sale?> GetSaleAsync(int id, CancellationToken cancellationToken) => Task.FromResult<Sale?>(null);
        public void AddSale(Sale sale) { }
        public Task<IAppTransaction> BeginTransactionAsync(CancellationToken cancellationToken) => throw new NotSupportedException();
    }
}
