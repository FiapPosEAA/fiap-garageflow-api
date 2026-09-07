using GarageFlowService.Application.DTOs;
using GarageFlowService.Domain.Interfaces;
using MediatR;

namespace GarageFlowService.Application.UseCases.WorkOrders;

public record GetWorkOrderStatusQuery(Guid Id) : IRequest<WorkOrderStatusDto?>;

public class GetWorkOrderStatusHandler : IRequestHandler<GetWorkOrderStatusQuery, WorkOrderStatusDto?>
{
    private readonly IWorkOrderRepository _workOrderRepository;

    public GetWorkOrderStatusHandler(IWorkOrderRepository workOrderRepository)
    {
        _workOrderRepository = workOrderRepository;
    }

    public async Task<WorkOrderStatusDto?> Handle(GetWorkOrderStatusQuery request, CancellationToken cancellationToken)
    {
        var workOrder = await _workOrderRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (workOrder is null)
            return null;

        return new WorkOrderStatusDto(
            workOrder.Id,
            workOrder.OrderNumber,
            workOrder.Status,
            workOrder.Status.ToString(),
            workOrder.UpdatedAt);
    }
}