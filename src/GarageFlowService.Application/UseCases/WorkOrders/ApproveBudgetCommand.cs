using GarageFlowService.Application.DTOs;
using GarageFlowService.Application.Interfaces;
using GarageFlowService.Domain.Exceptions;
using GarageFlowService.Domain.Interfaces;
using MediatR;

namespace GarageFlowService.Application.UseCases.WorkOrders;

public record ApproveBudgetCommand(Guid Id, bool Approved) : IRequest<WorkOrderDto?>;

public class ApproveBudgetHandler : IRequestHandler<ApproveBudgetCommand, WorkOrderDto?>
{
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ApproveBudgetHandler(IWorkOrderRepository workOrderRepository, IUnitOfWork unitOfWork)
    {
        _workOrderRepository = workOrderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<WorkOrderDto?> Handle(ApproveBudgetCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await _workOrderRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (workOrder is null)
            return null;

        try
        {
            workOrder.ResolveBudget(request.Approved);
        }
        catch (DomainException)
        {
            throw;
        }

        _workOrderRepository.Update(workOrder);
        await _unitOfWork.CommitAsync(cancellationToken);
        return CreateWorkOrderHandler.MapToDto(workOrder);
    }
}