using GarageFlowService.Application.DTOs;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Domain.Interfaces;
using GarageFlowService.Application.Interfaces;
using MediatR;

namespace GarageFlowService.Application.UseCases.Vehicles;

public record CreateVehicleCommand(Guid CustomerId, string Brand, string Model, int Year, string LicensePlate, string Color) : IRequest<VehicleDto>;

public class CreateVehicleHandler : IRequestHandler<CreateVehicleCommand, VehicleDto>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateVehicleHandler(IVehicleRepository vehicleRepository, IUnitOfWork unitOfWork)
    {
        _vehicleRepository = vehicleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<VehicleDto> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = new Vehicle(request.CustomerId, request.Brand, request.Model, request.Year, request.LicensePlate, request.Color);
        await _vehicleRepository.AddAsync(vehicle, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return new VehicleDto(vehicle.Id, vehicle.CustomerId, vehicle.Brand, vehicle.Model, vehicle.Year, vehicle.LicensePlate, vehicle.Color);
    }
}

