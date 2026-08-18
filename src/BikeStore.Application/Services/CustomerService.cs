using BikeStore.Application.Common;
using BikeStore.Application.Contracts;
using BikeStore.Application.DTOs;
using BikeStore.Application.Interfaces;
using BikeStore.Domain.Entities;

namespace BikeStore.Application.Services;

public sealed class CustomerService(IStoreRepository repository) : ICustomerService
{
    public async Task<IReadOnlyList<CustomerDto>> GetAllAsync(string? identification, string? lastName, CancellationToken cancellationToken)
        => (await repository.GetCustomersAsync(identification, lastName, cancellationToken)).Select(Map).ToList();

    public async Task<CustomerDto> GetByIdAsync(int id, CancellationToken cancellationToken) => Map(await FindAsync(id, cancellationToken));

    public async Task<CustomerDto> CreateAsync(SaveCustomerRequest request, CancellationToken cancellationToken)
    {
        Validate(request);
        var identification = request.Identification.Trim();
        if (await repository.CustomerIdentificationExistsAsync(identification, null, cancellationToken))
            throw new ConflictException("Ya existe un cliente con esa cédula/RUC.");

        var entity = new Customer();
        Apply(entity, request);
        repository.AddCustomer(entity);
        await repository.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task UpdateAsync(int id, SaveCustomerRequest request, CancellationToken cancellationToken)
    {
        Validate(request);
        var entity = await FindAsync(id, cancellationToken);
        var identification = request.Identification.Trim();
        if (await repository.CustomerIdentificationExistsAsync(identification, id, cancellationToken))
            throw new ConflictException("Ya existe un cliente con esa cédula/RUC.");
        Apply(entity, request);
        await repository.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await FindAsync(id, cancellationToken);
        repository.RemoveCustomer(entity);
        await repository.SaveChangesAsync(cancellationToken);
    }

    private async Task<Customer> FindAsync(int id, CancellationToken cancellationToken)
        => await repository.GetCustomerAsync(id, cancellationToken)
           ?? throw new NotFoundException("Cliente no encontrado.");

    private static void Apply(Customer entity, SaveCustomerRequest request)
    {
        entity.Identification = request.Identification.Trim();
        entity.FirstNames = request.FirstNames.Trim();
        entity.LastNames = request.LastNames.Trim();
        entity.Phone = Clean(request.Phone);
        entity.Email = Clean(request.Email)?.ToLowerInvariant();
    }

    private static CustomerDto Map(Customer entity) => new(entity.Id, entity.Identification, entity.FirstNames, entity.LastNames, entity.Phone, entity.Email);
    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static void Validate(SaveCustomerRequest request)
    {
        var identification = request.Identification?.Trim() ?? string.Empty;
        if (identification.Length is < 10 or > 13 || identification.Any(character => !char.IsDigit(character)))
            throw new BusinessException("La cédula/RUC debe tener entre 10 y 13 dígitos.");
        ValidateText(request.FirstNames, 2, 100, "Los nombres deben tener entre 2 y 100 caracteres.");
        ValidateText(request.LastNames, 2, 100, "Los apellidos deben tener entre 2 y 100 caracteres.");
        if (request.Phone?.Trim().Length > 20)
            throw new BusinessException("El teléfono no puede superar los 20 caracteres.");
        if (request.Email?.Trim().Length > 150)
            throw new BusinessException("El correo no puede superar los 150 caracteres.");
    }

    private static void ValidateText(string? value, int minimum, int maximum, string message)
    {
        var length = value?.Trim().Length ?? 0;
        if (length < minimum || length > maximum)
            throw new BusinessException(message);
    }
}
