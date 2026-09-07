using FluentAssertions;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Infrastructure.Repositories;
using Xunit;

namespace GarageFlowService.Tests.Integration;

public class PartRepositoryIntegrationTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;

    public PartRepositoryIntegrationTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreatePart_ShouldPersistAndRetrieve()
    {
        var repository = new PartRepository(_fixture.DbContext);
        var part = new Part("Óleo 5W-30", "Óleo de motor", 85.50m, 10);

        await repository.AddAsync(part);
        await _fixture.DbContext.SaveChangesAsync();

        var retrieved = await repository.GetByIdAsync(part.Id);

        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be("Óleo 5W-30");
        retrieved.Price.Should().Be(85.50m);
        retrieved.StockQuantity.Should().Be(10);
        retrieved.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllParts()
    {
        _fixture.ResetDatabase();
        var repository = new PartRepository(_fixture.DbContext);

        var part1 = new Part("Óleo 5W-30", "Óleo de motor", 85.50m, 10);
        var part2 = new Part("Filtro de ar", "Filtro para motor", 45.00m, 5);

        await repository.AddAsync(part1);
        await repository.AddAsync(part2);
        await _fixture.DbContext.SaveChangesAsync();

        var result = await repository.GetAllAsync();

        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Name == "Óleo 5W-30");
        result.Should().Contain(p => p.Name == "Filtro de ar");
    }

    [Fact]
    public async Task UpdatePartStock_ShouldPersistChanges()
    {
        _fixture.ResetDatabase();
        var repository = new PartRepository(_fixture.DbContext);
        var part = new Part("Óleo 5W-30", "Óleo de motor", 85.50m, 10);

        await repository.AddAsync(part);
        await _fixture.DbContext.SaveChangesAsync();

        var partId = part.Id;
        var retrieved = await repository.GetByIdAsync(partId);
        retrieved!.UpdateStock(5);
        repository.Update(retrieved);
        await _fixture.DbContext.SaveChangesAsync();

        var updated = await repository.GetByIdAsync(partId);
        updated!.StockQuantity.Should().Be(15);
    }

    [Fact]
    public async Task DeactivatePart_ShouldPersistInactiveStatus()
    {
        _fixture.ResetDatabase();
        var repository = new PartRepository(_fixture.DbContext);
        var part = new Part("Óleo 5W-30", "Óleo de motor", 85.50m, 10);

        await repository.AddAsync(part);
        await _fixture.DbContext.SaveChangesAsync();

        var partId = part.Id;
        var retrieved = await repository.GetByIdAsync(partId);
        retrieved!.Deactivate();
        repository.Update(retrieved);
        await _fixture.DbContext.SaveChangesAsync();

        var deactivated = await repository.GetByIdAsync(partId);
        deactivated!.IsActive.Should().BeFalse();
    }
}
