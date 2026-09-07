using FluentAssertions;
using Moq;
using GarageFlowService.Application.UseCases.WorkOrders;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Domain.Enums;
using GarageFlowService.Domain.Interfaces;
using GarageFlowService.Application.Interfaces;

namespace GarageFlowService.Tests.Application.WorkOrders;

public class UpdateWorkOrderStatusCommandTests
{
    private readonly Mock<IWorkOrderRepository> _workOrderRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UpdateWorkOrderStatusHandler _handler;

    public UpdateWorkOrderStatusCommandTests()
    {
        _workOrderRepositoryMock = new Mock<IWorkOrderRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new UpdateWorkOrderStatusHandler(_workOrderRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidTransition_ShouldUpdateStatus()
    {
        var workOrderId = Guid.NewGuid();
        var workOrder = new WorkOrder(Guid.NewGuid(), Guid.NewGuid(), "Revisão");
        
        _workOrderRepositoryMock
            .Setup(x => x.GetByIdWithDetailsAsync(workOrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workOrder);

        var command = new UpdateWorkOrderStatusCommand(workOrderId, WorkOrderStatus.Diagnosis);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Status.Should().Be(WorkOrderStatus.Diagnosis);
    }

    [Fact]
    public async Task Handle_ValidTransition_ShouldCallUnitOfWorkCommit()
    {
        var workOrderId = Guid.NewGuid();
        var workOrder = new WorkOrder(Guid.NewGuid(), Guid.NewGuid(), "Revisão");
        
        _workOrderRepositoryMock
            .Setup(x => x.GetByIdWithDetailsAsync(workOrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workOrder);

        var command = new UpdateWorkOrderStatusCommand(workOrderId, WorkOrderStatus.Diagnosis);
        await _handler.Handle(command, CancellationToken.None);

        _unitOfWorkMock.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()), 
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WorkOrderDoesNotExist_ShouldReturnNull()
    {
        var workOrderId = Guid.NewGuid();
        
        _workOrderRepositoryMock
            .Setup(x => x.GetByIdWithDetailsAsync(workOrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkOrder?)null);

        var command = new UpdateWorkOrderStatusCommand(workOrderId, WorkOrderStatus.Diagnosis);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
    }
}
