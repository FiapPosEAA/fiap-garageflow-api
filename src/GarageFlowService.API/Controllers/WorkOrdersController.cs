using System.Diagnostics.CodeAnalysis;
using GarageFlowService.Application.DTOs;
using GarageFlowService.Application.UseCases.WorkOrders;
using GarageFlowService.Domain.Enums;
using GarageFlowService.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GarageFlowService.API.Controllers;

[ExcludeFromCodeCoverage]
[ApiController]
[Route("api/[controller]")]
public class WorkOrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public WorkOrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkOrderDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllWorkOrdersQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WorkOrderDetailDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetWorkOrderByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("{id:guid}/status")]
    public async Task<ActionResult<WorkOrderStatusDto>> GetStatus(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetWorkOrderStatusQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("{id:guid}/budget")]
    public async Task<ActionResult<BudgetDto>> GetBudget(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetWorkOrderBudgetQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<WorkOrderDto>> Create([FromBody] CreateWorkOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateWorkOrderCommand(request.CustomerId, request.VehicleId, request.Description), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize]
    public async Task<ActionResult<WorkOrderDto>> UpdateStatus(Guid id, [FromBody] UpdateWorkOrderStatusRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(new UpdateWorkOrderStatusCommand(id, (WorkOrderStatus)request.NewStatus), cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/status/notification")]
    [AllowAnonymous]
    public async Task<ActionResult<WorkOrderDto>> NotifyStatus(Guid id, [FromBody] UpdateWorkOrderStatusRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(new UpdateWorkOrderStatusCommand(id, (WorkOrderStatus)request.NewStatus), cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/budget/decision")]
    [AllowAnonymous]
    public async Task<ActionResult<WorkOrderDto>> ResolveBudget(Guid id, [FromBody] BudgetDecisionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(new ApproveBudgetCommand(id, request.Approved), cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/services")]
    [Authorize]
    public async Task<ActionResult<WorkOrderDto>> AddService(Guid id, [FromBody] AddServiceToWorkOrderRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(new AddServiceToWorkOrderCommand(id, request.ServiceId, request.Quantity), cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/parts")]
    [Authorize]
    public async Task<ActionResult<WorkOrderDto>> AddPart(Guid id, [FromBody] AddPartToWorkOrderRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(new AddPartToWorkOrderCommand(id, request.PartId, request.Quantity), cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

