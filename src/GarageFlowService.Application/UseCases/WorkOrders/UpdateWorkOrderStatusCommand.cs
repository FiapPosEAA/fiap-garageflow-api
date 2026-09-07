using GarageFlowService.Application.DTOs;
using GarageFlowService.Domain.Enums;
using GarageFlowService.Domain.Interfaces;
using GarageFlowService.Application.Interfaces;
using MediatR;

namespace GarageFlowService.Application.UseCases.WorkOrders;

public record UpdateWorkOrderStatusCommand(Guid Id, WorkOrderStatus NewStatus) : IRequest<WorkOrderDto?>;

public class UpdateWorkOrderStatusHandler : IRequestHandler<UpdateWorkOrderStatusCommand, WorkOrderDto?>
{
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateWorkOrderStatusHandler(IWorkOrderRepository workOrderRepository, IUnitOfWork unitOfWork)
    {
        _workOrderRepository = workOrderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<WorkOrderDto?> Handle(UpdateWorkOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await _workOrderRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (workOrder is null) return null;

        workOrder.UpdateStatus(request.NewStatus);
        _workOrderRepository.Update(workOrder);
        await _unitOfWork.CommitAsync(cancellationToken);
        return CreateWorkOrderHandler.MapToDto(workOrder);
    }
}

