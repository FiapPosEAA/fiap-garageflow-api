using GarageFlowService.Application.DTOs;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Domain.Interfaces;
using GarageFlowService.Application.Interfaces;
using MediatR;

namespace GarageFlowService.Application.UseCases.Customers;

public record CreateCustomerCommand(string Name, string Email, string Phone, string Document) : IRequest<CustomerDto>;

public class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, CustomerDto>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CustomerDto> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = new Customer(request.Name, request.Email, request.Phone, request.Document);
        await _customerRepository.AddAsync(customer, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return new CustomerDto(customer.Id, customer.Name, customer.Email, customer.Phone, customer.Document, customer.CreatedAt);
    }
}

