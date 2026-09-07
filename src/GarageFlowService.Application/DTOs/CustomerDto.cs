namespace GarageFlowService.Application.DTOs;

public record CustomerDto(Guid Id, string Name, string Email, string Phone, string Document, DateTime CreatedAt);
public record CreateCustomerRequest(string Name, string Email, string Phone, string Document);
public record UpdateCustomerRequest(string Name, string Email, string Phone);

