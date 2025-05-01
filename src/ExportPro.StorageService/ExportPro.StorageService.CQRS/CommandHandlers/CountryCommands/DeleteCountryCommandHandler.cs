using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;

namespace ExportPro.StorageService.CQRS.CommandHandlers.CountryCommands;

public sealed record DeleteCountryCommand(Guid Id) : ICommand<bool>;

public sealed class DeleteCountryCommandHandler(ICountryRepository repository)
    : ICommandHandler<DeleteCountryCommand, bool>
{
    public async Task<BaseResponse<bool>> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetOneAsync(
            x => x.Id == request.Id.ToObjectId() && !x.IsDeleted,
            cancellationToken
        );
        if (existing == null)
            return new NotFoundResponse<bool> { Messages = ["Country not found."] };
        await repository.SoftDeleteAsync(request.Id.ToObjectId(), cancellationToken);
        return new SuccessResponse<bool>(true, "Country deleted successfully.");
    }
}
