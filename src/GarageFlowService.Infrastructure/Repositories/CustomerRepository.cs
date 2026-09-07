using System.Diagnostics.CodeAnalysis;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Domain.Interfaces;
using GarageFlowService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GarageFlowService.Infrastructure.Repositories;

[ExcludeFromCodeCoverage]
public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(AppDbContext context) : base(context) { }

    public async Task<Customer?> GetByDocumentAsync(string document, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(c => c.Document == document, cancellationToken);
}

