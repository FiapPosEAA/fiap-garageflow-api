namespace GarageFlowService.Application.DTOs;

public record VehicleDto(Guid Id, Guid CustomerId, string Brand, string Model, int Year, string LicensePlate, string Color);
public record CreateVehicleRequest(Guid CustomerId, string Brand, string Model, int Year, string LicensePlate, string Color);

