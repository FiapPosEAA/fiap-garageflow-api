using GarageFlowService.Domain.Entities;

namespace GarageFlowService.Domain.Interfaces;

public interface IWorkOrderRepository : IRepository<WorkOrder>
{
    Task<WorkOrder?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<WorkOrder>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
    Task AddServiceToWorkOrderAsync(WorkOrder workOrder, Service service, int quantity, CancellationToken cancellationToken = default);
    Task AddPartToWorkOrderAsync(WorkOrder workOrder, Part part, int quantity, CancellationToken cancellationToken = default);
}

