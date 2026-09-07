using System.Diagnostics.CodeAnalysis;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Domain.Interfaces;
using GarageFlowService.Infrastructure.Data;

namespace GarageFlowService.Infrastructure.Repositories;

[ExcludeFromCodeCoverage]
public class PartRepository : Repository<Part>, IPartRepository
{
    public PartRepository(AppDbContext context) : base(context) { }
}

