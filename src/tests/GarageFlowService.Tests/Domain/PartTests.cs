using FluentAssertions;
using GarageFlowService.Domain.Entities;

namespace GarageFlowService.Tests.Domain;

public class PartTests
{
    [Fact]
    public void Create_Part_ShouldSetPropertiesCorrectly()
    {
        var part = new Part("Filtro de óleo", "Filtro BOSCH", 45.90m, 100);

        part.Name.Should().Be("Filtro de óleo");
        part.Price.Should().Be(45.90m);
        part.StockQuantity.Should().Be(100);
        part.IsActive.Should().BeTrue();
    }

    [Fact]
    public void UpdateStock_ShouldAddQuantity()
    {
        var part = new Part("Filtro de óleo", "Filtro BOSCH", 45.90m, 10);
        part.UpdateStock(5);
        part.StockQuantity.Should().Be(15);
    }

    [Fact]
    public void UpdateStock_BelowZero_ShouldThrow()
    {
        var part = new Part("Filtro de óleo", "Filtro BOSCH", 45.90m, 5);
        var act = () => part.UpdateStock(-10);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Create_Part_WithNegativePrice_ShouldThrow()
    {
        var act = () => new Part("Filtro", "desc", -10m, 5);
        act.Should().Throw<ArgumentException>();
    }
}

