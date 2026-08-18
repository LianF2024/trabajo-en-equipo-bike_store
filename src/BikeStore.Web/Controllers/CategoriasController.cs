using BikeStore.Web.ModelBinding;
using BikeStore.Web.Models;
using BikeStore.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace BikeStore.Web.Controllers;

public sealed class CategoriasController(IBikeStoreApiClient api) : Controller
{
    public async Task<IActionResult> Index(string? buscar, bool incluirInactivas, CancellationToken cancellationToken)
    {
        ViewBag.Buscar = buscar;
        ViewBag.IncluirInactivas = incluirInactivas;
        return View(await api.GetCategoriesAsync(buscar, incluirInactivas, cancellationToken));
    }

    public IActionResult Crear() => View("Formulario", new CategoriaFormVm());

    [HttpPost, ActionName("Crear"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearPost(CancellationToken cancellationToken)
    {
        var model = await ReadFormAsync(cancellationToken, includeId: false);
        if (!ModelState.IsValid) return View("Formulario", model);

        try
        {
            var created = await api.CreateCategoryAsync(model, cancellationToken);
            TempData["Success"] = $"Categoría #{created.Id} registrada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty, $"No se guardó la categoría: {ex.Message}");
            return View("Formulario", model);
        }
    }

    public async Task<IActionResult> Editar(int id, CancellationToken cancellationToken)
    {
        var item = await api.GetCategoryAsync(id, cancellationToken);
        return View("Formulario", new CategoriaFormVm
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            Active = item.Active
        });
    }

    [HttpPost, ActionName("Editar"), ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarPost(CancellationToken cancellationToken)
    {
        var model = await ReadFormAsync(cancellationToken, includeId: true);
        if (!ModelState.IsValid) return View("Formulario", model);

        try
        {
            await api.UpdateCategoryAsync(model, cancellationToken);
            TempData["Success"] = $"Categoría #{model.Id} actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty, $"No se actualizó la categoría: {ex.Message}");
            return View("Formulario", model);
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            await api.DeleteCategoryAsync(id, cancellationToken);
            TempData["Success"] = $"Categoría #{id} desactivada mediante DELETE.";
        }
        catch (ApiException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task<CategoriaFormVm> ReadFormAsync(CancellationToken cancellationToken, bool includeId)
    {
        var form = await Request.ReadFormAsync(cancellationToken);
        var model = new CategoriaFormVm
        {
            Name = FormValueParser.Text(form, "Name"),
            Description = NullIfEmpty(FormValueParser.Text(form, "Description")),
            Active = FormValueParser.IsTrue(form, "Active")
        };

        ModelState.Clear();
        if (includeId)
        {
            if (FormValueParser.TryInt(form, "Id", out var id) && id > 0)
                model.Id = id;
            else
                ModelState.AddModelError(nameof(model.Id), "El identificador de la categoría no es válido.");
        }

        TryValidateModel(model);
        return model;
    }

    private static string? NullIfEmpty(string value)
        => string.IsNullOrWhiteSpace(value) ? null : value;
}
