using FluentAssertions;
using Moq;
using GarageFlowService.Application.UseCases.Customers;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Domain.Interfaces;
using GarageFlowService.Application.Interfaces;

namespace GarageFlowService.Tests.Application.Customers;

public class UpdateCustomerCommandTests
{
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UpdateCustomerHandler _handler;

    public UpdateCustomerCommandTests()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new UpdateCustomerHandler(_customerRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldUpdateCustomer()
    {
        var customerId = Guid.NewGuid();
        var customer = new Customer("João Silva", "joao@email.com", "11999999999", "12345678901");
        
        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        var command = new UpdateCustomerCommand(customerId, "João Souza", "joaosouza@email.com", "11988888888");
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Name.Should().Be("João Souza");
        result.Email.Should().Be("joaosouza@email.com");
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCallUnitOfWorkCommit()
    {
        var customerId = Guid.NewGuid();
        var customer = new Customer("João Silva", "joao@email.com", "11999999999", "12345678901");
        
        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        var command = new UpdateCustomerCommand(customerId, "João Souza", "joaosouza@email.com", "11988888888");
        await _handler.Handle(command, CancellationToken.None);

        _unitOfWorkMock.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()), 
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_CustomerDoesNotExist_ShouldReturnNull()
    {
        var customerId = Guid.NewGuid();
        
        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        var command = new UpdateCustomerCommand(customerId, "João Souza", "joaosouza@email.com", "11988888888");
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
    }
}
