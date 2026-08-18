using BikeStore.Web.ModelBinding;
using BikeStore.Web.Models;
using BikeStore.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BikeStore.Web.Controllers;

public sealed class VentasController(IBikeStoreApiClient api) : Controller
{
    public async Task<IActionResult> Index(int? clienteId, DateTime? desde, DateTime? hasta, CancellationToken cancellationToken)
    {
        ViewBag.ClienteId = new SelectList(
            await api.GetCustomersAsync(cancellationToken: cancellationToken),
            "Id",
            "FullName",
            clienteId);
        ViewBag.Desde = desde?.ToString("yyyy-MM-dd");
        ViewBag.Hasta = hasta?.ToString("yyyy-MM-dd");
        return View(await api.GetSalesAsync(clienteId, desde, hasta, cancellationToken));
    }

    public async Task<IActionResult> Detalle(int id, CancellationToken cancellationToken)
        => View(await api.GetSaleAsync(id, cancellationToken));

    public async Task<IActionResult> Crear(CancellationToken cancellationToken)
    {
        await LoadOptionsAsync(null, cancellationToken);
        return View(new VentaFormVm());
    }

    [HttpPost, ActionName("Crear"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearPost(CancellationToken cancellationToken)
    {
        var model = await ReadFormAsync(cancellationToken);
        if (!ModelState.IsValid)
        {
            AddValidationSummary();
            await LoadOptionsAsync(model.CustomerId, cancellationToken);
            return View(model);
        }

        try
        {
            var sale = await api.CreateSaleAsync(model, cancellationToken);
            TempData["Success"] = $"Venta #{sale.Id} registrada correctamente. Total: {sale.Total:C2}.";
            return RedirectToAction(nameof(Detalle), new { id = sale.Id });
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty, $"No se registró la venta: {ex.Message}");
            await LoadOptionsAsync(model.CustomerId, cancellationToken);
            return View(model);
        }
    }

    private async Task<VentaFormVm> ReadFormAsync(CancellationToken cancellationToken)
    {
        var form = await Request.ReadFormAsync(cancellationToken);
        var model = new VentaFormVm { Items = [] };
        ModelState.Clear();

        if (FormValueParser.TryInt(form, "CustomerId", out var customerId))
            model.CustomerId = customerId;
        else
            ModelState.AddModelError(nameof(model.CustomerId), "Seleccione un cliente válido.");

        for (var index = 0; index < 1000; index++)
        {
            var bicycleKey = $"Items[{index}].BicycleId";
            var quantityKey = $"Items[{index}].Quantity";
            if (!form.ContainsKey(bicycleKey) && !form.ContainsKey(quantityKey)) break;

            var item = new VentaLineaFormVm();
            if (FormValueParser.TryInt(form, bicycleKey, out var bicycleId))
                item.BicycleId = bicycleId;
            else
                ModelState.AddModelError(bicycleKey, "Seleccione una bicicleta válida.");

            if (FormValueParser.TryInt(form, quantityKey, out var quantity))
                item.Quantity = quantity;
            else
                ModelState.AddModelError(quantityKey, "Ingrese una cantidad válida.");

            model.Items.Add(item);
        }

        if (model.Items.Count == 0)
            ModelState.AddModelError(nameof(model.Items), "Agregue al menos una bicicleta.");

        TryValidateModel(model);
        return model;
    }

    private void AddValidationSummary()
    {
        var errors = ModelState
            .Where(item => item.Value?.Errors.Count > 0)
            .SelectMany(item => item.Value!.Errors)
            .Select(error => error.ErrorMessage)
            .Where(message => !string.IsNullOrWhiteSpace(message))
            .Distinct()
            .ToList();

        var detail = errors.Count == 0 ? "Revise los campos señalados." : string.Join(" ", errors);
        ModelState.AddModelError(string.Empty, $"No se registró la venta. {detail}");
    }

    private async Task LoadOptionsAsync(int? customerId, CancellationToken cancellationToken)
    {
        ViewBag.Clientes = new SelectList(
            await api.GetCustomersAsync(cancellationToken: cancellationToken),
            "Id",
            "FullName",
            customerId);
        ViewBag.Bicicletas = await api.GetBicyclesAsync(cancellationToken: cancellationToken);
    }
}
