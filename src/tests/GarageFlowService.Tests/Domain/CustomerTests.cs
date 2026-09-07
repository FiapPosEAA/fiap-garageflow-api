using FluentAssertions;
using GarageFlowService.Domain.Entities;

namespace GarageFlowService.Tests.Domain;

public class CustomerTests
{
    [Fact]
    public void Create_Customer_ShouldSetPropertiesCorrectly()
    {
        var customer = new Customer("João Silva", "joao@email.com", "11999999999", "12345678901");

        customer.Name.Should().Be("João Silva");
        customer.Email.Should().Be("joao@email.com");
        customer.Document.Should().Be("12345678901");
        customer.Id.Should().NotBeEmpty();
        customer.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Create_Customer_WithEmptyName_ShouldThrow()
    {
        var act = () => new Customer("", "joao@email.com", "11999999999", "12345678901");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Update_Customer_ShouldUpdateProperties()
    {
        var customer = new Customer("João Silva", "joao@email.com", "11999999999", "12345678901");
        customer.Update("João Souza", "joaosouza@email.com", "11988888888");

        customer.Name.Should().Be("João Souza");
        customer.Email.Should().Be("joaosouza@email.com");
    }
}

