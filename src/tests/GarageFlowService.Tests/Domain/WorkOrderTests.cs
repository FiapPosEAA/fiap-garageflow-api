using FluentAssertions;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Domain.Enums;
using GarageFlowService.Domain.Exceptions;

namespace GarageFlowService.Tests.Domain;

public class WorkOrderTests
{
    private static WorkOrder CreateWorkOrder() =>
        new(Guid.NewGuid(), Guid.NewGuid(), "Troca de óleo");

    private static Service CreateService(decimal price = 150m) =>
        new("Troca de óleo", "Troca de óleo do motor", price, 1m);

    private static Part CreatePart(decimal price = 50m) =>
        new("Filtro de óleo", "Filtro de óleo BOSCH", price, 10);

    [Fact]
    public void Create_WorkOrder_ShouldHaveReceivedStatus()
    {
        var wo = CreateWorkOrder();
        wo.Status.Should().Be(WorkOrderStatus.Received);
        wo.OrderNumber.Should().StartWith("OS-");
        wo.TotalAmount.Should().Be(0);
    }

    [Fact]
    public void UpdateStatus_ValidTransition_ShouldSucceed()
    {
        var wo = CreateWorkOrder();
        wo.UpdateStatus(WorkOrderStatus.Diagnosis);
        wo.Status.Should().Be(WorkOrderStatus.Diagnosis);
    }

    [Fact]
    public void UpdateStatus_InvalidTransition_ShouldThrow()
    {
        var wo = CreateWorkOrder();
        var act = () => wo.UpdateStatus(WorkOrderStatus.Finished);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AddService_ShouldCalculateTotal()
    {
        var wo = CreateWorkOrder();
        var service = CreateService(150m);

        wo.AddService(service, 1);

        wo.TotalAmount.Should().Be(150m);
        wo.WorkOrderServices.Should().HaveCount(1);
    }

    [Fact]
    public void AddPart_ShouldCalculateTotal()
    {
        var wo = CreateWorkOrder();
        var part = CreatePart(50m);

        wo.AddPart(part, 2);

        wo.TotalAmount.Should().Be(100m);
        wo.WorkOrderParts.Should().HaveCount(1);
    }

    [Fact]
    public void AddServiceAndPart_ShouldCalculateTotalCorrectly()
    {
        var wo = CreateWorkOrder();
        var service = CreateService(150m);
        var part = CreatePart(50m);

        wo.AddService(service, 1);
        wo.AddPart(part, 2);

        wo.TotalAmount.Should().Be(250m);
    }

    [Fact]
    public void AddService_Duplicate_ShouldThrow()
    {
        var wo = CreateWorkOrder();
        var service = CreateService();

        wo.AddService(service, 1);
        var act = () => wo.AddService(service, 1);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AddService_ToDeliveredWorkOrder_ShouldThrow()
    {
        var wo = CreateWorkOrder();
        wo.UpdateStatus(WorkOrderStatus.Diagnosis);
        wo.UpdateStatus(WorkOrderStatus.WaitingApproval);
        wo.UpdateStatus(WorkOrderStatus.InProgress);
        wo.UpdateStatus(WorkOrderStatus.Finished);
        wo.UpdateStatus(WorkOrderStatus.Delivered);

        var service = CreateService();
        var act = () => wo.AddService(service, 1);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void GenerateBudget_ShouldReturnCorrectBudget()
    {
        var wo = CreateWorkOrder();
        var service = CreateService(200m);
        var part = CreatePart(75m);

        wo.AddService(service, 1);
        wo.AddPart(part, 3);

        var budget = wo.GenerateBudget();

        budget.Total.Should().Be(425m);
        budget.Services.Should().HaveCount(1);
        budget.Parts.Should().HaveCount(1);
    }

    [Fact]
    public void StatusTransition_FullLifecycle_ShouldSucceed()
    {
        var wo = CreateWorkOrder();
        wo.Status.Should().Be(WorkOrderStatus.Received);

        wo.UpdateStatus(WorkOrderStatus.Diagnosis);
        wo.Status.Should().Be(WorkOrderStatus.Diagnosis);

        wo.UpdateStatus(WorkOrderStatus.WaitingApproval);
        wo.Status.Should().Be(WorkOrderStatus.WaitingApproval);

        wo.UpdateStatus(WorkOrderStatus.InProgress);
        wo.Status.Should().Be(WorkOrderStatus.InProgress);

        wo.UpdateStatus(WorkOrderStatus.Finished);
        wo.Status.Should().Be(WorkOrderStatus.Finished);

        wo.UpdateStatus(WorkOrderStatus.Delivered);
        wo.Status.Should().Be(WorkOrderStatus.Delivered);
    }
}

