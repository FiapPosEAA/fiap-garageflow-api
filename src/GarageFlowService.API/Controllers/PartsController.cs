using System.Diagnostics.CodeAnalysis;
using GarageFlowService.Application.DTOs;
using GarageFlowService.Application.UseCases.Parts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GarageFlowService.API.Controllers;

[ExcludeFromCodeCoverage]
[ApiController]
[Route("api/[controller]")]
public class PartsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PartsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PartDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllPartsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<PartDto>> Create([FromBody] CreatePartRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreatePartCommand(request.Name, request.Description, request.Price, request.StockQuantity), cancellationToken);
        return CreatedAtAction(nameof(GetAll), result);
    }

    [HttpPatch("{id:guid}/stock")]
    [Authorize]
    public async Task<ActionResult<PartDto>> UpdateStock(Guid id, [FromBody] UpdateStockRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdatePartStockCommand(id, request.QuantityChange), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }
}

