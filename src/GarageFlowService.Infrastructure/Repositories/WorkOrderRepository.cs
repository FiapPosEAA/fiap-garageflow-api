using System.Diagnostics.CodeAnalysis;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Domain.Interfaces;
using GarageFlowService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GarageFlowService.Infrastructure.Repositories;

[ExcludeFromCodeCoverage]
public class WorkOrderRepository : Repository<WorkOrder>, IWorkOrderRepository
{
    public WorkOrderRepository(AppDbContext context) : base(context) { }

    public async Task<WorkOrder?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet
            .Include(wo => wo.Customer)
            .Include(wo => wo.Vehicle)
            .Include(wo => wo.WorkOrderServices).ThenInclude(ws => ws.Service)
            .Include(wo => wo.WorkOrderParts).ThenInclude(wp => wp.Part)
            .FirstOrDefaultAsync(wo => wo.Id == id, cancellationToken);

    public async Task<IEnumerable<WorkOrder>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
        => await _dbSet
            .Include(wo => wo.Customer)
            .Include(wo => wo.Vehicle)
            .ToListAsync(cancellationToken);

    public async Task AddServiceToWorkOrderAsync(WorkOrder workOrder, Service service, int quantity, CancellationToken cancellationToken = default)
    {
        var workOrderService = workOrder.AddService(service, quantity);
        _context.Set<WorkOrderService>().Add(workOrderService);
        await Task.CompletedTask;
    }

    public async Task AddPartToWorkOrderAsync(WorkOrder workOrder, Part part, int quantity, CancellationToken cancellationToken = default)
    {
        var workOrderPart = workOrder.AddPart(part, quantity);
        _context.Set<WorkOrderPart>().Add(workOrderPart);
        await Task.CompletedTask;
    }
}


