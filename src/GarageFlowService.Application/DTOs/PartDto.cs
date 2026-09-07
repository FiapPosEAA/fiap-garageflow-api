namespace GarageFlowService.Application.DTOs;

public record PartDto(Guid Id, string Name, string Description, decimal Price, int StockQuantity, bool IsActive);
public record CreatePartRequest(string Name, string Description, decimal Price, int StockQuantity);
public record UpdateStockRequest(int QuantityChange);

