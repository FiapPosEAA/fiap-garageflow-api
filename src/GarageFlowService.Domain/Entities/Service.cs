namespace GarageFlowService.Domain.Entities;

public class Service : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public decimal EstimatedHours { get; private set; }
    public bool IsActive { get; private set; }

    protected Service() { }

    public Service(string name, string description, decimal price, decimal estimatedHours)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (price < 0) throw new ArgumentException("Price cannot be negative.", nameof(price));

        Name = name;
        Description = description;
        Price = price;
        EstimatedHours = estimatedHours;
        IsActive = true;
    }

    public void Update(string name, string description, decimal price, decimal estimatedHours)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (price < 0) throw new ArgumentException("Price cannot be negative.", nameof(price));

        Name = name;
        Description = description;
        Price = price;
        EstimatedHours = estimatedHours;
        SetUpdatedAt();
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}

