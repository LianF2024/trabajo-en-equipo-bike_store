using BikeStore.Web.ModelBinding;
using BikeStore.Web.Models;
using BikeStore.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BikeStore.Web.Controllers;

public sealed class BicicletasController(IBikeStoreApiClient api) : Controller
{
    public async Task<IActionResult> Index(string? nombre, int? categoriaId, string? marca, bool stockBajo, bool agotadas, CancellationToken cancellationToken)
    {
        await LoadCategoriesAsync(categoriaId, cancellationToken);
        ViewBag.Nombre = nombre;
        ViewBag.Marca = marca;
        ViewBag.StockBajo = stockBajo;
        ViewBag.Agotadas = agotadas;
        return View(await api.GetBicyclesAsync(nombre, categoriaId, marca, stockBajo, agotadas, cancellationToken));
    }

    public async Task<IActionResult> Crear(CancellationToken cancellationToken)
    {
        await LoadCategoriesAsync(null, cancellationToken);
        return View("Formulario", new BicicletaFormVm());
    }

    [HttpPost, ActionName("Crear"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearPost(CancellationToken cancellationToken)
    {
        var model = await ReadFormAsync(cancellationToken, includeId: false);
        if (!ModelState.IsValid)
            return await InvalidFormAsync(model, "No se guardó la bicicleta.", cancellationToken);

        try
        {
            var created = await api.CreateBicycleAsync(model, cancellationToken);
            TempData["Success"] = $"Bicicleta #{created.Id} registrada correctamente mediante POST.";
            return RedirectToAction(nameof(Index));
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty, $"No se guardó la bicicleta: {ex.Message}");
            await LoadCategoriesAsync(model.CategoryId, cancellationToken);
            return View("Formulario", model);
        }
    }

    public async Task<IActionResult> Editar(int id, CancellationToken cancellationToken)
    {
        var item = await api.GetBicycleAsync(id, cancellationToken);
        await LoadCategoriesAsync(item.CategoryId, cancellationToken);
        return View("Formulario", new BicicletaFormVm
        {
            Id = item.Id,
            CategoryId = item.CategoryId,
            Brand = item.Brand,
            Model = item.Model,
            Price = item.Price,
            Stock = item.Stock
        });
    }

    [HttpPost, ActionName("Editar"), ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarPost(CancellationToken cancellationToken)
    {
        var model = await ReadFormAsync(cancellationToken, includeId: true);
        if (!ModelState.IsValid)
            return await InvalidFormAsync(model, "No se actualizó la bicicleta.", cancellationToken);

        try
        {
            await api.UpdateBicycleAsync(model, cancellationToken);
            TempData["Success"] = $"Bicicleta #{model.Id} actualizada correctamente mediante PUT.";
            return RedirectToAction(nameof(Index));
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty, $"No se actualizó la bicicleta: {ex.Message}");
            await LoadCategoriesAsync(model.CategoryId, cancellationToken);
            return View("Formulario", model);
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            await api.DeleteBicycleAsync(id, cancellationToken);
            TempData["Success"] = $"Bicicleta #{id} eliminada mediante DELETE.";
        }
        catch (ApiException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task<BicicletaFormVm> ReadFormAsync(CancellationToken cancellationToken, bool includeId)
    {
        var form = await Request.ReadFormAsync(cancellationToken);
        var model = new BicicletaFormVm
        {
            Brand = FormValueParser.Text(form, "Brand"),
            Model = FormValueParser.Text(form, "Model")
        };

        ModelState.Clear();

        if (includeId)
        {
            if (FormValueParser.TryInt(form, "Id", out var id) && id > 0)
                model.Id = id;
            else
                ModelState.AddModelError(nameof(model.Id), "El identificador de la bicicleta no es válido.");
        }

        if (FormValueParser.TryInt(form, "CategoryId", out var categoryId))
            model.CategoryId = categoryId;
        else
            ModelState.AddModelError(nameof(model.CategoryId), "Seleccione una categoría válida.");

        var validPrice = FormValueParser.TryDecimal(form, "Price", out var price);
        if (validPrice)
            model.Price = price;
        else
            ModelState.AddModelError(nameof(model.Price), "Ingrese un precio válido, por ejemplo 950,50.");

        if (FormValueParser.TryInt(form, "Stock", out var stock))
            model.Stock = stock;
        else
            ModelState.AddModelError(nameof(model.Stock), "Ingrese una cantidad de stock válida.");

        TryValidateModel(model);
        if (validPrice && (model.Price < 0.01m || model.Price > 9999999.99m))
            ModelState.AddModelError(nameof(model.Price), "El precio debe estar entre 0,01 y 9 999 999,99.");

        return model;
    }

    private async Task<IActionResult> InvalidFormAsync(BicicletaFormVm model, string title, CancellationToken cancellationToken)
    {
        AddValidationSummary(title);
        await LoadCategoriesAsync(model.CategoryId, cancellationToken);
        return View("Formulario", model);
    }

    private void AddValidationSummary(string title)
    {
        var errors = ModelState
            .Where(item => item.Value?.Errors.Count > 0)
            .SelectMany(item => item.Value!.Errors)
            .Select(error => error.ErrorMessage)
            .Where(message => !string.IsNullOrWhiteSpace(message))
            .Distinct()
            .ToList();

        var detail = errors.Count == 0 ? "Revise los campos señalados." : string.Join(" ", errors);
        ModelState.AddModelError(string.Empty, $"{title} {detail}");
    }

    private async Task LoadCategoriesAsync(int? selected, CancellationToken cancellationToken)
        => ViewBag.Categorias = new SelectList(
            await api.GetCategoriesAsync(cancellationToken: cancellationToken),
            "Id",
            "Name",
            selected);
}
