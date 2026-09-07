using FluentAssertions;
using Moq;
using GarageFlowService.Application.UseCases.WorkOrders;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Domain.Enums;
using GarageFlowService.Domain.Interfaces;
using System.Reflection;

namespace GarageFlowService.Tests.Application.WorkOrders;

public class GetAllWorkOrdersQueryTests
{
    private readonly Mock<IWorkOrderRepository> _workOrderRepositoryMock;
    private readonly GetAllWorkOrdersHandler _handler;

    public GetAllWorkOrdersQueryTests()
    {
        _workOrderRepositoryMock = new Mock<IWorkOrderRepository>();
        _handler = new GetAllWorkOrdersHandler(_workOrderRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_NoWorkOrders_ShouldReturnEmptyList()
    {
        var workOrders = new List<WorkOrder>();
        
        _workOrderRepositoryMock
            .Setup(x => x.GetAllWithDetailsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(workOrders);

        var query = new GetAllWorkOrdersQuery();
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_MultipleWorkOrders_ShouldReturnAllWorkOrders()
    {
        var customerId1 = Guid.NewGuid();
        var vehicleId1 = Guid.NewGuid();
        var customerId2 = Guid.NewGuid();
        var vehicleId2 = Guid.NewGuid();

        var workOrder1 = new WorkOrder(customerId1, vehicleId1, "Revisão 1");
        var workOrder2 = new WorkOrder(customerId2, vehicleId2, "Revisão 2");
        var workOrders = new List<WorkOrder> { workOrder1, workOrder2 };
        
        _workOrderRepositoryMock
            .Setup(x => x.GetAllWithDetailsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(workOrders);

        var query = new GetAllWorkOrdersQuery();
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().HaveCount(2);
        result.Should().AllSatisfy(dto => dto.Should().NotBeNull());
    }

    [Fact]
    public async Task Handle_WorkOrdersWithServices_ShouldMapCorrectly()
    {
        var customerId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        var workOrder = new WorkOrder(customerId, vehicleId, "Revisão completa");
        
        var service = new Service("Troca de óleo", "desc", 150m, 1m);
        workOrder.AddService(service, 1);
        
        var workOrders = new List<WorkOrder> { workOrder };
        
        _workOrderRepositoryMock
            .Setup(x => x.GetAllWithDetailsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(workOrders);

        var query = new GetAllWorkOrdersQuery();
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().HaveCount(1);
        var dto = result.First();
        dto.Description.Should().Be("Revisão completa");
    }

    [Fact]
    public async Task Handle_ShouldFilterFinishedAndDeliveredWorkOrdersAndOrderByStatusThenCreationDate()
    {
        var customerId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();

        var receivedOldest = new WorkOrder(customerId, vehicleId, "Recebida antiga");
        receivedOldest.UpdateStatus(WorkOrderStatus.Diagnosis);
        receivedOldest.UpdateStatus(WorkOrderStatus.WaitingApproval);
        receivedOldest.UpdateStatus(WorkOrderStatus.Received);
        SetCreationDate(receivedOldest, new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc));

        var receivedNewest = new WorkOrder(customerId, vehicleId, "Recebida nova");
        receivedNewest.UpdateStatus(WorkOrderStatus.Diagnosis);
        receivedNewest.UpdateStatus(WorkOrderStatus.WaitingApproval);
        receivedNewest.UpdateStatus(WorkOrderStatus.Received);
        SetCreationDate(receivedNewest, new DateTime(2024, 1, 2, 10, 0, 0, DateTimeKind.Utc));

        var diagnosis = new WorkOrder(customerId, vehicleId, "Diagnóstico");
        diagnosis.UpdateStatus(WorkOrderStatus.Diagnosis);
        SetCreationDate(diagnosis, new DateTime(2024, 1, 3, 10, 0, 0, DateTimeKind.Utc));

        var waitingApproval = new WorkOrder(customerId, vehicleId, "Aguardando aprovação");
        waitingApproval.UpdateStatus(WorkOrderStatus.Diagnosis);
        waitingApproval.UpdateStatus(WorkOrderStatus.WaitingApproval);
        SetCreationDate(waitingApproval, new DateTime(2024, 1, 4, 10, 0, 0, DateTimeKind.Utc));

        var inProgress = new WorkOrder(customerId, vehicleId, "Em execução");
        inProgress.UpdateStatus(WorkOrderStatus.Diagnosis);
        inProgress.UpdateStatus(WorkOrderStatus.WaitingApproval);
        inProgress.UpdateStatus(WorkOrderStatus.InProgress);
        SetCreationDate(inProgress, new DateTime(2024, 1, 5, 10, 0, 0, DateTimeKind.Utc));

        var finished = new WorkOrder(customerId, vehicleId, "Finalizada");
        finished.UpdateStatus(WorkOrderStatus.Diagnosis);
        finished.UpdateStatus(WorkOrderStatus.WaitingApproval);
        finished.UpdateStatus(WorkOrderStatus.InProgress);
        finished.UpdateStatus(WorkOrderStatus.Finished);
        SetCreationDate(finished, new DateTime(2024, 1, 6, 10, 0, 0, DateTimeKind.Utc));

        var delivered = new WorkOrder(customerId, vehicleId, "Entregue");
        delivered.UpdateStatus(WorkOrderStatus.Diagnosis);
        delivered.UpdateStatus(WorkOrderStatus.WaitingApproval);
        delivered.UpdateStatus(WorkOrderStatus.InProgress);
        delivered.UpdateStatus(WorkOrderStatus.Finished);
        delivered.UpdateStatus(WorkOrderStatus.Delivered);
        SetCreationDate(delivered, new DateTime(2024, 1, 7, 10, 0, 0, DateTimeKind.Utc));

        var workOrders = new List<WorkOrder>
        {
            delivered,
            finished,
            receivedNewest,
            waitingApproval,
            receivedOldest,
            diagnosis,
            inProgress
        };

        _workOrderRepositoryMock
            .Setup(x => x.GetAllWithDetailsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(workOrders);

        var result = await _handler.Handle(new GetAllWorkOrdersQuery(), CancellationToken.None);

        result.Should().HaveCount(5);
        result.Select(x => x.Description).Should().ContainInOrder(
            "Em execução",
            "Aguardando aprovação",
            "Diagnóstico",
            "Recebida antiga",
            "Recebida nova");
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryGetAllWithDetailsAsync()
    {
        _workOrderRepositoryMock
            .Setup(x => x.GetAllWithDetailsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<WorkOrder>());

        var query = new GetAllWorkOrdersQuery();
        await _handler.Handle(query, CancellationToken.None);

        _workOrderRepositoryMock.Verify(
            x => x.GetAllWithDetailsAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    private static void SetCreationDate(WorkOrder workOrder, DateTime createdAt)
    {
        var entityType = typeof(Entity);
        entityType.GetProperty(nameof(Entity.CreatedAt), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
            .SetValue(workOrder, createdAt);
        entityType.GetProperty(nameof(Entity.UpdatedAt), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
            .SetValue(workOrder, createdAt);
    }
}
