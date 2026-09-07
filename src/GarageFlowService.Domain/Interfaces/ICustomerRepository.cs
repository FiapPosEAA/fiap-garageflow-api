using GarageFlowService.Domain.Entities;

namespace GarageFlowService.Domain.Interfaces;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByDocumentAsync(string document, CancellationToken cancellationToken = default);
}

