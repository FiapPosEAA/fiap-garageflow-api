using GarageFlowService.Application.DTOs;
using GarageFlowService.Domain.Interfaces;
using GarageFlowService.Application.Interfaces;
using MediatR;

namespace GarageFlowService.Application.UseCases.Parts;

public record UpdatePartStockCommand(Guid Id, int QuantityChange) : IRequest<PartDto?>;

public class UpdatePartStockHandler : IRequestHandler<UpdatePartStockCommand, PartDto?>
{
    private readonly IPartRepository _partRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePartStockHandler(IPartRepository partRepository, IUnitOfWork unitOfWork)
    {
        _partRepository = partRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PartDto?> Handle(UpdatePartStockCommand request, CancellationToken cancellationToken)
    {
        var part = await _partRepository.GetByIdAsync(request.Id, cancellationToken);
        if (part is null) return null;

        part.UpdateStock(request.QuantityChange);
        _partRepository.Update(part);
        await _unitOfWork.CommitAsync(cancellationToken);
        return new PartDto(part.Id, part.Name, part.Description, part.Price, part.StockQuantity, part.IsActive);
    }
}

