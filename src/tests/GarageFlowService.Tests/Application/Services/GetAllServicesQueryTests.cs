using FluentAssertions;
using Moq;
using GarageFlowService.Application.UseCases.Services;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Domain.Interfaces;

namespace GarageFlowService.Tests.Application.Services;

public class GetAllServicesQueryTests
{
    private readonly Mock<IServiceRepository> _serviceRepositoryMock;
    private readonly GetAllServicesHandler _handler;

    public GetAllServicesQueryTests()
    {
        _serviceRepositoryMock = new Mock<IServiceRepository>();
        _handler = new GetAllServicesHandler(_serviceRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenServicesExist_ShouldReturnAll()
    {
        var services = new List<Service>
        {
            new Service("Serviço 1", "desc1", 100m, 1m),
            new Service("Serviço 2", "desc2", 200m, 2m),
            new Service("Serviço 3", "desc3", 300m, 3m)
        };

        _serviceRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(services);

        var query = new GetAllServicesQuery();
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task Handle_WhenNoServicesExist_ShouldReturnEmptyList()
    {
        _serviceRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Service>());

        var query = new GetAllServicesQuery();
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldReturnDtoList()
    {
        var services = new List<Service>
        {
            new Service("Troca de óleo", "desc", 150m, 1m)
        };

        _serviceRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(services);

        var query = new GetAllServicesQuery();
        var result = await _handler.Handle(query, CancellationToken.None);

        result.First().Name.Should().Be("Troca de óleo");
    }
}
