using FluentAssertions;
using Moq;
using GarageFlowService.Application.UseCases.Parts;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Domain.Interfaces;
using GarageFlowService.Application.Interfaces;

namespace GarageFlowService.Tests.Application.Parts;

public class UpdatePartStockCommandTests
{
    private readonly Mock<IPartRepository> _partRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UpdatePartStockHandler _handler;

    public UpdatePartStockCommandTests()
    {
        _partRepositoryMock = new Mock<IPartRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new UpdatePartStockHandler(_partRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldUpdateStock()
    {
        var partId = Guid.NewGuid();
        var part = new Part("Filtro", "Filtro BOSCH", 45.90m, 10);
        
        _partRepositoryMock
            .Setup(x => x.GetByIdAsync(partId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(part);

        var command = new UpdatePartStockCommand(partId, 5);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result!.StockQuantity.Should().Be(15);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCallUnitOfWorkCommit()
    {
        var partId = Guid.NewGuid();
        var part = new Part("Filtro", "Filtro BOSCH", 45.90m, 10);
        
        _partRepositoryMock
            .Setup(x => x.GetByIdAsync(partId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(part);

        var command = new UpdatePartStockCommand(partId, 5);
        await _handler.Handle(command, CancellationToken.None);

        _unitOfWorkMock.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()), 
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_PartDoesNotExist_ShouldReturnNull()
    {
        var partId = Guid.NewGuid();
        
        _partRepositoryMock
            .Setup(x => x.GetByIdAsync(partId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Part?)null);

        var command = new UpdatePartStockCommand(partId, 5);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
    }
}
