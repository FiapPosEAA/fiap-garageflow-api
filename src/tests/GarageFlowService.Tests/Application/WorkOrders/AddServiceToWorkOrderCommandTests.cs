using FluentAssertions;
using Moq;
using GarageFlowService.Application.UseCases.WorkOrders;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Domain.Exceptions;
using GarageFlowService.Domain.Interfaces;
using GarageFlowService.Application.Interfaces;

namespace GarageFlowService.Tests.Application.WorkOrders;

public class AddServiceToWorkOrderCommandTests
{
    private readonly Mock<IWorkOrderRepository> _workOrderRepositoryMock;
    private readonly Mock<IServiceRepository> _serviceRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly AddServiceToWorkOrderHandler _handler;

    public AddServiceToWorkOrderCommandTests()
    {
        _workOrderRepositoryMock = new Mock<IWorkOrderRepository>();
        _serviceRepositoryMock = new Mock<IServiceRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        
        _handler = new AddServiceToWorkOrderHandler(
            _workOrderRepositoryMock.Object,
            _serviceRepositoryMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldAddServiceToWorkOrder()
    {
        var workOrderId = Guid.NewGuid();
        var serviceId = Guid.NewGuid();
        
        var workOrder = new WorkOrder(Guid.NewGuid(), Guid.NewGuid(), "Revisão");
        var service = new Service("Troca de óleo", "desc", 150m, 1m);
        
        _workOrderRepositoryMock
            .Setup(x => x.GetByIdWithDetailsAsync(workOrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workOrder);
        
        _serviceRepositoryMock
            .Setup(x => x.GetByIdAsync(serviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(service);

        _workOrderRepositoryMock
            .Setup(x => x.AddServiceToWorkOrderAsync(workOrder, service, 1, It.IsAny<CancellationToken>()))
            .Callback(() => workOrder.AddService(service, 1))
            .Returns(Task.CompletedTask);

        var command = new AddServiceToWorkOrderCommand(workOrderId, serviceId, 1);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result!.TotalAmount.Should().Be(150m);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCallUnitOfWorkCommit()
    {
        var workOrderId = Guid.NewGuid();
        var serviceId = Guid.NewGuid();
        
        var workOrder = new WorkOrder(Guid.NewGuid(), Guid.NewGuid(), "Revisão");
        var service = new Service("Troca de óleo", "desc", 150m, 1m);
        
        _workOrderRepositoryMock
            .Setup(x => x.GetByIdWithDetailsAsync(workOrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workOrder);
        
        _serviceRepositoryMock
            .Setup(x => x.GetByIdAsync(serviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(service);

        _workOrderRepositoryMock
            .Setup(x => x.AddServiceToWorkOrderAsync(workOrder, service, 1, It.IsAny<CancellationToken>()))
            .Callback(() => workOrder.AddService(service, 1))
            .Returns(Task.CompletedTask);

        var command = new AddServiceToWorkOrderCommand(workOrderId, serviceId, 1);
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
        var serviceId = Guid.NewGuid();
        
        _workOrderRepositoryMock
            .Setup(x => x.GetByIdWithDetailsAsync(workOrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkOrder?)null);

        var command = new AddServiceToWorkOrderCommand(workOrderId, serviceId, 1);
        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }
}
