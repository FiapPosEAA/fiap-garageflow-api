using GarageFlowService.Application.DTOs;
using GarageFlowService.Domain.Interfaces;
using MediatR;

namespace GarageFlowService.Application.UseCases.WorkOrders;

public record GetAllWorkOrdersQuery : IRequest<IEnumerable<WorkOrderDto>>;

public class GetAllWorkOrdersHandler : IRequestHandler<GetAllWorkOrdersQuery, IEnumerable<WorkOrderDto>>
{
    private readonly IWorkOrderRepository _workOrderRepository;

    public GetAllWorkOrdersHandler(IWorkOrderRepository workOrderRepository)
    {
        _workOrderRepository = workOrderRepository;
    }

    public async Task<IEnumerable<WorkOrderDto>> Handle(GetAllWorkOrdersQuery request, CancellationToken cancellationToken)
    {
        var workOrders = await _workOrderRepository.GetAllWithDetailsAsync(cancellationToken);
        return workOrders
            .Where(workOrder => workOrder.Status is not Domain.Enums.WorkOrderStatus.Finished and not Domain.Enums.WorkOrderStatus.Delivered)
            .OrderByDescending(GetStatusPriority)
            .ThenBy(workOrder => workOrder.CreatedAt)
            .Select(CreateWorkOrderHandler.MapToDto);
    }

    private static int GetStatusPriority(Domain.Entities.WorkOrder workOrder) => workOrder.Status switch
    {
        Domain.Enums.WorkOrderStatus.InProgress => 4,
        Domain.Enums.WorkOrderStatus.WaitingApproval => 3,
        Domain.Enums.WorkOrderStatus.Diagnosis => 2,
        Domain.Enums.WorkOrderStatus.Received => 1,
        Domain.Enums.WorkOrderStatus.Canceled => 0,
        _ => 0
    };
}

