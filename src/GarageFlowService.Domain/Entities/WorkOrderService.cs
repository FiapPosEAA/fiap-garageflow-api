namespace GarageFlowService.Domain.Entities;

public class WorkOrderService : Entity
{
    public Guid WorkOrderId { get; private set; }
    public Guid ServiceId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Subtotal => UnitPrice * Quantity;

    public Service? Service { get; private set; }

    protected WorkOrderService() { }

    public WorkOrderService(Guid workOrderId, Guid serviceId, int quantity, decimal unitPrice)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive.", nameof(quantity));
        if (unitPrice < 0) throw new ArgumentException("Unit price cannot be negative.", nameof(unitPrice));

        WorkOrderId = workOrderId;
        ServiceId = serviceId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}

