namespace GarageFlowService.Domain.Entities;

public class Part : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public bool IsActive { get; private set; }

    protected Part() { }

    public Part(string name, string description, decimal price, int stockQuantity)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (price < 0) throw new ArgumentException("Price cannot be negative.", nameof(price));
        if (stockQuantity < 0) throw new ArgumentException("Stock quantity cannot be negative.", nameof(stockQuantity));

        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
        IsActive = true;
    }

    public void UpdateStock(int quantity)
    {
        if (StockQuantity + quantity < 0)
            throw new InvalidOperationException("Insufficient stock.");
        StockQuantity += quantity;
        SetUpdatedAt();
    }

    public void Deactivate() => IsActive = false;
}

