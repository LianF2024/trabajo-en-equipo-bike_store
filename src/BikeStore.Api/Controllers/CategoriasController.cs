using BikeStore.Application.DTOs;
using BikeStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BikeStore.Api.Controllers;

[ApiController]
[Route("api/categorias")]
public sealed class CategoriasController(ICategoryService service) : BikeStoreApiControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<CategoryDto>>(StatusCodes.Status200OK)]
    public Task<ActionResult<IReadOnlyList<CategoryDto>>> GetAll([FromQuery] string? buscar, [FromQuery] bool incluirInactivas = false, CancellationToken cancellationToken = default)
        => ExecuteAsync(
            () => service.GetAllAsync(buscar, incluirInactivas, cancellationToken),
            result => Ok(result));

    [HttpGet("{id:int}")]
    [ProducesResponseType<CategoryDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public Task<ActionResult<CategoryDto>> GetById(int id, CancellationToken cancellationToken)
        => ExecuteAsync(
            () => service.GetByIdAsync(id, cancellationToken),
            result => Ok(result));

    [HttpPost]
    [ProducesResponseType<CategoryDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public Task<ActionResult<CategoryDto>> Create([FromBody] SaveCategoryRequest request, CancellationToken cancellationToken)
        => ExecuteAsync(
            () => service.CreateAsync(request, cancellationToken),
            created => CreatedAtAction(nameof(GetById), new { id = created.Id }, created));

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public Task<IActionResult> Update(int id, [FromBody] SaveCategoryRequest request, CancellationToken cancellationToken)
        => ExecuteAsync(
            () => service.UpdateAsync(id, request, cancellationToken),
            () => NoContent());

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        => ExecuteAsync(
            () => service.DeleteAsync(id, cancellationToken),
            () => NoContent());
}
