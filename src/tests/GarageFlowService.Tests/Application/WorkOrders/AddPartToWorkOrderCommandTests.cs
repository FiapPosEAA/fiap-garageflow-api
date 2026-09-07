using FluentAssertions;
using Moq;
using GarageFlowService.Application.UseCases.WorkOrders;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Domain.Exceptions;
using GarageFlowService.Domain.Interfaces;
using GarageFlowService.Application.Interfaces;

namespace GarageFlowService.Tests.Application.WorkOrders;

public class AddPartToWorkOrderCommandTests
{
    private readonly Mock<IWorkOrderRepository> _workOrderRepositoryMock;
    private readonly Mock<IPartRepository> _partRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly AddPartToWorkOrderHandler _handler;

    public AddPartToWorkOrderCommandTests()
    {
        _workOrderRepositoryMock = new Mock<IWorkOrderRepository>();
        _partRepositoryMock = new Mock<IPartRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        
        _handler = new AddPartToWorkOrderHandler(
            _workOrderRepositoryMock.Object,
            _partRepositoryMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldAddPartToWorkOrder()
    {
        var workOrderId = Guid.NewGuid();
        var partId = Guid.NewGuid();
        
        var workOrder = new WorkOrder(Guid.NewGuid(), Guid.NewGuid(), "Revisão");
        var part = new Part("Filtro", "Filtro BOSCH", 50m, 100);
        
        _workOrderRepositoryMock
            .Setup(x => x.GetByIdWithDetailsAsync(workOrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workOrder);
        
        _partRepositoryMock
            .Setup(x => x.GetByIdAsync(partId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(part);

        _workOrderRepositoryMock
            .Setup(x => x.AddPartToWorkOrderAsync(workOrder, part, 2, It.IsAny<CancellationToken>()))
            .Callback(() => workOrder.AddPart(part, 2))
            .Returns(Task.CompletedTask);

        var command = new AddPartToWorkOrderCommand(workOrderId, partId, 2);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result!.TotalAmount.Should().Be(100m); // 50 * 2
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCallUnitOfWorkCommit()
    {
        var workOrderId = Guid.NewGuid();
        var partId = Guid.NewGuid();
        
        var workOrder = new WorkOrder(Guid.NewGuid(), Guid.NewGuid(), "Revisão");
        var part = new Part("Filtro", "Filtro BOSCH", 50m, 100);
        
        _workOrderRepositoryMock
            .Setup(x => x.GetByIdWithDetailsAsync(workOrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workOrder);
        
        _partRepositoryMock
            .Setup(x => x.GetByIdAsync(partId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(part);

        _workOrderRepositoryMock
            .Setup(x => x.AddPartToWorkOrderAsync(workOrder, part, 2, It.IsAny<CancellationToken>()))
            .Callback(() => workOrder.AddPart(part, 2))
            .Returns(Task.CompletedTask);

        var command = new AddPartToWorkOrderCommand(workOrderId, partId, 2);
        await _handler.Handle(command, CancellationToken.None);

        _unitOfWorkMock.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()), 
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_PartDoesNotExist_ShouldThrowDomainException()
    {
        var workOrderId = Guid.NewGuid();
        var partId = Guid.NewGuid();
        
        var workOrder = new WorkOrder(Guid.NewGuid(), Guid.NewGuid(), "Revisão");
        
        _workOrderRepositoryMock
            .Setup(x => x.GetByIdWithDetailsAsync(workOrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workOrder);
        
        _partRepositoryMock
            .Setup(x => x.GetByIdAsync(partId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Part?)null);

        var command = new AddPartToWorkOrderCommand(workOrderId, partId, 2);
        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }
}
