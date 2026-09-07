using FluentAssertions;
using Moq;
using GarageFlowService.Application.UseCases.Vehicles;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Domain.Interfaces;

namespace GarageFlowService.Tests.Application.Vehicles;

public class GetVehiclesByCustomerQueryTests
{
    private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
    private readonly GetVehiclesByCustomerHandler _handler;

    public GetVehiclesByCustomerQueryTests()
    {
        _vehicleRepositoryMock = new Mock<IVehicleRepository>();
        _handler = new GetVehiclesByCustomerHandler(_vehicleRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_CustomerHasNoVehicles_ShouldReturnEmptyList()
    {
        var customerId = Guid.NewGuid();
        var vehicles = new List<Vehicle>();
        
        _vehicleRepositoryMock
            .Setup(x => x.GetByCustomerIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicles);

        var query = new GetVehiclesByCustomerQuery(customerId);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_CustomerHasVehicles_ShouldReturnAllVehicles()
    {
        var customerId = Guid.NewGuid();
        var vehicle1 = new Vehicle(customerId, "Toyota", "Corolla", 2020, "ABC1234", "Prata");
        var vehicle2 = new Vehicle(customerId, "Honda", "Civic", 2021, "XYZ9876", "Preto");
        var vehicles = new List<Vehicle> { vehicle1, vehicle2 };
        
        _vehicleRepositoryMock
            .Setup(x => x.GetByCustomerIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicles);

        var query = new GetVehiclesByCustomerQuery(customerId);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().HaveCount(2);
        result.Should().AllSatisfy(dto => dto.CustomerId.Should().Be(customerId));
    }

    [Fact]
    public async Task Handle_SingleVehicle_ShouldMapCorrectly()
    {
        var customerId = Guid.NewGuid();
        var vehicle = new Vehicle(customerId, "Toyota", "Corolla", 2020, "ABC1234", "Prata");
        var vehicles = new List<Vehicle> { vehicle };
        
        _vehicleRepositoryMock
            .Setup(x => x.GetByCustomerIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicles);

        var query = new GetVehiclesByCustomerQuery(customerId);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().HaveCount(1);
        var dto = result.First();
        dto.Brand.Should().Be("Toyota");
        dto.Model.Should().Be("Corolla");
        dto.Year.Should().Be(2020);
        dto.LicensePlate.Should().Be("ABC1234");
        dto.Color.Should().Be("Prata");
    }

    [Fact]
    public async Task Handle_MultipleVehicles_ShouldMapAllCorrectly()
    {
        var customerId = Guid.NewGuid();
        var vehicle1 = new Vehicle(customerId, "Toyota", "Corolla", 2020, "ABC1234", "Prata");
        var vehicle2 = new Vehicle(customerId, "Honda", "Civic", 2021, "XYZ9876", "Preto");
        var vehicle3 = new Vehicle(customerId, "Volkswagen", "Gol", 2019, "DEF5678", "Branco");
        var vehicles = new List<Vehicle> { vehicle1, vehicle2, vehicle3 };
        
        _vehicleRepositoryMock
            .Setup(x => x.GetByCustomerIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicles);

        var query = new GetVehiclesByCustomerQuery(customerId);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().HaveCount(3);
        result.Should().Contain(dto => dto.Brand == "Toyota" && dto.Model == "Corolla");
        result.Should().Contain(dto => dto.Brand == "Honda" && dto.Model == "Civic");
        result.Should().Contain(dto => dto.Brand == "Volkswagen" && dto.Model == "Gol");
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryGetByCustomerIdAsync()
    {
        var customerId = Guid.NewGuid();
        
        _vehicleRepositoryMock
            .Setup(x => x.GetByCustomerIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Vehicle>());

        var query = new GetVehiclesByCustomerQuery(customerId);
        await _handler.Handle(query, CancellationToken.None);

        _vehicleRepositoryMock.Verify(
            x => x.GetByCustomerIdAsync(customerId, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
