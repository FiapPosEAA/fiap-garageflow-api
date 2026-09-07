namespace GarageFlowService.Domain.Entities;

public class WorkOrderPart : Entity
{
    public Guid WorkOrderId { get; private set; }
    public Guid PartId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Subtotal => UnitPrice * Quantity;

    public Part? Part { get; private set; }

    protected WorkOrderPart() { }

    public WorkOrderPart(Guid workOrderId, Guid partId, int quantity, decimal unitPrice)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive.", nameof(quantity));
        if (unitPrice < 0) throw new ArgumentException("Unit price cannot be negative.", nameof(unitPrice));

        WorkOrderId = workOrderId;
        PartId = partId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}

