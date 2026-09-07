using FluentAssertions;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Infrastructure.Repositories;
using Xunit;

namespace GarageFlowService.Tests.Integration;

public class VehicleRepositoryIntegrationTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;

    public VehicleRepositoryIntegrationTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateVehicle_ShouldPersistAndRetrieve()
    {
        var customerId = Guid.NewGuid();
        var repository = new VehicleRepository(_fixture.DbContext);
        var vehicle = new Vehicle(customerId, "Toyota", "Corolla", 2020, "ABC1234", "Prata");

        await repository.AddAsync(vehicle);
        await _fixture.DbContext.SaveChangesAsync();

        var retrieved = await repository.GetByIdAsync(vehicle.Id);

        retrieved.Should().NotBeNull();
        retrieved!.Brand.Should().Be("Toyota");
        retrieved.Model.Should().Be("Corolla");
        retrieved.CustomerId.Should().Be(customerId);
    }

    [Fact]
    public async Task GetByCustomerIdAsync_ShouldReturnVehiclesForCustomer()
    {
        _fixture.ResetDatabase();
        var customerId1 = Guid.NewGuid();
        var customerId2 = Guid.NewGuid();
        var repository = new VehicleRepository(_fixture.DbContext);

        var vehicle1 = new Vehicle(customerId1, "Toyota", "Corolla", 2020, "ABC1234", "Prata");
        var vehicle2 = new Vehicle(customerId1, "Honda", "Civic", 2021, "XYZ5678", "Preto");
        var vehicle3 = new Vehicle(customerId2, "Fiat", "Uno", 2019, "DEF9012", "Branco");

        await repository.AddAsync(vehicle1);
        await repository.AddAsync(vehicle2);
        await repository.AddAsync(vehicle3);
        await _fixture.DbContext.SaveChangesAsync();

        var result = await repository.GetByCustomerIdAsync(customerId1);

        result.Should().HaveCount(2);
        result.Should().Contain(v => v.Brand == "Toyota");
        result.Should().Contain(v => v.Brand == "Honda");
        result.Should().NotContain(v => v.Brand == "Fiat");
    }

    [Fact]
    public async Task DeleteVehicle_ShouldRemoveFromDatabase()
    {
        _fixture.ResetDatabase();
        var customerId = Guid.NewGuid();
        var repository = new VehicleRepository(_fixture.DbContext);
        var vehicle = new Vehicle(customerId, "Toyota", "Corolla", 2020, "ABC1234", "Prata");

        await repository.AddAsync(vehicle);
        await _fixture.DbContext.SaveChangesAsync();

        var vehicleId = vehicle.Id;
        var toDelete = await repository.GetByIdAsync(vehicleId);
        toDelete.Should().NotBeNull();

        repository.Remove(toDelete!);
        await _fixture.DbContext.SaveChangesAsync();

        var deleted = await repository.GetByIdAsync(vehicleId);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllVehicles()
    {
        _fixture.ResetDatabase();
        var customerId = Guid.NewGuid();
        var repository = new VehicleRepository(_fixture.DbContext);

        var vehicle1 = new Vehicle(customerId, "Toyota", "Corolla", 2020, "ABC1234", "Prata");
        var vehicle2 = new Vehicle(customerId, "Honda", "Civic", 2021, "XYZ5678", "Preto");

        await repository.AddAsync(vehicle1);
        await repository.AddAsync(vehicle2);
        await _fixture.DbContext.SaveChangesAsync();

        var result = await repository.GetAllAsync();

        result.Should().HaveCount(2);
        result.Should().Contain(v => v.Model == "Corolla");
        result.Should().Contain(v => v.Model == "Civic");
    }
}

