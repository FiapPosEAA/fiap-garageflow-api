using GarageFlowService.Application.DTOs;
using GarageFlowService.Domain.Interfaces;
using MediatR;

namespace GarageFlowService.Application.UseCases.Vehicles;

public record GetVehiclesByCustomerQuery(Guid CustomerId) : IRequest<IEnumerable<VehicleDto>>;

public class GetVehiclesByCustomerHandler : IRequestHandler<GetVehiclesByCustomerQuery, IEnumerable<VehicleDto>>
{
    private readonly IVehicleRepository _vehicleRepository;

    public GetVehiclesByCustomerHandler(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task<IEnumerable<VehicleDto>> Handle(GetVehiclesByCustomerQuery request, CancellationToken cancellationToken)
    {
        var vehicles = await _vehicleRepository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);
        return vehicles.Select(v => new VehicleDto(v.Id, v.CustomerId, v.Brand, v.Model, v.Year, v.LicensePlate, v.Color));
    }
}

