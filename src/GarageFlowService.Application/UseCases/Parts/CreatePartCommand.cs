using GarageFlowService.Application.DTOs;
using GarageFlowService.Domain.Entities;
using GarageFlowService.Domain.Interfaces;
using GarageFlowService.Application.Interfaces;
using MediatR;

namespace GarageFlowService.Application.UseCases.Parts;

public record CreatePartCommand(string Name, string Description, decimal Price, int StockQuantity) : IRequest<PartDto>;

public class CreatePartHandler : IRequestHandler<CreatePartCommand, PartDto>
{
    private readonly IPartRepository _partRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePartHandler(IPartRepository partRepository, IUnitOfWork unitOfWork)
    {
        _partRepository = partRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PartDto> Handle(CreatePartCommand request, CancellationToken cancellationToken)
    {
        var part = new Part(request.Name, request.Description, request.Price, request.StockQuantity);
        await _partRepository.AddAsync(part, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return new PartDto(part.Id, part.Name, part.Description, part.Price, part.StockQuantity, part.IsActive);
    }
}

