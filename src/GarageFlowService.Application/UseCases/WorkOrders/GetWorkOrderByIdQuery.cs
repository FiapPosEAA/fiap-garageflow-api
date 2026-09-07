using GarageFlowService.Application.DTOs;
using GarageFlowService.Domain.Interfaces;
using MediatR;

namespace GarageFlowService.Application.UseCases.WorkOrders;

public record GetWorkOrderByIdQuery(Guid Id) : IRequest<WorkOrderDetailDto?>;

public class GetWorkOrderByIdHandler : IRequestHandler<GetWorkOrderByIdQuery, WorkOrderDetailDto?>
{
    private readonly IWorkOrderRepository _workOrderRepository;

    public GetWorkOrderByIdHandler(IWorkOrderRepository workOrderRepository)
    {
        _workOrderRepository = workOrderRepository;
    }

    public async Task<WorkOrderDetailDto?> Handle(GetWorkOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var wo = await _workOrderRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (wo is null) return null;

        return new WorkOrderDetailDto(
            wo.Id,
            wo.OrderNumber,
            wo.CustomerId,
            wo.Customer?.Name ?? string.Empty,
            wo.VehicleId,
            wo.Vehicle is not null ? $"{wo.Vehicle.Brand} {wo.Vehicle.Model} ({wo.Vehicle.Year})" : string.Empty,
            wo.Status,
            wo.Status.ToString(),
            wo.Description,
            wo.DiagnosisNotes,
            wo.TotalAmount,
            wo.CreatedAt,
            wo.UpdatedAt,
            wo.WorkOrderServices.Select(s => new WorkOrderServiceDto(s.ServiceId, s.Service?.Name ?? string.Empty, s.Quantity, s.UnitPrice, s.Subtotal)).ToList(),
            wo.WorkOrderParts.Select(p => new WorkOrderPartDto(p.PartId, p.Part?.Name ?? string.Empty, p.Quantity, p.UnitPrice, p.Subtotal)).ToList());
    }
}

