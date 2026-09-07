using GarageFlowService.Application.DTOs;
using GarageFlowService.Domain.Interfaces;
using MediatR;

namespace GarageFlowService.Application.UseCases.Parts;

public record GetAllPartsQuery : IRequest<IEnumerable<PartDto>>;

public class GetAllPartsHandler : IRequestHandler<GetAllPartsQuery, IEnumerable<PartDto>>
{
    private readonly IPartRepository _partRepository;

    public GetAllPartsHandler(IPartRepository partRepository)
    {
        _partRepository = partRepository;
    }

    public async Task<IEnumerable<PartDto>> Handle(GetAllPartsQuery request, CancellationToken cancellationToken)
    {
        var parts = await _partRepository.GetAllAsync(cancellationToken);
        return parts.Select(p => new PartDto(p.Id, p.Name, p.Description, p.Price, p.StockQuantity, p.IsActive));
    }
}

