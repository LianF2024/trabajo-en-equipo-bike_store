using BikeStore.Web.ModelBinding;
using BikeStore.Web.Models;
using BikeStore.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace BikeStore.Web.Controllers;

public sealed class ClientesController(IBikeStoreApiClient api) : Controller
{
    public async Task<IActionResult> Index(string? cedula, string? apellido, CancellationToken cancellationToken)
    {
        ViewBag.Cedula = cedula;
        ViewBag.Apellido = apellido;
        return View(await api.GetCustomersAsync(cedula, apellido, cancellationToken));
    }

    public IActionResult Crear() => View("Formulario", new ClienteFormVm());

    [HttpPost, ActionName("Crear"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearPost(CancellationToken cancellationToken)
    {
        var model = await ReadFormAsync(cancellationToken, includeId: false);
        if (!ModelState.IsValid) return View("Formulario", model);

        try
        {
            var created = await api.CreateCustomerAsync(model, cancellationToken);
            TempData["Success"] = $"Cliente #{created.Id} registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty, $"No se guardó el cliente: {ex.Message}");
            return View("Formulario", model);
        }
    }

    public async Task<IActionResult> Editar(int id, CancellationToken cancellationToken)
    {
        var item = await api.GetCustomerAsync(id, cancellationToken);
        return View("Formulario", new ClienteFormVm
        {
            Id = item.Id,
            Identification = item.Identification,
            FirstNames = item.FirstNames,
            LastNames = item.LastNames,
            Phone = item.Phone,
            Email = item.Email
        });
    }

    [HttpPost, ActionName("Editar"), ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarPost(CancellationToken cancellationToken)
    {
        var model = await ReadFormAsync(cancellationToken, includeId: true);
        if (!ModelState.IsValid) return View("Formulario", model);

        try
        {
            await api.UpdateCustomerAsync(model, cancellationToken);
            TempData["Success"] = $"Cliente #{model.Id} actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty, $"No se actualizó el cliente: {ex.Message}");
            return View("Formulario", model);
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            await api.DeleteCustomerAsync(id, cancellationToken);
            TempData["Success"] = $"Cliente #{id} eliminado mediante DELETE.";
        }
        catch (ApiException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task<ClienteFormVm> ReadFormAsync(CancellationToken cancellationToken, bool includeId)
    {
        var form = await Request.ReadFormAsync(cancellationToken);
        var model = new ClienteFormVm
        {
            Identification = FormValueParser.Text(form, "Identification"),
            FirstNames = FormValueParser.Text(form, "FirstNames"),
            LastNames = FormValueParser.Text(form, "LastNames"),
            Phone = NullIfEmpty(FormValueParser.Text(form, "Phone")),
            Email = NullIfEmpty(FormValueParser.Text(form, "Email"))
        };

        ModelState.Clear();
        if (includeId)
        {
            if (FormValueParser.TryInt(form, "Id", out var id) && id > 0)
                model.Id = id;
            else
                ModelState.AddModelError(nameof(model.Id), "El identificador del cliente no es válido.");
        }

        TryValidateModel(model);
        return model;
    }

    private static string? NullIfEmpty(string value)
        => string.IsNullOrWhiteSpace(value) ? null : value;
}
