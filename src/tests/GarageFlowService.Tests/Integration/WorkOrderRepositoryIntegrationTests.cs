using FluentAssertions;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Infrastructure.Repositories;
using Xunit;

namespace GarageFlowService.Tests.Integration;

public class WorkOrderRepositoryIntegrationTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;

    public WorkOrderRepositoryIntegrationTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateWorkOrder_ShouldPersistAndRetrieve()
    {
        var customerId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        var repository = new WorkOrderRepository(_fixture.DbContext);

        var workOrder = new WorkOrder(customerId, vehicleId, "Revisão completa");
        await repository.AddAsync(workOrder);
        await _fixture.DbContext.SaveChangesAsync();

        var retrieved = await repository.GetByIdAsync(workOrder.Id);

        retrieved.Should().NotBeNull();
        retrieved!.Description.Should().Be("Revisão completa");
        retrieved.CustomerId.Should().Be(customerId);
        retrieved.VehicleId.Should().Be(vehicleId);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllWorkOrders()
    {
        _fixture.ResetDatabase();
        var repository = new WorkOrderRepository(_fixture.DbContext);
        var customerId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();

        var workOrder1 = new WorkOrder(customerId, vehicleId, "Revisão");
        var workOrder2 = new WorkOrder(customerId, vehicleId, "Manutenção");

        await repository.AddAsync(workOrder1);
        await repository.AddAsync(workOrder2);
        await _fixture.DbContext.SaveChangesAsync();

        var result = await repository.GetAllAsync();

        result.Should().HaveCount(2);
        result.Should().Contain(w => w.Description == "Revisão");
        result.Should().Contain(w => w.Description == "Manutenção");
    }

    [Fact]
    public async Task DeleteWorkOrder_ShouldRemoveFromDatabase()
    {
        _fixture.ResetDatabase();
        var repository = new WorkOrderRepository(_fixture.DbContext);
        var customerId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();

        var workOrder = new WorkOrder(customerId, vehicleId, "Revisão");
        await repository.AddAsync(workOrder);
        await _fixture.DbContext.SaveChangesAsync();

        var workOrderId = workOrder.Id;
        var toDelete = await repository.GetByIdAsync(workOrderId);
        toDelete.Should().NotBeNull();

        repository.Remove(toDelete!);
        await _fixture.DbContext.SaveChangesAsync();

        var deleted = await repository.GetByIdAsync(workOrderId);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task CreateMultipleWorkOrders_ShouldMaintainDataIntegrity()
    {
        _fixture.ResetDatabase();
        var repository = new WorkOrderRepository(_fixture.DbContext);

        var customerId1 = Guid.NewGuid();
        var vehicleId1 = Guid.NewGuid();
        var customerId2 = Guid.NewGuid();
        var vehicleId2 = Guid.NewGuid();

        var workOrder1 = new WorkOrder(customerId1, vehicleId1, "Revisão 1");
        var workOrder2 = new WorkOrder(customerId2, vehicleId2, "Revisão 2");

        await repository.AddAsync(workOrder1);
        await repository.AddAsync(workOrder2);
        await _fixture.DbContext.SaveChangesAsync();

        var all = await repository.GetAllAsync();

        all.Should().HaveCount(2);
        all.FirstOrDefault(w => w.CustomerId == customerId1).Should().NotBeNull();
        all.FirstOrDefault(w => w.CustomerId == customerId2).Should().NotBeNull();
    }
}
