namespace GarageFlowService.Application.DTOs;

public record ServiceDto(Guid Id, string Name, string Description, decimal Price, decimal EstimatedHours, bool IsActive);
public record CreateServiceRequest(string Name, string Description, decimal Price, decimal EstimatedHours);

