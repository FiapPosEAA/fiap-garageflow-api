using FluentAssertions;
using Moq;
using GarageFlowService.Application.UseCases.Customers;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Domain.Interfaces;

namespace GarageFlowService.Tests.Application.Customers;

public class GetAllCustomersQueryTests
{
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly GetAllCustomersHandler _handler;

    public GetAllCustomersQueryTests()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _handler = new GetAllCustomersHandler(_customerRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenCustomersExist_ShouldReturnAll()
    {
        var customers = new List<Customer>
        {
            new Customer("Cliente 1", "cliente1@email.com", "11111111111", "11111111111"),
            new Customer("Cliente 2", "cliente2@email.com", "22222222222", "22222222222"),
            new Customer("Cliente 3", "cliente3@email.com", "33333333333", "33333333333")
        };

        _customerRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(customers);

        var query = new GetAllCustomersQuery();
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().HaveCount(3);
        result.Should().AllSatisfy(c => c.Should().NotBeNull());
    }

    [Fact]
    public async Task Handle_WhenNoCustomersExist_ShouldReturnEmptyList()
    {
        _customerRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Customer>());

        var query = new GetAllCustomersQuery();
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldCallRepository()
    {
        _customerRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Customer>());

        var query = new GetAllCustomersQuery();
        await _handler.Handle(query, CancellationToken.None);

        _customerRepositoryMock.Verify(
            x => x.GetAllAsync(It.IsAny<CancellationToken>()), 
            Times.Once
        );
    }
}
