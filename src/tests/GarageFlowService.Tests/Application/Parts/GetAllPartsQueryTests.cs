using FluentAssertions;
using Moq;
using GarageFlowService.Application.UseCases.Parts;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Domain.Interfaces;

namespace GarageFlowService.Tests.Application.Parts;

public class GetAllPartsQueryTests
{
    private readonly Mock<IPartRepository> _partRepositoryMock;
    private readonly GetAllPartsHandler _handler;

    public GetAllPartsQueryTests()
    {
        _partRepositoryMock = new Mock<IPartRepository>();
        _handler = new GetAllPartsHandler(_partRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenPartsExist_ShouldReturnAll()
    {
        var parts = new List<Part>
        {
            new Part("Filtro 1", "desc1", 50m, 10),
            new Part("Filtro 2", "desc2", 60m, 20),
            new Part("Filtro 3", "desc3", 70m, 30)
        };

        _partRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(parts);

        var query = new GetAllPartsQuery();
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task Handle_WhenNoPartsExist_ShouldReturnEmptyList()
    {
        _partRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Part>());

        var query = new GetAllPartsQuery();
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldReturnDtoList()
    {
        var parts = new List<Part>
        {
            new Part("Filtro", "desc", 50m, 10)
        };

        _partRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(parts);

        var query = new GetAllPartsQuery();
        var result = await _handler.Handle(query, CancellationToken.None);

        result.First().Name.Should().Be("Filtro");
    }
}
