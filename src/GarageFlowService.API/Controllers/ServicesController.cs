using System.Diagnostics.CodeAnalysis;
using GarageFlowService.Application.DTOs;
using GarageFlowService.Application.UseCases.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GarageFlowService.API.Controllers;

[ExcludeFromCodeCoverage]
[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ServicesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllServicesQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ServiceDto>> Create([FromBody] CreateServiceRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateServiceCommand(request.Name, request.Description, request.Price, request.EstimatedHours), cancellationToken);
        return CreatedAtAction(nameof(GetAll), result);
    }
}

