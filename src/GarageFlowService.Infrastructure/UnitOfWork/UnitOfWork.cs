using System.Diagnostics.CodeAnalysis;
using GarageFlowService.Application.Interfaces;
using GarageFlowService.Infrastructure.Data;

namespace GarageFlowService.Infrastructure.UnitOfWork;

[ExcludeFromCodeCoverage]
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}

