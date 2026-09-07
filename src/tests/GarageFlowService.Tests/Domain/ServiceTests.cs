using FluentAssertions;
using GarageFlowService.Domain.Entities;

namespace GarageFlowService.Tests.Domain;

public class ServiceTests
{
    [Fact]
    public void Create_Service_ShouldSetPropertiesCorrectly()
    {
        var service = new Service("Troca de óleo", "Troca de óleo do motor", 150m, 1m);

        service.Name.Should().Be("Troca de óleo");
        service.Description.Should().Be("Troca de óleo do motor");
        service.Price.Should().Be(150m);
        service.EstimatedHours.Should().Be(1m);
        service.IsActive.Should().BeTrue();
        service.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_Service_WithEmptyName_ShouldThrow()
    {
        var act = () => new Service("", "description", 150m, 1m);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_Service_WithNegativePrice_ShouldThrow()
    {
        var act = () => new Service("Troca de óleo", "description", -50m, 1m);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_Service_WithZeroPrice_ShouldSucceed()
    {
        var service = new Service("Serviço Grátis", "descrição", 0m, 1m);
        service.Price.Should().Be(0m);
    }

    [Fact]
    public void Update_Service_ShouldUpdateAllProperties()
    {
        var service = new Service("Troca de óleo", "Descrição antiga", 150m, 1m);
        
        service.Update("Troca de óleo sintético", "Descrição nova", 200m, 1.5m);

        service.Name.Should().Be("Troca de óleo sintético");
        service.Description.Should().Be("Descrição nova");
        service.Price.Should().Be(200m);
        service.EstimatedHours.Should().Be(1.5m);
    }

    [Fact]
    public void Update_Service_WithEmptyName_ShouldThrow()
    {
        var service = new Service("Troca de óleo", "description", 150m, 1m);
        var act = () => service.Update("", "new desc", 200m, 1m);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deactivate_Service_ShouldSetIsActiveToFalse()
    {
        var service = new Service("Serviço", "desc", 100m, 1m);
        
        service.Deactivate();
        
        service.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_Service_ShouldSetIsActiveToTrue()
    {
        var service = new Service("Serviço", "desc", 100m, 1m);
        service.Deactivate();
        
        service.Activate();
        
        service.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_Service_WithWhitespaceOnlyName_ShouldThrow()
    {
        var act = () => new Service("   ", "description", 100m, 1m);
        act.Should().Throw<ArgumentException>();
    }
}
