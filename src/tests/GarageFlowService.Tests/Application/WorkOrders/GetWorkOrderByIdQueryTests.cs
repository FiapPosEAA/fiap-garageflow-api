using FluentAssertions;
using Moq;
using GarageFlowService.Application.UseCases.WorkOrders;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Domain.Interfaces;

namespace GarageFlowService.Tests.Application.WorkOrders;

public class GetWorkOrderByIdQueryTests
{
    private readonly Mock<IWorkOrderRepository> _workOrderRepositoryMock;
    private readonly GetWorkOrderByIdHandler _handler;

    public GetWorkOrderByIdQueryTests()
    {
        _workOrderRepositoryMock = new Mock<IWorkOrderRepository>();
        _handler = new GetWorkOrderByIdHandler(_workOrderRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WorkOrderExists_ShouldReturnWorkOrderDetail()
    {
        var workOrderId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        var workOrder = new WorkOrder(customerId, vehicleId, "Revisão");
        
        _workOrderRepositoryMock
            .Setup(x => x.GetByIdWithDetailsAsync(workOrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workOrder);

        var query = new GetWorkOrderByIdQuery(workOrderId);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result!.CustomerId.Should().Be(customerId);
        result.VehicleId.Should().Be(vehicleId);
        result.Description.Should().Be("Revisão");
    }

    [Fact]
    public async Task Handle_WorkOrderDoesNotExist_ShouldReturnNull()
    {
        var workOrderId = Guid.NewGuid();
        
        _workOrderRepositoryMock
            .Setup(x => x.GetByIdWithDetailsAsync(workOrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkOrder?)null);

        var query = new GetWorkOrderByIdQuery(workOrderId);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WorkOrderWithServicesAndParts_ShouldMapCorrectly()
    {
        var workOrderId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        var workOrder = new WorkOrder(customerId, vehicleId, "Revisão completa");
        
        var service = new Service("Troca de óleo", "desc", 150m, 1m);
        var part = new Part("Óleo sintético", "desc", 80m, 5);
        workOrder.AddService(service, 1);
        workOrder.AddPart(part, 1);
        
        _workOrderRepositoryMock
            .Setup(x => x.GetByIdWithDetailsAsync(workOrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workOrder);

        var query = new GetWorkOrderByIdQuery(workOrderId);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Services.Should().HaveCount(1);
        result.Parts.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_WorkOrderWithoutCustomerAndVehicle_ShouldReturnEmptyStrings()
    {
        var workOrderId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        var workOrder = new WorkOrder(customerId, vehicleId, "Revisão");
        
        _workOrderRepositoryMock
            .Setup(x => x.GetByIdWithDetailsAsync(workOrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workOrder);

        var query = new GetWorkOrderByIdQuery(workOrderId);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result!.CustomerName.Should().Be(string.Empty);
        result.VehicleInfo.Should().Be(string.Empty);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryGetByIdWithDetailsAsync()
    {
        var workOrderId = Guid.NewGuid();
        
        _workOrderRepositoryMock
            .Setup(x => x.GetByIdWithDetailsAsync(workOrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkOrder?)null);

        var query = new GetWorkOrderByIdQuery(workOrderId);
        await _handler.Handle(query, CancellationToken.None);

        _workOrderRepositoryMock.Verify(
            x => x.GetByIdWithDetailsAsync(workOrderId, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
