using GarageFlowService.Application.DTOs;
using GarageFlowService.Domain.Interfaces;
using GarageFlowService.Application.Interfaces;
using GarageFlowService.Domain.Exceptions;
using MediatR;

namespace GarageFlowService.Application.UseCases.WorkOrders;

public record AddPartToWorkOrderCommand(Guid WorkOrderId, Guid PartId, int Quantity) : IRequest<WorkOrderDto?>;

public class AddPartToWorkOrderHandler : IRequestHandler<AddPartToWorkOrderCommand, WorkOrderDto?>
{
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IPartRepository _partRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddPartToWorkOrderHandler(
        IWorkOrderRepository workOrderRepository,
        IPartRepository partRepository,
        IUnitOfWork unitOfWork)
    {
        _workOrderRepository = workOrderRepository;
        _partRepository = partRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<WorkOrderDto?> Handle(AddPartToWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await _workOrderRepository.GetByIdWithDetailsAsync(request.WorkOrderId, cancellationToken);
        if (workOrder is null)
            throw new DomainException($"Ordem de serviço com ID '{request.WorkOrderId}' não encontrada.");

        var part = await _partRepository.GetByIdAsync(request.PartId, cancellationToken);
        if (part is null)
            throw new DomainException($"Peça com ID '{request.PartId}' não encontrada.");

        await _workOrderRepository.AddPartToWorkOrderAsync(workOrder, part, request.Quantity, cancellationToken);
        _workOrderRepository.Update(workOrder);
        await _unitOfWork.CommitAsync(cancellationToken);
        return CreateWorkOrderHandler.MapToDto(workOrder);
    }
}

