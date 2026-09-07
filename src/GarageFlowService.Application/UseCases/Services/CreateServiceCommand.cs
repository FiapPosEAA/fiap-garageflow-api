using GarageFlowService.Application.DTOs;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Domain.Interfaces;
using GarageFlowService.Application.Interfaces;
using MediatR;

namespace GarageFlowService.Application.UseCases.Services;

public record CreateServiceCommand(string Name, string Description, decimal Price, decimal EstimatedHours) : IRequest<ServiceDto>;

public class CreateServiceHandler : IRequestHandler<CreateServiceCommand, ServiceDto>
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateServiceHandler(IServiceRepository serviceRepository, IUnitOfWork unitOfWork)
    {
        _serviceRepository = serviceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceDto> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        var service = new Service(request.Name, request.Description, request.Price, request.EstimatedHours);
        await _serviceRepository.AddAsync(service, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return new ServiceDto(service.Id, service.Name, service.Description, service.Price, service.EstimatedHours, service.IsActive);
    }
}

