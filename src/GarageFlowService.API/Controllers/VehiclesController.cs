using System.Diagnostics.CodeAnalysis;
using GarageFlowService.Application.DTOs;
using GarageFlowService.Application.UseCases.Vehicles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GarageFlowService.API.Controllers;

[ExcludeFromCodeCoverage]
[ApiController]
[Route("api/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly IMediator _mediator;

    public VehiclesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("customer/{customerId:guid}")]
    public async Task<ActionResult<IEnumerable<VehicleDto>>> GetByCustomer(Guid customerId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetVehiclesByCustomerQuery(customerId), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<VehicleDto>> Create([FromBody] CreateVehicleRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateVehicleCommand(request.CustomerId, request.Brand, request.Model, request.Year, request.LicensePlate, request.Color), cancellationToken);
        return CreatedAtAction(nameof(GetByCustomer), new { customerId = result.CustomerId }, result);
    }
}

