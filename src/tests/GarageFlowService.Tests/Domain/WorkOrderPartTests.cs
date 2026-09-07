using FluentAssertions;
using GarageFlowService.Domain.Entities;

namespace GarageFlowService.Tests.Domain;

public class WorkOrderPartTests
{
    [Fact]
    public void Create_WorkOrderPart_ShouldSetPropertiesCorrectly()
    {
        var workOrderId = Guid.NewGuid();
        var partId = Guid.NewGuid();
        
        var wop = new WorkOrderPart(workOrderId, partId, 5, 75.50m);

        wop.WorkOrderId.Should().Be(workOrderId);
        wop.PartId.Should().Be(partId);
        wop.Quantity.Should().Be(5);
        wop.UnitPrice.Should().Be(75.50m);
        wop.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Subtotal_ShouldCalculateQuantityTimesUnitPrice()
    {
        var wop = new WorkOrderPart(Guid.NewGuid(), Guid.NewGuid(), 4, 50m);
        
        wop.Subtotal.Should().Be(200m);
    }

    [Fact]
    public void Create_WorkOrderPart_WithZeroQuantity_ShouldThrow()
    {
        var act = () => new WorkOrderPart(Guid.NewGuid(), Guid.NewGuid(), 0, 75.50m);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WorkOrderPart_WithNegativeQuantity_ShouldThrow()
    {
        var act = () => new WorkOrderPart(Guid.NewGuid(), Guid.NewGuid(), -5, 75.50m);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WorkOrderPart_WithNegativeUnitPrice_ShouldThrow()
    {
        var act = () => new WorkOrderPart(Guid.NewGuid(), Guid.NewGuid(), 1, -75.50m);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WorkOrderPart_WithZeroUnitPrice_ShouldSucceed()
    {
        var wop = new WorkOrderPart(Guid.NewGuid(), Guid.NewGuid(), 1, 0m);
        wop.UnitPrice.Should().Be(0m);
    }

    [Fact]
    public void Subtotal_WithLargeQuantities_ShouldCalculateCorrectly()
    {
        var wop = new WorkOrderPart(Guid.NewGuid(), Guid.NewGuid(), 100, 25.75m);
        
        wop.Subtotal.Should().Be(2575m);
    }

    [Fact]
    public void Subtotal_WithDecimalQuantity_ShouldCalculateCorrectly()
    {
        var wop = new WorkOrderPart(Guid.NewGuid(), Guid.NewGuid(), 3, 33.33m);
        
        wop.Subtotal.Should().Be(99.99m);
    }
}
