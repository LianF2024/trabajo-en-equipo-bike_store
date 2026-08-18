using BikeStore.Application.Common;
using BikeStore.Application.Contracts;
using BikeStore.Application.DTOs;
using BikeStore.Application.Services;
using BikeStore.Domain.Entities;
using BikeStore.Domain.Enums;

namespace BikeStore.Application.Tests;

public sealed class SaleServiceTests
{
    [Fact]
    public async Task CreateAsync_CalculatesVatAndUpdatesStock()
    {
        var repository = TestRepository.Create(stock: 10, price: 100m);
        var service = new SaleService(repository, new BusinessOptions { VatRate = 0.15m, LowStockThreshold = 5 });

        var result = await service.CreateAsync(new CreateSaleRequest
        {
            CustomerId = 1,
            Items = [new CreateSaleItemRequest { BicycleId = 1, Quantity = 2 }]
        }, CancellationToken.None);

        Assert.Equal(200m, result.Subtotal);
        Assert.Equal(30m, result.Vat);
        Assert.Equal(230m, result.Total);
        Assert.Equal(8, repository.Bicycles.Single().Stock);
        Assert.True(repository.Transaction!.Committed);
    }

    [Fact]
    public async Task CreateAsync_InsufficientStock_RollsBackAndThrows()
    {
        var repository = TestRepository.Create(stock: 1, price: 100m);
        var service = new SaleService(repository, new BusinessOptions());

        var exception = await Assert.ThrowsAsync<BusinessException>(() => service.CreateAsync(new CreateSaleRequest
        {
            CustomerId = 1,
            Items = [new CreateSaleItemRequest { BicycleId = 1, Quantity = 2 }]
        }, CancellationToken.None));

        Assert.Contains("Stock insuficiente", exception.Message);
        Assert.Equal(1, repository.Bicycles.Single().Stock);
        Assert.True(repository.Transaction!.RolledBack);
        Assert.Empty(repository.Sales);
    }

    private sealed class TestRepository : IStoreRepository
    {
        public List<Bicycle> Bicycles { get; } = [];
        public List<Customer> Customers { get; } = [];
        public List<Sale> Sales { get; } = [];
        public TestTransaction? Transaction { get; private set; }

        public static TestRepository Create(int stock, decimal price)
        {
            var repository = new TestRepository();
            var category = new Category { Id = 1, Name = "Montaña" };
            repository.Bicycles.Add(new Bicycle { Id = 1, Brand = "Giant", Model = "Talon 2", CategoryId = 1, Category = category, Stock = stock, Price = price });
            repository.Customers.Add(new Customer { Id = 1, Identification = "0000000001", FirstNames = "Cliente", LastNames = "Prueba" });
            return repository;
        }

        public Task<IReadOnlyList<Category>> GetCategoriesAsync(string? search, bool includeInactive, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<Category>>([]);
        public Task<Category?> GetCategoryAsync(int id, CancellationToken cancellationToken) => Task.FromResult<Category?>(null);
        public Task<bool> CategoryNameExistsAsync(string name, int? exceptId, CancellationToken cancellationToken) => Task.FromResult(false);
        public void AddCategory(Category category) { }
        public Task<IReadOnlyList<Bicycle>> GetBicyclesAsync(BicycleFilter filter, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<Bicycle>>(Bicycles);
        public Task<Bicycle?> GetBicycleAsync(int id, CancellationToken cancellationToken) => Task.FromResult(Bicycles.SingleOrDefault(x => x.Id == id));
        public Task<IReadOnlyList<Bicycle>> GetBicyclesByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<Bicycle>>(Bicycles.Where(x => ids.Contains(x.Id)).ToList());
        public void AddBicycle(Bicycle bicycle) => Bicycles.Add(bicycle);
        public void RemoveBicycle(Bicycle bicycle) => Bicycles.Remove(bicycle);
        public Task<IReadOnlyList<Customer>> GetCustomersAsync(string? identification, string? lastName, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<Customer>>(Customers);
        public Task<Customer?> GetCustomerAsync(int id, CancellationToken cancellationToken) => Task.FromResult(Customers.SingleOrDefault(x => x.Id == id));
        public Task<bool> CustomerIdentificationExistsAsync(string identification, int? exceptId, CancellationToken cancellationToken) => Task.FromResult(false);
        public void AddCustomer(Customer customer) => Customers.Add(customer);
        public void RemoveCustomer(Customer customer) => Customers.Remove(customer);
        public Task<IReadOnlyList<Sale>> GetSalesAsync(int? customerId, DateTime? from, DateTime? to, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<Sale>>(Sales);
        public Task<Sale?> GetSaleAsync(int id, CancellationToken cancellationToken) => Task.FromResult(Sales.SingleOrDefault(x => x.Id == id));
        public void AddSale(Sale sale)
        {
            sale.Id = Sales.Count + 1;
            sale.Customer = Customers.Single(x => x.Id == sale.CustomerId);
            var nextDetailId = 1;
            foreach (var detail in sale.Details)
            {
                detail.Id = nextDetailId++;
                detail.Bicycle = Bicycles.Single(x => x.Id == detail.BicycleId);
                detail.Sale = sale;
            }
            Sales.Add(sale);
        }
        public Task<IAppTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
        {
            Transaction = new TestTransaction(Bicycles.ToDictionary(x => x.Id, x => (x.Stock, x.Status)), Sales);
            return Task.FromResult<IAppTransaction>(Transaction);
        }
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken) => Task.FromResult(1);
    }

    private sealed class TestTransaction(Dictionary<int, (int Stock, BicycleStatus Status)> original, List<Sale> sales) : IAppTransaction
    {
        public bool Committed { get; private set; }
        public bool RolledBack { get; private set; }
        public Task CommitAsync(CancellationToken cancellationToken) { Committed = true; return Task.CompletedTask; }
        public Task RollbackAsync(CancellationToken cancellationToken)
        {
            RolledBack = true;
            sales.Clear();
            return Task.CompletedTask;
        }
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
