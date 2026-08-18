using BikeStore.Application.DTOs;
using BikeStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BikeStore.Api.Controllers;

[ApiController]
[Route("api/clientes")]
public sealed class ClientesController(ICustomerService service) : BikeStoreApiControllerBase
{
    [HttpGet]
    public Task<ActionResult<IReadOnlyList<CustomerDto>>> GetAll([FromQuery] string? cedula, [FromQuery] string? apellido, CancellationToken cancellationToken = default)
        => ExecuteAsync(
            () => service.GetAllAsync(cedula, apellido, cancellationToken),
            result => Ok(result));

    [HttpGet("{id:int}")]
    public Task<ActionResult<CustomerDto>> GetById(int id, CancellationToken cancellationToken)
        => ExecuteAsync(
            () => service.GetByIdAsync(id, cancellationToken),
            result => Ok(result));

    [HttpPost]
    public Task<ActionResult<CustomerDto>> Create([FromBody] SaveCustomerRequest request, CancellationToken cancellationToken)
        => ExecuteAsync(
            () => service.CreateAsync(request, cancellationToken),
            created => CreatedAtAction(nameof(GetById), new { id = created.Id }, created));

    [HttpPut("{id:int}")]
    public Task<IActionResult> Update(int id, [FromBody] SaveCustomerRequest request, CancellationToken cancellationToken)
        => ExecuteAsync(
            () => service.UpdateAsync(id, request, cancellationToken),
            () => NoContent());

    [HttpDelete("{id:int}")]
    public Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        => ExecuteAsync(
            () => service.DeleteAsync(id, cancellationToken),
            () => NoContent());
}
