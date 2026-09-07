using FluentAssertions;
using Moq;
using GarageFlowService.Application.UseCases.Vehicles;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Domain.Interfaces;
using GarageFlowService.Application.Interfaces;

namespace GarageFlowService.Tests.Application.Vehicles;

public class CreateVehicleCommandTests
{
    private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateVehicleHandler _handler;

    public CreateVehicleCommandTests()
    {
        _vehicleRepositoryMock = new Mock<IVehicleRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        
        _handler = new CreateVehicleHandler(
            _vehicleRepositoryMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateVehicle()
    {
        var customerId = Guid.NewGuid();

        var command = new CreateVehicleCommand(customerId, "Toyota", "Corolla", 2023, "ABC1234", "Prata");
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Brand.Should().Be("Toyota");
        result.Model.Should().Be("Corolla");
        result.LicensePlate.Should().Be("ABC1234");
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCallRepository()
    {
        var customerId = Guid.NewGuid();

        var command = new CreateVehicleCommand(customerId, "Toyota", "Corolla", 2023, "ABC1234", "Prata");
        await _handler.Handle(command, CancellationToken.None);

        _vehicleRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()), 
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCallUnitOfWorkCommit()
    {
        var customerId = Guid.NewGuid();

        var command = new CreateVehicleCommand(customerId, "Toyota", "Corolla", 2023, "ABC1234", "Prata");
        await _handler.Handle(command, CancellationToken.None);

        _unitOfWorkMock.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()), 
            Times.Once
        );
    }
}