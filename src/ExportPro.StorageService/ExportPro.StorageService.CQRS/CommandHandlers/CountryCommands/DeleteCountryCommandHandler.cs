using System.Net;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.CommandHandlers.CountryCommands;

public record DeleteCountryCommand(Guid Id) : ICommand<bool>;

public class DeleteCountryCommandHandler(ICountryRepository repository) : ICommandHandler<DeleteCountryCommand, bool>
{
    public async Task<BaseResponse<bool>> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByIdAsync(request.Id.ToObjectId(), cancellationToken);
        if (existing == null)
        {
            return new NotFoundResponse<bool>() { Messages = ["Country not found."] };
        }
        await repository.SoftDeleteAsync(request.Id.ToObjectId(), cancellationToken);
        return new SuccessResponse<bool>(true, "Country deleted successfully.");
    }
}
