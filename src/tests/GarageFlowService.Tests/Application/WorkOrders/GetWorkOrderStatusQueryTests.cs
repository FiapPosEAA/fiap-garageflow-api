using FluentAssertions;
using GarageFlowService.Application.UseCases.WorkOrders;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Domain.Enums;
using GarageFlowService.Domain.Interfaces;
using Moq;

namespace GarageFlowService.Tests.Application.WorkOrders;

public class GetWorkOrderStatusQueryTests
{
    private readonly Mock<IWorkOrderRepository> _workOrderRepositoryMock;
    private readonly GetWorkOrderStatusHandler _handler;

    public GetWorkOrderStatusQueryTests()
    {
        _workOrderRepositoryMock = new Mock<IWorkOrderRepository>();
        _handler = new GetWorkOrderStatusHandler(_workOrderRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenWorkOrderExists_ShouldReturnStatusSummary()
    {
        var workOrderId = Guid.NewGuid();
        var workOrder = new WorkOrder(Guid.NewGuid(), Guid.NewGuid(), "Revisão");
        workOrder.UpdateStatus(WorkOrderStatus.Diagnosis);

        _workOrderRepositoryMock
            .Setup(x => x.GetByIdWithDetailsAsync(workOrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workOrder);

        var result = await _handler.Handle(new GetWorkOrderStatusQuery(workOrderId), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(workOrder.Id);
        result.Status.Should().Be(WorkOrderStatus.Diagnosis);
        result.StatusDescription.Should().Be(WorkOrderStatus.Diagnosis.ToString());
    }

    [Fact]
    public async Task Handle_WhenWorkOrderDoesNotExist_ShouldReturnNull()
    {
        _workOrderRepositoryMock
            .Setup(x => x.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkOrder?)null);

        var result = await _handler.Handle(new GetWorkOrderStatusQuery(Guid.NewGuid()), CancellationToken.None);

        result.Should().BeNull();
    }
}