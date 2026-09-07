using System.Diagnostics.CodeAnalysis;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Domain.Interfaces;
using GarageFlowService.Infrastructure.Data;

namespace GarageFlowService.Infrastructure.Repositories;

[ExcludeFromCodeCoverage]
public class ServiceRepository : Repository<Service>, IServiceRepository
{
    public ServiceRepository(AppDbContext context) : base(context) { }
}

