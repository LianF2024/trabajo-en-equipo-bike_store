using BikeStore.Web.Models;

namespace BikeStore.Web.Services;

public interface IBikeStoreApiClient
{
    Task<IReadOnlyList<CategoriaVm>> GetCategoriesAsync(string? search = null, bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<CategoriaVm> GetCategoryAsync(int id, CancellationToken cancellationToken = default);
    Task<CategoriaVm> CreateCategoryAsync(CategoriaFormVm model, CancellationToken cancellationToken = default);
    Task UpdateCategoryAsync(CategoriaFormVm model, CancellationToken cancellationToken = default);
    Task DeleteCategoryAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BicicletaVm>> GetBicyclesAsync(string? name = null, int? categoryId = null, string? brand = null, bool lowStock = false, bool outOfStock = false, CancellationToken cancellationToken = default);
    Task<BicicletaVm> GetBicycleAsync(int id, CancellationToken cancellationToken = default);
    Task<BicicletaVm> CreateBicycleAsync(BicicletaFormVm model, CancellationToken cancellationToken = default);
    Task UpdateBicycleAsync(BicicletaFormVm model, CancellationToken cancellationToken = default);
    Task DeleteBicycleAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ClienteVm>> GetCustomersAsync(string? identification = null, string? lastName = null, CancellationToken cancellationToken = default);
    Task<ClienteVm> GetCustomerAsync(int id, CancellationToken cancellationToken = default);
    Task<ClienteVm> CreateCustomerAsync(ClienteFormVm model, CancellationToken cancellationToken = default);
    Task UpdateCustomerAsync(ClienteFormVm model, CancellationToken cancellationToken = default);
    Task DeleteCustomerAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<VentaVm>> GetSalesAsync(int? customerId = null, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default);
    Task<VentaVm> GetSaleAsync(int id, CancellationToken cancellationToken = default);
    Task<VentaVm> CreateSaleAsync(VentaFormVm model, CancellationToken cancellationToken = default);
}
