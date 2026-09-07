using GarageFlowService.Domain.Enums;

namespace GarageFlowService.Application.DTOs;

public record WorkOrderDto(
    Guid Id,
    string OrderNumber,
    Guid CustomerId,
    string CustomerName,
    Guid VehicleId,
    string VehicleInfo,
    WorkOrderStatus Status,
    string StatusDescription,
    string Description,
    string? DiagnosisNotes,
    decimal TotalAmount,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record WorkOrderStatusDto(
    Guid Id,
    string OrderNumber,
    WorkOrderStatus Status,
    string StatusDescription,
    DateTime UpdatedAt);

public record WorkOrderDetailDto(
    Guid Id,
    string OrderNumber,
    Guid CustomerId,
    string CustomerName,
    Guid VehicleId,
    string VehicleInfo,
    WorkOrderStatus Status,
    string StatusDescription,
    string Description,
    string? DiagnosisNotes,
    decimal TotalAmount,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<WorkOrderServiceDto> Services,
    List<WorkOrderPartDto> Parts);

public record WorkOrderServiceDto(Guid ServiceId, string ServiceName, int Quantity, decimal UnitPrice, decimal Subtotal);
public record WorkOrderPartDto(Guid PartId, string PartName, int Quantity, decimal UnitPrice, decimal Subtotal);

public record CreateWorkOrderRequest(Guid CustomerId, Guid VehicleId, string Description);
public record UpdateWorkOrderStatusRequest(int NewStatus);
public record AddServiceToWorkOrderRequest(Guid ServiceId, int Quantity);
public record AddPartToWorkOrderRequest(Guid PartId, int Quantity);
public record BudgetDecisionRequest(bool Approved);

public record BudgetDto(
    string OrderNumber,
    List<BudgetItemDto> Services,
    List<BudgetItemDto> Parts,
    decimal Total);

public record BudgetItemDto(string Name, int Quantity, decimal UnitPrice, decimal Subtotal);

