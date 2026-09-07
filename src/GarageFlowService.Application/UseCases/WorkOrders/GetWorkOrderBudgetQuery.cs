using GarageFlowService.Application.DTOs;
using GarageFlowService.Domain.Interfaces;
using MediatR;

namespace GarageFlowService.Application.UseCases.WorkOrders;

public record GetWorkOrderBudgetQuery(Guid Id) : IRequest<BudgetDto?>;

public class GetWorkOrderBudgetHandler : IRequestHandler<GetWorkOrderBudgetQuery, BudgetDto?>
{
    private readonly IWorkOrderRepository _workOrderRepository;

    public GetWorkOrderBudgetHandler(IWorkOrderRepository workOrderRepository)
    {
        _workOrderRepository = workOrderRepository;
    }

    public async Task<BudgetDto?> Handle(GetWorkOrderBudgetQuery request, CancellationToken cancellationToken)
    {
        var wo = await _workOrderRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (wo is null) return null;

        wo.CalculateTotal();
        var budget = wo.GenerateBudget();

        return new BudgetDto(
            budget.OrderNumber,
            budget.Services.Select(s => new BudgetItemDto(s.Name, s.Quantity, s.UnitPrice, s.Subtotal)).ToList(),
            budget.Parts.Select(p => new BudgetItemDto(p.Name, p.Quantity, p.UnitPrice, p.Subtotal)).ToList(),
            budget.Total);
    }
}

