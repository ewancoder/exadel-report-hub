using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Commands.Country;
using ExportPro.StorageService.DataAccess.Interfaces;

namespace ExportPro.StorageService.CQRS.Handlers.Country;

public class UpdateCountryCommandHandler(ICountryRepository repository) : ICommandHandler<UpdateCountryCommand, bool>
{
    private readonly ICountryRepository _repository = repository;

    public async Task<BaseResponse<bool>> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (existing == null) return new BaseResponse<bool> { IsSuccess = false, Messages = ["Country not found"] };

        existing.Name = request.Name;
        existing.Code = request.Code;
        existing.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateOneAsync(existing, cancellationToken);
        return new BaseResponse<bool> { Data = true };
    }
}