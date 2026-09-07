using FluentAssertions;
using Moq;
using GarageFlowService.Application.UseCases.Customers;
using GarageFlowService.Application.DTOs;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Domain.Interfaces;
using GarageFlowService.Application.Interfaces;

namespace GarageFlowService.Tests.Application.Customers;

public class CreateCustomerCommandTests
{
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateCustomerHandler _handler;

    public CreateCustomerCommandTests()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CreateCustomerHandler(_customerRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateCustomer()
    {
        var command = new CreateCustomerCommand(
            "João Silva", 
            "joao@email.com", 
            "11999999999", 
            "12345678901"
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be("João Silva");
        result.Email.Should().Be("joao@email.com");
        result.Phone.Should().Be("11999999999");
        result.Document.Should().Be("12345678901");
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCallRepository()
    {
        var command = new CreateCustomerCommand(
            "João Silva", 
            "joao@email.com", 
            "11999999999", 
            "12345678901"
        );

        await _handler.Handle(command, CancellationToken.None);

        _customerRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), 
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCallUnitOfWorkCommit()
    {
        var command = new CreateCustomerCommand(
            "João Silva", 
            "joao@email.com", 
            "11999999999", 
            "12345678901"
        );

        await _handler.Handle(command, CancellationToken.None);

        _unitOfWorkMock.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()), 
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_MultipleCustomers_ShouldCreateWithDifferentIds()
    {
        var command1 = new CreateCustomerCommand("Cliente 1", "cliente1@email.com", "11111111111", "11111111111");
        var command2 = new CreateCustomerCommand("Cliente 2", "cliente2@email.com", "22222222222", "22222222222");

        var result1 = await _handler.Handle(command1, CancellationToken.None);
        var result2 = await _handler.Handle(command2, CancellationToken.None);

        result1.Id.Should().NotBe(result2.Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnCustomerDto()
    {
        var command = new CreateCustomerCommand(
            "João Silva", 
            "joao@email.com", 
            "11999999999", 
            "12345678901"
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeOfType<CustomerDto>();
    }
}
