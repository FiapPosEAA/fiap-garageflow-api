using GarageFlowService.Domain.Enums;
using GarageFlowService.Domain.Exceptions;

namespace GarageFlowService.Domain.Entities;

public class WorkOrder : Entity
{
    public string OrderNumber { get; private set; } = string.Empty;
    public Guid CustomerId { get; private set; }
    public Guid VehicleId { get; private set; }
    public WorkOrderStatus Status { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public string? DiagnosisNotes { get; private set; }
    public decimal TotalAmount { get; private set; }

    public Customer? Customer { get; private set; }
    public Vehicle? Vehicle { get; private set; }

    private readonly List<WorkOrderService> _workOrderServices = new();
    public IReadOnlyCollection<WorkOrderService> WorkOrderServices => _workOrderServices.AsReadOnly();

    private readonly List<WorkOrderPart> _workOrderParts = new();
    public IReadOnlyCollection<WorkOrderPart> WorkOrderParts => _workOrderParts.AsReadOnly();

    protected WorkOrder() { }

    public WorkOrder(Guid customerId, Guid vehicleId, string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);

        CustomerId = customerId;
        VehicleId = vehicleId;
        Description = description;
        Status = WorkOrderStatus.Received;
        OrderNumber = GenerateOrderNumber();
    }

    private static string GenerateOrderNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        return $"OS-{timestamp}";
    }

    public void UpdateStatus(WorkOrderStatus newStatus)
    {
        ValidateStatusTransition(Status, newStatus);
        Status = newStatus;
        SetUpdatedAt();
    }

    public void ResolveBudget(bool approved)
    {
        if (Status != WorkOrderStatus.WaitingApproval)
            throw new DomainException("Budget can only be resolved when the work order is waiting approval.");

        UpdateStatus(approved ? WorkOrderStatus.InProgress : WorkOrderStatus.Canceled);
    }

    public void AddDiagnosisNotes(string notes)
    {
        if (Status != WorkOrderStatus.Diagnosis)
            throw new DomainException("Diagnosis notes can only be added when status is Diagnosis.");
        DiagnosisNotes = notes;
        SetUpdatedAt();
    }

    public WorkOrderService AddService(Service service, int quantity)
    {
        if (Status is WorkOrderStatus.Finished or WorkOrderStatus.Delivered or WorkOrderStatus.Canceled)
            throw new DomainException("Cannot add services to a finished, delivered or canceled work order.");

        var existing = _workOrderServices.FirstOrDefault(s => s.ServiceId == service.Id);
        if (existing != null)
            throw new DomainException("Service already added to this work order.");

        var newService = new WorkOrderService(Id, service.Id, quantity, service.Price);
        _workOrderServices.Add(newService);
        CalculateTotal();
        SetUpdatedAt();
        return newService;
    }

    public WorkOrderPart AddPart(Part part, int quantity)
    {
        if (Status is WorkOrderStatus.Finished or WorkOrderStatus.Delivered or WorkOrderStatus.Canceled)
            throw new DomainException("Cannot add parts to a finished, delivered or canceled work order.");

        var existing = _workOrderParts.FirstOrDefault(p => p.PartId == part.Id);
        if (existing != null)
            throw new DomainException("Part already added to this work order.");

        var newPart = new WorkOrderPart(Id, part.Id, quantity, part.Price);
        _workOrderParts.Add(newPart);
        CalculateTotal();
        SetUpdatedAt();
        return newPart;
    }

    public void CalculateTotal()
    {
        TotalAmount = _workOrderServices.Sum(s => s.Subtotal)
                    + _workOrderParts.Sum(p => p.Subtotal);
    }

    public BudgetSummary GenerateBudget()
    {
        return new BudgetSummary(
            OrderNumber,
            _workOrderServices.Select(s => new BudgetItem(s.Service?.Name ?? "Service", s.Quantity, s.UnitPrice, s.Subtotal)).ToList(),
            _workOrderParts.Select(p => new BudgetItem(p.Part?.Name ?? "Part", p.Quantity, p.UnitPrice, p.Subtotal)).ToList(),
            TotalAmount
        );
    }

    private static void ValidateStatusTransition(WorkOrderStatus current, WorkOrderStatus next)
    {
        var validTransitions = new Dictionary<WorkOrderStatus, WorkOrderStatus[]>
        {
            { WorkOrderStatus.Received, new[] { WorkOrderStatus.Diagnosis, WorkOrderStatus.Canceled } },
            { WorkOrderStatus.Diagnosis, new[] { WorkOrderStatus.WaitingApproval, WorkOrderStatus.Canceled } },
            { WorkOrderStatus.WaitingApproval, new[] { WorkOrderStatus.InProgress, WorkOrderStatus.Received, WorkOrderStatus.Canceled } },
            { WorkOrderStatus.InProgress, new[] { WorkOrderStatus.Finished, WorkOrderStatus.Canceled } },
            { WorkOrderStatus.Finished, new[] { WorkOrderStatus.Delivered, WorkOrderStatus.Diagnosis } },
            { WorkOrderStatus.Delivered, Array.Empty<WorkOrderStatus>() },
            { WorkOrderStatus.Canceled, Array.Empty<WorkOrderStatus>() }
        };

        if (!validTransitions.TryGetValue(current, out var allowed) || !allowed.Contains(next))
            throw new DomainException($"Invalid status transition from {current} to {next}.");
    }
}

public record BudgetItem(string Name, int Quantity, decimal UnitPrice, decimal Subtotal);
public record BudgetSummary(string OrderNumber, List<BudgetItem> Services, List<BudgetItem> Parts, decimal Total);

