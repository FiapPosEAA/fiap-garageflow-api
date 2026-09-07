using FluentAssertions;
using GarageFlowService.Application.Interfaces;
using GarageFlowService.Application.UseCases.WorkOrders;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Domain.Enums;
using GarageFlowService.Domain.Interfaces;
using Moq;

namespace GarageFlowService.Tests.Application.WorkOrders;

public class ApproveBudgetCommandTests
{
    private readonly Mock<IWorkOrderRepository> _workOrderRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ApproveBudgetHandler _handler;

    public ApproveBudgetCommandTests()
    {
        _workOrderRepositoryMock = new Mock<IWorkOrderRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new ApproveBudgetHandler(_workOrderRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenApproved_ShouldMoveWorkOrderToInProgress()
    {
        var workOrderId = Guid.NewGuid();
        var workOrder = new WorkOrder(Guid.NewGuid(), Guid.NewGuid(), "Revisão");
        workOrder.UpdateStatus(WorkOrderStatus.Diagnosis);
        workOrder.UpdateStatus(WorkOrderStatus.WaitingApproval);

        _workOrderRepositoryMock
            .Setup(x => x.GetByIdWithDetailsAsync(workOrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workOrder);

        var result = await _handler.Handle(new ApproveBudgetCommand(workOrderId, true), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Status.Should().Be(WorkOrderStatus.InProgress);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRejected_ShouldCancelWorkOrder()
    {
        var workOrderId = Guid.NewGuid();
        var workOrder = new WorkOrder(Guid.NewGuid(), Guid.NewGuid(), "Revisão");
        workOrder.UpdateStatus(WorkOrderStatus.Diagnosis);
        workOrder.UpdateStatus(WorkOrderStatus.WaitingApproval);

        _workOrderRepositoryMock
            .Setup(x => x.GetByIdWithDetailsAsync(workOrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workOrder);

        var result = await _handler.Handle(new ApproveBudgetCommand(workOrderId, false), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Status.Should().Be(WorkOrderStatus.Canceled);
    }

    [Fact]
    public async Task Handle_WhenWorkOrderDoesNotExist_ShouldReturnNull()
    {
        _workOrderRepositoryMock
            .Setup(x => x.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkOrder?)null);

        var result = await _handler.Handle(new ApproveBudgetCommand(Guid.NewGuid(), true), CancellationToken.None);

        result.Should().BeNull();
    }
}