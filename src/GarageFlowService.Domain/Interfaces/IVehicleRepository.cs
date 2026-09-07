using GarageFlowService.Domain.Entities;

namespace GarageFlowService.Domain.Interfaces;

public interface IVehicleRepository : IRepository<Vehicle>
{
    Task<IEnumerable<Vehicle>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
}

