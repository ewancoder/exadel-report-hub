using System.Security.Claims;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;
using Microsoft.AspNetCore.Http;

namespace ExportPro.StorageService.CQRS.CommandHandlers.CountryCommands;

public sealed record UpdateCountryCommand(Guid Id, UpdateCountry Country) : ICommand<bool>;

public sealed class UpdateCountryCommandHandler(IHttpContextAccessor httpContext, ICountryRepository repository)
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
        existing.UpdatedBy = httpContext.HttpContext?.User.FindFirst(ClaimTypes.Name)!.Value;
        existing.UpdatedAt = DateTime.UtcNow;

        await repository.UpdateOneAsync(existing, cancellationToken);

        return new SuccessResponse<bool>(true, "Country updated successfully.");
    }
}
