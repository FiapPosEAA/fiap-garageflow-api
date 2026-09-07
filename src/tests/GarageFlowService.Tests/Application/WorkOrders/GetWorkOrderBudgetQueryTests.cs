using FluentAssertions;
using Moq;
using GarageFlowService.Application.UseCases.WorkOrders;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Domain.Interfaces;

namespace GarageFlowService.Tests.Application.WorkOrders;

public class GetWorkOrderBudgetQueryTests
{
    private readonly Mock<IWorkOrderRepository> _workOrderRepositoryMock;
    private readonly GetWorkOrderBudgetHandler _handler;

    public GetWorkOrderBudgetQueryTests()
    {
        _workOrderRepositoryMock = new Mock<IWorkOrderRepository>();
        _handler = new GetWorkOrderBudgetHandler(_workOrderRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WorkOrderExists_ShouldReturnBudget()
    {
        var workOrderId = Guid.NewGuid();
        var workOrder = new WorkOrder(Guid.NewGuid(), Guid.NewGuid(), "Revisão");
        
        var service = new Service("Troca de óleo", "desc", 150m, 1m);
        workOrder.AddService(service, 1);
        
        _workOrderRepositoryMock
            .Setup(x => x.GetByIdWithDetailsAsync(workOrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workOrder);

        var query = new GetWorkOrderBudgetQuery(workOrderId);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Total.Should().Be(150m);
    }

    [Fact]
    public async Task Handle_WorkOrderWithMultipleServices_ShouldCalculateTotalCorrectly()
    {
        var workOrderId = Guid.NewGuid();
        var workOrder = new WorkOrder(Guid.NewGuid(), Guid.NewGuid(), "Revisão");
        
        var service1 = new Service("Troca de óleo", "desc", 150m, 1m);
        var service2 = new Service("Filtro", "desc", 50m, 0.5m);
        
        workOrder.AddService(service1, 1);
        workOrder.AddService(service2, 2);
        
        _workOrderRepositoryMock
            .Setup(x => x.GetByIdWithDetailsAsync(workOrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workOrder);

        var query = new GetWorkOrderBudgetQuery(workOrderId);
        var result = await _handler.Handle(query, CancellationToken.None);

        result!.Total.Should().Be(250m); // 150 + (50 * 2)
    }

    [Fact]
    public async Task Handle_WorkOrderDoesNotExist_ShouldReturnNull()
    {
        var workOrderId = Guid.NewGuid();
        
        _workOrderRepositoryMock
            .Setup(x => x.GetByIdAsync(workOrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkOrder?)null);

        var query = new GetWorkOrderBudgetQuery(workOrderId);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeNull();
    }
}
