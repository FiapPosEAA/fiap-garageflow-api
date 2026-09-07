using FluentAssertions;
using Moq;
using GarageFlowService.Application.UseCases.Services;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Domain.Interfaces;
using GarageFlowService.Application.Interfaces;

namespace GarageFlowService.Tests.Application.Services;

public class CreateServiceCommandTests
{
    private readonly Mock<IServiceRepository> _serviceRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateServiceHandler _handler;

    public CreateServiceCommandTests()
    {
        _serviceRepositoryMock = new Mock<IServiceRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CreateServiceHandler(_serviceRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateService()
    {
        var command = new CreateServiceCommand(
            "Troca de óleo", 
            "Troca de óleo do motor", 
            150m, 
            1m
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be("Troca de óleo");
        result.Price.Should().Be(150m);
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCallRepository()
    {
        var command = new CreateServiceCommand(
            "Troca de óleo", 
            "Troca de óleo do motor", 
            150m, 
            1m
        );

        await _handler.Handle(command, CancellationToken.None);

        _serviceRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Service>(), It.IsAny<CancellationToken>()), 
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCallUnitOfWorkCommit()
    {
        var command = new CreateServiceCommand(
            "Troca de óleo", 
            "Troca de óleo do motor", 
            150m, 
            1m
        );

        await _handler.Handle(command, CancellationToken.None);

        _unitOfWorkMock.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()), 
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_MultipleServices_ShouldCreateWithDifferentIds()
    {
        var command1 = new CreateServiceCommand("Serviço 1", "desc1", 100m, 1m);
        var command2 = new CreateServiceCommand("Serviço 2", "desc2", 200m, 2m);

        var result1 = await _handler.Handle(command1, CancellationToken.None);
        var result2 = await _handler.Handle(command2, CancellationToken.None);

        result1.Id.Should().NotBe(result2.Id);
    }
}
