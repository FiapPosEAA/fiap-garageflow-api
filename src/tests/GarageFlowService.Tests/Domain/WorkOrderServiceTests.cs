using FluentAssertions;
using GarageFlowService.Domain.Entities;

namespace GarageFlowService.Tests.Domain;

public class WorkOrderServiceTests
{
    [Fact]
    public void Create_WorkOrderService_ShouldSetPropertiesCorrectly()
    {
        var workOrderId = Guid.NewGuid();
        var serviceId = Guid.NewGuid();
        
        var wos = new WorkOrderService(workOrderId, serviceId, 2, 150m);

        wos.WorkOrderId.Should().Be(workOrderId);
        wos.ServiceId.Should().Be(serviceId);
        wos.Quantity.Should().Be(2);
        wos.UnitPrice.Should().Be(150m);
        wos.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Subtotal_ShouldCalculateQuantityTimesUnitPrice()
    {
        var wos = new WorkOrderService(Guid.NewGuid(), Guid.NewGuid(), 3, 100m);
        
        wos.Subtotal.Should().Be(300m);
    }

    [Fact]
    public void Create_WorkOrderService_WithZeroQuantity_ShouldThrow()
    {
        var act = () => new WorkOrderService(Guid.NewGuid(), Guid.NewGuid(), 0, 150m);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WorkOrderService_WithNegativeQuantity_ShouldThrow()
    {
        var act = () => new WorkOrderService(Guid.NewGuid(), Guid.NewGuid(), -1, 150m);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WorkOrderService_WithNegativeUnitPrice_ShouldThrow()
    {
        var act = () => new WorkOrderService(Guid.NewGuid(), Guid.NewGuid(), 1, -50m);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WorkOrderService_WithZeroUnitPrice_ShouldSucceed()
    {
        var wos = new WorkOrderService(Guid.NewGuid(), Guid.NewGuid(), 1, 0m);
        wos.UnitPrice.Should().Be(0m);
    }

    [Fact]
    public void Subtotal_WithDecimalPrices_ShouldCalculateCorrectly()
    {
        var wos = new WorkOrderService(Guid.NewGuid(), Guid.NewGuid(), 2, 75.50m);
        
        wos.Subtotal.Should().Be(151.00m);
    }
}
