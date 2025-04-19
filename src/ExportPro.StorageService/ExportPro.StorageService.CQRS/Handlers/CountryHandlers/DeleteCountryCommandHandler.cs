using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Commands.CountryCommand;
using ExportPro.StorageService.DataAccess.Interfaces;
using MongoDB.Bson;
using System.Net;

namespace ExportPro.StorageService.CQRS.Handlers.CountryHandlers;

public class DeleteCountryCommandHandler(ICountryRepository repository)
    : ICommandHandler<DeleteCountryCommand, bool>
{
    private readonly ICountryRepository _repository = repository;

    public async Task<BaseResponse<bool>> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(request.Id, out var objectId))
        {
            return new BaseResponse<bool>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.BadRequest,
                Messages = new List<string> { "Invalid country ID format." }
            };
        }

        var existing = await _repository.GetByIdAsync(objectId, cancellationToken);
        if (existing == null)
        {
            return new BaseResponse<bool>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.NotFound,
                Messages = new List<string> { "Country not found." }
            };
        }

        await _repository.SoftDeleteAsync(objectId, cancellationToken);

        return new BaseResponse<bool>
        {
            IsSuccess = true,
            ApiState = HttpStatusCode.OK,
            Data = true,
            Messages = new List<string> { "Country deleted successfully." }
        };
    }
}
