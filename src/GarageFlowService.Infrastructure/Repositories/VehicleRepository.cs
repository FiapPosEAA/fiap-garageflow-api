using System.Diagnostics.CodeAnalysis;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Domain.Interfaces;
using GarageFlowService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GarageFlowService.Infrastructure.Repositories;

[ExcludeFromCodeCoverage]
public class VehicleRepository : Repository<Vehicle>, IVehicleRepository
{
    public VehicleRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Vehicle>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        => await _dbSet.Where(v => v.CustomerId == customerId).ToListAsync(cancellationToken);
}

