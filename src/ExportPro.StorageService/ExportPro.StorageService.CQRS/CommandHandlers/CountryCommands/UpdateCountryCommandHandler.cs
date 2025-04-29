using System.Net;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.CommandHandlers.CountryCommands;

public record UpdateCountryCommand(Guid Id, UpdateCountry Country) : ICommand<bool>;

public class UpdateCountryCommandHandler(ICountryRepository repository) : ICommandHandler<UpdateCountryCommand, bool>
{
    public async Task<BaseResponse<bool>> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetOneAsync(
            x => x.Id == request.Id.ToObjectId() && !x.IsDeleted,
            cancellationToken
        );
        if (existing == null)
        {
            return new BaseResponse<bool>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.NotFound,
                Messages = ["Country not found."],
            };
        }

        existing.Name = request.Country.Name;
        existing.Code = request.Country.Code;
        existing.UpdatedAt = DateTime.UtcNow;

        await repository.UpdateOneAsync(existing, cancellationToken);

        return new BaseResponse<bool>
        {
            IsSuccess = true,
            ApiState = HttpStatusCode.OK,
            Data = true,
            Messages = ["Country updated successfully."],
        };
    }
}
