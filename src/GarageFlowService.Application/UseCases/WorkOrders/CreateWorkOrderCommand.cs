using GarageFlowService.Application.DTOs;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Domain.Interfaces;
using GarageFlowService.Application.Interfaces;
using GarageFlowService.Domain.Exceptions;
using MediatR;

namespace GarageFlowService.Application.UseCases.WorkOrders;

public record CreateWorkOrderCommand(Guid CustomerId, Guid VehicleId, string Description) : IRequest<WorkOrderDto>;

public class CreateWorkOrderHandler : IRequestHandler<CreateWorkOrderCommand, WorkOrderDto>
{
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateWorkOrderHandler(
        IWorkOrderRepository workOrderRepository,
        ICustomerRepository customerRepository,
        IVehicleRepository vehicleRepository,
        IUnitOfWork unitOfWork)
    {
        _workOrderRepository = workOrderRepository;
        _customerRepository = customerRepository;
        _vehicleRepository = vehicleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<WorkOrderDto> Handle(CreateWorkOrderCommand request, CancellationToken cancellationToken)
    {
        // Validar se o cliente existe
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer is null)
            throw new DomainException($"Cliente com ID '{request.CustomerId}' não encontrado.");

        // Validar se o veículo existe
        var vehicle = await _vehicleRepository.GetByIdAsync(request.VehicleId, cancellationToken);
        if (vehicle is null)
            throw new DomainException($"Veículo com ID '{request.VehicleId}' não encontrado.");

        // Validar se o veículo pertence ao cliente
        if (vehicle.CustomerId != request.CustomerId)
            throw new DomainException($"O veículo '{request.VehicleId}' não pertence ao cliente '{request.CustomerId}'.");

        var workOrder = new WorkOrder(request.CustomerId, request.VehicleId, request.Description);
        await _workOrderRepository.AddAsync(workOrder, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return MapToDto(workOrder);
    }

    internal static WorkOrderDto MapToDto(WorkOrder wo) => new(
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
        wo.UpdatedAt);
}

