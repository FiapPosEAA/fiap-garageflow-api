using FluentAssertions;
using Moq;
using GarageFlowService.Application.UseCases.Parts;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Domain.Interfaces;
using GarageFlowService.Application.Interfaces;

namespace GarageFlowService.Tests.Application.Parts;

public class CreatePartCommandTests
{
    private readonly Mock<IPartRepository> _partRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreatePartHandler _handler;

    public CreatePartCommandTests()
    {
        _partRepositoryMock = new Mock<IPartRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CreatePartHandler(_partRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreatePart()
    {
        var command = new CreatePartCommand("Filtro de óleo", "Filtro BOSCH", 45.90m, 100);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be("Filtro de óleo");
        result.Price.Should().Be(45.90m);
        result.StockQuantity.Should().Be(100);
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCallRepository()
    {
        var command = new CreatePartCommand("Filtro de óleo", "Filtro BOSCH", 45.90m, 100);

        await _handler.Handle(command, CancellationToken.None);

        _partRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Part>(), It.IsAny<CancellationToken>()), 
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCallUnitOfWorkCommit()
    {
        var command = new CreatePartCommand("Filtro de óleo", "Filtro BOSCH", 45.90m, 100);

        await _handler.Handle(command, CancellationToken.None);

        _unitOfWorkMock.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()), 
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_MultiipleParts_ShouldCreateWithDifferentIds()
    {
        var command1 = new CreatePartCommand("Filtro 1", "desc1", 50m, 10);
        var command2 = new CreatePartCommand("Filtro 2", "desc2", 60m, 20);

        var result1 = await _handler.Handle(command1, CancellationToken.None);
        var result2 = await _handler.Handle(command2, CancellationToken.None);

        result1.Id.Should().NotBe(result2.Id);
    }
}
