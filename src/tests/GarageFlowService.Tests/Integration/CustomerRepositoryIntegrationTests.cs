using FluentAssertions;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Infrastructure.Repositories;
using Xunit;

namespace GarageFlowService.Tests.Integration;

public class CustomerRepositoryIntegrationTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;

    public CustomerRepositoryIntegrationTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task AddCustomer_ShouldPersistAndRetrieve()
    {
        var repository = new CustomerRepository(_fixture.DbContext);
        var customer = new Customer("João Silva", "joao@email.com", "11987654321", "12345678901");

        await repository.AddAsync(customer);
        await _fixture.DbContext.SaveChangesAsync();

        var retrieved = await repository.GetByIdAsync(customer.Id);

        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be("João Silva");
        retrieved.Email.Should().Be("joao@email.com");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllCustomers()
    {
        _fixture.ResetDatabase();
        var repository = new CustomerRepository(_fixture.DbContext);

        var customer1 = new Customer("João", "joao@email.com", "11987654321", "12345678901");
        var customer2 = new Customer("Maria", "maria@email.com", "11912345678", "98765432101");

        await repository.AddAsync(customer1);
        await repository.AddAsync(customer2);
        await _fixture.DbContext.SaveChangesAsync();

        var result = await repository.GetAllAsync();

        result.Should().HaveCount(2);
        result.Should().Contain(c => c.Name == "João");
        result.Should().Contain(c => c.Name == "Maria");
    }

    [Fact]
    public async Task DeleteCustomer_ShouldRemoveFromDatabase()
    {
        _fixture.ResetDatabase();
        var repository = new CustomerRepository(_fixture.DbContext);
        var customer = new Customer("João", "joao@email.com", "11987654321", "12345678901");

        await repository.AddAsync(customer);
        await _fixture.DbContext.SaveChangesAsync();

        var customerId = customer.Id;
        var toDelete = await repository.GetByIdAsync(customerId);
        toDelete.Should().NotBeNull();

        repository.Remove(toDelete!);
        await _fixture.DbContext.SaveChangesAsync();

        var deleted = await repository.GetByIdAsync(customerId);
        deleted.Should().BeNull();
    }
}

