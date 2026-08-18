using System.Net.Http.Json;
using System.Text.Json;
using BikeStore.Web.Models;

namespace BikeStore.Web.Services;

public sealed class BikeStoreApiClient(HttpClient httpClient) : IBikeStoreApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public Task<IReadOnlyList<CategoriaVm>> GetCategoriesAsync(string? search = null, bool includeInactive = false, CancellationToken cancellationToken = default)
        => GetListAsync<CategoriaVm>($"api/categorias?buscar={Uri.EscapeDataString(search ?? string.Empty)}&incluirInactivas={includeInactive}", cancellationToken);
    public Task<CategoriaVm> GetCategoryAsync(int id, CancellationToken cancellationToken = default) => GetAsync<CategoriaVm>($"api/categorias/{id}", cancellationToken);
    public Task<CategoriaVm> CreateCategoryAsync(CategoriaFormVm model, CancellationToken cancellationToken = default)
        => PostAsync<CategoriaVm>("api/categorias", model, "La API no devolvió la categoría registrada.", cancellationToken);
    public Task UpdateCategoryAsync(CategoriaFormVm model, CancellationToken cancellationToken = default) => SendAsync(HttpMethod.Put, $"api/categorias/{model.Id}", model, cancellationToken);
    public Task DeleteCategoryAsync(int id, CancellationToken cancellationToken = default) => SendAsync(HttpMethod.Delete, $"api/categorias/{id}", null, cancellationToken);

    public Task<IReadOnlyList<BicicletaVm>> GetBicyclesAsync(string? name = null, int? categoryId = null, string? brand = null, bool lowStock = false, bool outOfStock = false, CancellationToken cancellationToken = default)
        => GetListAsync<BicicletaVm>($"api/bicicletas?name={Uri.EscapeDataString(name ?? string.Empty)}&categoryId={categoryId}&brand={Uri.EscapeDataString(brand ?? string.Empty)}&lowStock={lowStock}&outOfStock={outOfStock}", cancellationToken);
    public Task<BicicletaVm> GetBicycleAsync(int id, CancellationToken cancellationToken = default) => GetAsync<BicicletaVm>($"api/bicicletas/{id}", cancellationToken);
    public Task<BicicletaVm> CreateBicycleAsync(BicicletaFormVm model, CancellationToken cancellationToken = default)
        => PostAsync<BicicletaVm>("api/bicicletas", model, "La API no devolvió la bicicleta registrada.", cancellationToken);
    public Task UpdateBicycleAsync(BicicletaFormVm model, CancellationToken cancellationToken = default) => SendAsync(HttpMethod.Put, $"api/bicicletas/{model.Id}", model, cancellationToken);
    public Task DeleteBicycleAsync(int id, CancellationToken cancellationToken = default) => SendAsync(HttpMethod.Delete, $"api/bicicletas/{id}", null, cancellationToken);

    public Task<IReadOnlyList<ClienteVm>> GetCustomersAsync(string? identification = null, string? lastName = null, CancellationToken cancellationToken = default)
        => GetListAsync<ClienteVm>($"api/clientes?cedula={Uri.EscapeDataString(identification ?? string.Empty)}&apellido={Uri.EscapeDataString(lastName ?? string.Empty)}", cancellationToken);
    public Task<ClienteVm> GetCustomerAsync(int id, CancellationToken cancellationToken = default) => GetAsync<ClienteVm>($"api/clientes/{id}", cancellationToken);
    public Task<ClienteVm> CreateCustomerAsync(ClienteFormVm model, CancellationToken cancellationToken = default)
        => PostAsync<ClienteVm>("api/clientes", model, "La API no devolvió el cliente registrado.", cancellationToken);
    public Task UpdateCustomerAsync(ClienteFormVm model, CancellationToken cancellationToken = default) => SendAsync(HttpMethod.Put, $"api/clientes/{model.Id}", model, cancellationToken);
    public Task DeleteCustomerAsync(int id, CancellationToken cancellationToken = default) => SendAsync(HttpMethod.Delete, $"api/clientes/{id}", null, cancellationToken);

    public Task<IReadOnlyList<VentaVm>> GetSalesAsync(int? customerId = null, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
        => GetListAsync<VentaVm>($"api/ventas?clienteId={customerId}&desde={FormatDate(from)}&hasta={FormatDate(to)}", cancellationToken);
    public Task<VentaVm> GetSaleAsync(int id, CancellationToken cancellationToken = default) => GetAsync<VentaVm>($"api/ventas/{id}", cancellationToken);
    public Task<VentaVm> CreateSaleAsync(VentaFormVm model, CancellationToken cancellationToken = default)
        => PostAsync<VentaVm>("api/ventas", model, "La API no devolvió la venta registrada.", cancellationToken);

    private async Task<T> PostAsync<T>(string uri, object body, string emptyResponseMessage, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, uri)
        {
            Content = JsonContent.Create(body, body.GetType(), options: JsonOptions)
        };
        using var response = await ExecuteHttpAsync(
            () => httpClient.SendAsync(request, cancellationToken),
            cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken)
            ?? throw new ApiException(emptyResponseMessage, 502);
    }

    private async Task<IReadOnlyList<T>> GetListAsync<T>(string uri, CancellationToken cancellationToken)
        => await GetAsync<List<T>>(uri, cancellationToken);

    private async Task<T> GetAsync<T>(string uri, CancellationToken cancellationToken)
    {
        using var response = await ExecuteHttpAsync(
            () => httpClient.GetAsync(uri, cancellationToken), cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken) ?? throw new ApiException("La API devolvió una respuesta vacía.", 502);
    }

    private async Task SendAsync(HttpMethod method, string uri, object? body, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, uri);
        if (body is not null) request.Content = JsonContent.Create(body, body.GetType(), options: JsonOptions);
        using var response = await ExecuteHttpAsync(
            () => httpClient.SendAsync(request, cancellationToken), cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    private static async Task<HttpResponseMessage> ExecuteHttpAsync(
        Func<Task<HttpResponseMessage>> operation,
        CancellationToken cancellationToken)
    {
        try
        {
            return await operation();
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new ApiException("La API tardó demasiado en responder.", 504);
        }
        catch (HttpRequestException)
        {
            throw new ApiException("No fue posible conectarse con la API. Verifique que BikeStore.Api esté iniciada.", 503);
        }
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode) return;
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var message = "No fue posible completar la solicitud.";
        try
        {
            using var json = JsonDocument.Parse(content);
            message = json.RootElement.TryGetProperty("detail", out var detail) ? detail.GetString() ?? message : message;
            if (json.RootElement.TryGetProperty("errors", out var errors))
                message = string.Join(" ", errors.EnumerateObject().SelectMany(x => x.Value.EnumerateArray()).Select(x => x.GetString()));
        }
        catch (JsonException) { if (!string.IsNullOrWhiteSpace(content)) message = content; }
        throw new ApiException(message, (int)response.StatusCode);
    }

    private static string FormatDate(DateTime? value) => value?.ToString("yyyy-MM-dd") ?? string.Empty;
}
