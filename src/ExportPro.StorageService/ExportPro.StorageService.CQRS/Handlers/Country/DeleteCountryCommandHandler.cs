using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Commands.Country;
using ExportPro.StorageService.DataAccess.Interfaces;

namespace ExportPro.StorageService.CQRS.Handlers.Country;

public class DeleteCountryCommandHandler(ICountryRepository repository) : ICommandHandler<DeleteCountryCommand, bool>
{
    private readonly ICountryRepository _repository = repository;

    public async Task<BaseResponse<bool>> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
    {
        await _repository.SoftDeleteAsync(request.Id, cancellationToken);
        return new BaseResponse<bool> { Data = true };
    }
}