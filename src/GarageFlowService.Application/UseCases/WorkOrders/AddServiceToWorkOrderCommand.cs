using GarageFlowService.Application.DTOs;
using GarageFlowService.Domain.Interfaces;
using GarageFlowService.Application.Interfaces;
using GarageFlowService.Domain.Exceptions;
using MediatR;

namespace GarageFlowService.Application.UseCases.WorkOrders;

public record AddServiceToWorkOrderCommand(Guid WorkOrderId, Guid ServiceId, int Quantity) : IRequest<WorkOrderDto?>;

public class AddServiceToWorkOrderHandler : IRequestHandler<AddServiceToWorkOrderCommand, WorkOrderDto?>
{
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddServiceToWorkOrderHandler(
        IWorkOrderRepository workOrderRepository,
        IServiceRepository serviceRepository,
        IUnitOfWork unitOfWork)
    {
        _workOrderRepository = workOrderRepository;
        _serviceRepository = serviceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<WorkOrderDto?> Handle(AddServiceToWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await _workOrderRepository.GetByIdWithDetailsAsync(request.WorkOrderId, cancellationToken);
        if (workOrder is null)
            throw new DomainException($"Ordem de serviço com ID '{request.WorkOrderId}' não encontrada.");

        var service = await _serviceRepository.GetByIdAsync(request.ServiceId, cancellationToken);
        if (service is null)
            throw new DomainException($"Serviço com ID '{request.ServiceId}' não encontrado.");

        await _workOrderRepository.AddServiceToWorkOrderAsync(workOrder, service, request.Quantity, cancellationToken);
        _workOrderRepository.Update(workOrder);
        await _unitOfWork.CommitAsync(cancellationToken);
        return CreateWorkOrderHandler.MapToDto(workOrder);
    }
}

