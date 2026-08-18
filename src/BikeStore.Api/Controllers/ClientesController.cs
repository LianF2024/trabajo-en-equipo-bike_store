using BikeStore.Application.DTOs;
using BikeStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BikeStore.Api.Controllers;

[ApiController]
[Route("api/clientes")]
public sealed class ClientesController(ICustomerService service) : BikeStoreApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CustomerDto>), StatusCodes.Status200OK)]
    public Task<ActionResult<IReadOnlyList<CustomerDto>>> GetAll([FromQuery] string? cedula, [FromQuery] string? apellido, CancellationToken cancellationToken = default)
        => ExecuteAsync(
            () => service.GetAllAsync(cedula, apellido, cancellationToken),
            result => Ok(result));

    [HttpGet("{id:int:min(1)}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<ActionResult<CustomerDto>> GetById(int id, CancellationToken cancellationToken)
        => ExecuteAsync(
            () => service.GetByIdAsync(id, cancellationToken),
            result => Ok(result));

    [HttpPost]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public Task<ActionResult<CustomerDto>> Create([FromBody] SaveCustomerRequest request, CancellationToken cancellationToken)
        => ExecuteAsync(
            () => service.CreateAsync(request, cancellationToken),
            created => CreatedAtAction(nameof(GetById), new { id = created.Id }, created));

    [HttpPut("{id:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public Task<IActionResult> Update(int id, [FromBody] SaveCustomerRequest request, CancellationToken cancellationToken)
        => ExecuteAsync(
            () => service.UpdateAsync(id, request, cancellationToken),
            () => NoContent());

    [HttpDelete("{id:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        => ExecuteAsync(
            () => service.DeleteAsync(id, cancellationToken),
            () => NoContent());
}
