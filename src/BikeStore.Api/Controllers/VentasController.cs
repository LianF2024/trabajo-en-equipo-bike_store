using BikeStore.Application.DTOs;
using BikeStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BikeStore.Api.Controllers;

[ApiController]
[Route("api/ventas")]
public sealed class VentasController(ISaleService service) : BikeStoreApiControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<SaleDto>>(StatusCodes.Status200OK)]
    public Task<ActionResult<IReadOnlyList<SaleDto>>> GetAll([FromQuery] int? clienteId, [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, CancellationToken cancellationToken = default)
        => ExecuteAsync(
            () => service.GetAllAsync(clienteId, desde, hasta, cancellationToken),
            result => Ok(result));

    [HttpGet("cliente/{clienteId:int}")]
    public Task<ActionResult<IReadOnlyList<SaleDto>>> GetByCustomer(int clienteId, CancellationToken cancellationToken)
        => ExecuteAsync(
            () => service.GetAllAsync(clienteId, null, null, cancellationToken),
            result => Ok(result));

    [HttpGet("{id:int}")]
    [ProducesResponseType<SaleDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public Task<ActionResult<SaleDto>> GetById(int id, CancellationToken cancellationToken)
        => ExecuteAsync(
            () => service.GetByIdAsync(id, cancellationToken),
            result => Ok(result));

    [HttpPost]
    [ProducesResponseType<SaleDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public Task<ActionResult<SaleDto>> Create([FromBody] CreateSaleRequest request, CancellationToken cancellationToken)
        => ExecuteAsync(
            () => service.CreateAsync(request, cancellationToken),
            created => CreatedAtAction(nameof(GetById), new { id = created.Id }, created));
}
