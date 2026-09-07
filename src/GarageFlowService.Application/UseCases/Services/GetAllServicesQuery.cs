using GarageFlowService.Application.DTOs;
using GarageFlowService.Domain.Interfaces;
using MediatR;

namespace GarageFlowService.Application.UseCases.Services;

public record GetAllServicesQuery : IRequest<IEnumerable<ServiceDto>>;

public class GetAllServicesHandler : IRequestHandler<GetAllServicesQuery, IEnumerable<ServiceDto>>
{
    private readonly IServiceRepository _serviceRepository;

    public GetAllServicesHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<IEnumerable<ServiceDto>> Handle(GetAllServicesQuery request, CancellationToken cancellationToken)
    {
        var services = await _serviceRepository.GetAllAsync(cancellationToken);
        return services.Select(s => new ServiceDto(s.Id, s.Name, s.Description, s.Price, s.EstimatedHours, s.IsActive));
    }
}

