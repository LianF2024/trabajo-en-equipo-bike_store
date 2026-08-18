using BikeStore.Application.Common;
using BikeStore.Application.Contracts;
using BikeStore.Application.DTOs;
using BikeStore.Application.Interfaces;
using BikeStore.Domain.Entities;
using BikeStore.Domain.Enums;

namespace BikeStore.Application.Services;

public sealed class SaleService(IStoreRepository repository, BusinessOptions options) : ISaleService
{
    public async Task<IReadOnlyList<SaleDto>> GetAllAsync(int? customerId, DateTime? from, DateTime? to, CancellationToken cancellationToken)
        => (await repository.GetSalesAsync(customerId, from, to, cancellationToken)).Select(Map).ToList();

    public async Task<SaleDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        => Map(await repository.GetSaleAsync(id, cancellationToken) ?? throw new NotFoundException("Venta no encontrada."));

    public async Task<SaleDto> CreateAsync(CreateSaleRequest request, CancellationToken cancellationToken)
    {
        if (request.CustomerId <= 0) throw new BusinessException("Seleccione un cliente válido.");
        if (request.Items is null || request.Items.Count == 0) throw new BusinessException("La venta debe incluir al menos una bicicleta.");
        var customer = await repository.GetCustomerAsync(request.CustomerId, cancellationToken);
        if (customer is null) throw new BusinessException("El cliente seleccionado no existe.");

        var grouped = request.Items.GroupBy(x => x.BicycleId)
            .Select(g => new CreateSaleItemRequest { BicycleId = g.Key, Quantity = g.Sum(x => x.Quantity) })
            .ToList();

        await using var transaction = await repository.BeginTransactionAsync(cancellationToken);
        try
        {
            var bicycles = await repository.GetBicyclesByIdsAsync(grouped.Select(x => x.BicycleId).ToArray(), cancellationToken);
            if (bicycles.Count != grouped.Count) throw new BusinessException("Una o más bicicletas no existen.");

            var sale = new Sale { CustomerId = customer.Id, Date = DateTime.Now };
            foreach (var item in grouped)
            {
                var bicycle = bicycles.Single(x => x.Id == item.BicycleId);
                if (item.Quantity <= 0) throw new BusinessException("La cantidad debe ser mayor que cero.");
                if (bicycle.Stock < item.Quantity) throw new BusinessException($"Stock insuficiente para '{bicycle.Brand} {bicycle.Model}'. Disponible: {bicycle.Stock}.");

                var lineSubtotal = decimal.Round(bicycle.Price * item.Quantity, 2);
                sale.Details.Add(new SaleDetail
                {
                    BicycleId = bicycle.Id,
                    Quantity = item.Quantity,
                    UnitPrice = bicycle.Price,
                    Subtotal = lineSubtotal
                });
                bicycle.Stock -= item.Quantity;
                bicycle.Status = bicycle.Stock == 0 ? BicycleStatus.Agotado : bicycle.Stock <= options.LowStockThreshold ? BicycleStatus.BajoStock : BicycleStatus.Disponible;
            }

            sale.Subtotal = sale.Details.Sum(x => x.Subtotal);
            sale.Vat = decimal.Round(sale.Subtotal * options.VatRate, 2, MidpointRounding.AwayFromZero);
            sale.Total = sale.Subtotal + sale.Vat;
            repository.AddSale(sale);
            await repository.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return await GetByIdAsync(sale.Id, cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    private static SaleDto Map(Sale sale) => new(
        sale.Id, sale.Date, sale.CustomerId, $"{sale.Customer.FirstNames} {sale.Customer.LastNames}",
        sale.Subtotal, sale.Vat, sale.Total,
        sale.Details.Select(x => new SaleDetailDto(x.Id, x.BicycleId, $"{x.Bicycle.Brand} {x.Bicycle.Model}", x.Quantity, x.UnitPrice, x.Subtotal)).ToList());
}
