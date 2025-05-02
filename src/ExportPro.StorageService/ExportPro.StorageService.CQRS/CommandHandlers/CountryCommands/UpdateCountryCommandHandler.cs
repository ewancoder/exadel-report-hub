using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;

namespace ExportPro.StorageService.CQRS.CommandHandlers.CountryCommands;

public sealed record UpdateCountryCommand(Guid Id, UpdateCountry Country) : ICommand<bool>;

public sealed class UpdateCountryCommandHandler(ICountryRepository repository)
    : ICommandHandler<UpdateCountryCommand, bool>
{
    public async Task<BaseResponse<bool>> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetOneAsync(
            x => x.Id == request.Id.ToObjectId() && !x.IsDeleted,
            cancellationToken
        );
        if (existing == null)
            return new NotFoundResponse<bool>("Country not found.");

        existing.Name = request.Country.Name;
        existing.Code = request.Country.Code;
        existing.UpdatedAt = DateTime.UtcNow;

        await repository.UpdateOneAsync(existing, cancellationToken);

        return new SuccessResponse<bool>(true, "Country updated successfully.");
    }
}
