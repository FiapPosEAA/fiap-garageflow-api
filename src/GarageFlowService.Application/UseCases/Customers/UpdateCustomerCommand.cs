using GarageFlowService.Application.DTOs;
using GarageFlowService.Domain.Interfaces;
using GarageFlowService.Application.Interfaces;
using MediatR;

namespace GarageFlowService.Application.UseCases.Customers;

public record UpdateCustomerCommand(Guid Id, string Name, string Email, string Phone) : IRequest<CustomerDto?>;

public class UpdateCustomerHandler : IRequestHandler<UpdateCustomerCommand, CustomerDto?>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CustomerDto?> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (customer is null) return null;

        customer.Update(request.Name, request.Email, request.Phone);
        _customerRepository.Update(customer);
        await _unitOfWork.CommitAsync(cancellationToken);
        return new CustomerDto(customer.Id, customer.Name, customer.Email, customer.Phone, customer.Document, customer.CreatedAt);
    }
}

