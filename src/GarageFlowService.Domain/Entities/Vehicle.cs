namespace GarageFlowService.Domain.Entities;

public class Vehicle : Entity
{
    public Guid CustomerId { get; private set; }
    public string Brand { get; private set; } = string.Empty;
    public string Model { get; private set; } = string.Empty;
    public int Year { get; private set; }
    public string LicensePlate { get; private set; } = string.Empty;
    public string Color { get; private set; } = string.Empty;

    public Customer? Customer { get; private set; }

    protected Vehicle() { }

    public Vehicle(Guid customerId, string brand, string model, int year, string licensePlate, string color)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(brand);
        ArgumentException.ThrowIfNullOrWhiteSpace(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(licensePlate);

        CustomerId = customerId;
        Brand = brand;
        Model = model;
        Year = year;
        LicensePlate = licensePlate;
        Color = color;
    }
}

