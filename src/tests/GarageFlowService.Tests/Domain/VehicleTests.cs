using FluentAssertions;
using GarageFlowService.Domain.Entities;

namespace GarageFlowService.Tests.Domain;

public class VehicleTests
{
    [Fact]
    public void Create_Vehicle_ShouldSetPropertiesCorrectly()
    {
        var customerId = Guid.NewGuid();
        var vehicle = new Vehicle(customerId, "Toyota", "Corolla", 2023, "ABC1234", "Prata");

        vehicle.CustomerId.Should().Be(customerId);
        vehicle.Brand.Should().Be("Toyota");
        vehicle.Model.Should().Be("Corolla");
        vehicle.Year.Should().Be(2023);
        vehicle.LicensePlate.Should().Be("ABC1234");
        vehicle.Color.Should().Be("Prata");
        vehicle.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_Vehicle_WithEmptyBrand_ShouldThrow()
    {
        var customerId = Guid.NewGuid();
        var act = () => new Vehicle(customerId, "", "Corolla", 2023, "ABC1234", "Prata");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_Vehicle_WithEmptyModel_ShouldThrow()
    {
        var customerId = Guid.NewGuid();
        var act = () => new Vehicle(customerId, "Toyota", "", 2023, "ABC1234", "Prata");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_Vehicle_WithEmptyLicensePlate_ShouldThrow()
    {
        var customerId = Guid.NewGuid();
        var act = () => new Vehicle(customerId, "Toyota", "Corolla", 2023, "", "Prata");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_Vehicle_WithWhitespaceOnlyBrand_ShouldThrow()
    {
        var customerId = Guid.NewGuid();
        var act = () => new Vehicle(customerId, "   ", "Corolla", 2023, "ABC1234", "Prata");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_Vehicle_MultipleVehicles_ShouldHaveDifferentIds()
    {
        var customerId = Guid.NewGuid();
        var vehicle1 = new Vehicle(customerId, "Toyota", "Corolla", 2023, "ABC1234", "Prata");
        var vehicle2 = new Vehicle(customerId, "Honda", "Civic", 2022, "XYZ5678", "Preto");

        vehicle1.Id.Should().NotBe(vehicle2.Id);
    }
}
