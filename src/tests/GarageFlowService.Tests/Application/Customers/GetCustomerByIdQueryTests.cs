using FluentAssertions;
using Moq;
using GarageFlowService.Application.UseCases.Customers;
using GarageFlowService.Application.DTOs;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Domain.Interfaces;

namespace GarageFlowService.Tests.Application.Customers;

public class GetCustomerByIdQueryTests
{
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly GetCustomerByIdHandler _handler;

    public GetCustomerByIdQueryTests()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _handler = new GetCustomerByIdHandler(_customerRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_CustomerExists_ShouldReturnCustomer()
    {
        var customerId = Guid.NewGuid();
        var customer = new Customer("João Silva", "joao@email.com", "11999999999", "12345678901");
        
        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        var query = new GetCustomerByIdQuery(customerId);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Name.Should().Be("João Silva");
        result.Email.Should().Be("joao@email.com");
    }

    [Fact]
    public async Task Handle_CustomerDoesNotExist_ShouldReturnNull()
    {
        var customerId = Guid.NewGuid();
        
        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        var query = new GetCustomerByIdQuery(customerId);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldCallRepository()
    {
        var customerId = Guid.NewGuid();
        
        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        var query = new GetCustomerByIdQuery(customerId);
        await _handler.Handle(query, CancellationToken.None);

        _customerRepositoryMock.Verify(
            x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()), 
            Times.Once
        );
    }
}
