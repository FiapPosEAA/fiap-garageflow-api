namespace GarageFlowService.Domain.Entities;

public class Customer : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string Document { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    private readonly List<Vehicle> _vehicles = new();
    public IReadOnlyCollection<Vehicle> Vehicles => _vehicles.AsReadOnly();

    protected Customer() { }

    public Customer(string name, string email, string phone, string document, bool isActive = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(document);

        Name = name;
        Email = email;
        Phone = phone;
        Document = NormalizeDocument(document);
        IsActive = isActive;
    }

    public void Update(string name, string email, string phone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        Name = name;
        Email = email;
        Phone = phone;
        SetUpdatedAt();
    }

    public void SetActiveStatus(bool isActive)
    {
        IsActive = isActive;
        SetUpdatedAt();
    }

    private static string NormalizeDocument(string document)
        => new(string.Concat(document.Where(char.IsDigit)).ToArray());
}

