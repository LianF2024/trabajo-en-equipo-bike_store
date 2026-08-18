using BikeStore.Application.DTOs;
using BikeStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BikeStore.Api.Controllers;

[ApiController]
[Route("api/bicicletas")]
public sealed class BicicletasController(IBicycleService service) : BikeStoreApiControllerBase
{
    [HttpGet]
    public Task<ActionResult<IReadOnlyList<BicycleDto>>> GetAll([FromQuery] BicycleFilter filter, CancellationToken cancellationToken = default)
        => ExecuteAsync(
            () => service.GetAllAsync(filter, cancellationToken),
            result => Ok(result));

    [HttpGet("stock-bajo")]
    public Task<ActionResult<IReadOnlyList<BicycleDto>>> GetLowStock([FromQuery] int limite = 5, CancellationToken cancellationToken = default)
        => ExecuteAsync(
            () => service.GetAllAsync(new BicycleFilter { LowStock = true, LowStockThreshold = limite }, cancellationToken),
            result => Ok(result));

    [HttpGet("agotadas")]
    public Task<ActionResult<IReadOnlyList<BicycleDto>>> GetOutOfStock(CancellationToken cancellationToken)
        => ExecuteAsync(
            () => service.GetAllAsync(new BicycleFilter { OutOfStock = true }, cancellationToken),
            result => Ok(result));

    [HttpGet("{id:int}")]
    public Task<ActionResult<BicycleDto>> GetById(int id, CancellationToken cancellationToken)
        => ExecuteAsync(
            () => service.GetByIdAsync(id, cancellationToken),
            result => Ok(result));

    [HttpPost]
    public Task<ActionResult<BicycleDto>> Create([FromBody] SaveBicycleRequest request, CancellationToken cancellationToken)
        => ExecuteAsync(
            () => service.CreateAsync(request, cancellationToken),
            created => CreatedAtAction(nameof(GetById), new { id = created.Id }, created));

    [HttpPut("{id:int}")]
    public Task<IActionResult> Update(int id, [FromBody] SaveBicycleRequest request, CancellationToken cancellationToken)
        => ExecuteAsync(
            () => service.UpdateAsync(id, request, cancellationToken),
            () => NoContent());

    [HttpDelete("{id:int}")]
    public Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        => ExecuteAsync(
            () => service.DeleteAsync(id, cancellationToken),
            () => NoContent());
}
