using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Repositories;
using ExportPro.StorageService.Models.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Net;

namespace ExportPro.StorageService.CQRS.Commands.Items;

public record DeleteItemCommand(string Id, string ClientId) : ICommand<bool>;

public class DeleteItemCommandHandler(ClientRepository repository) : ICommandHandler<DeleteItemCommand, bool>
{
    private readonly ClientRepository _repository = repository;
    public async Task<BaseResponse<bool>> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {

        if (!ObjectId.TryParse(request.ClientId, out var clientObjectId))
        {
            return new BaseResponse<bool>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.BadRequest,
                Messages = ["Invalid ClientId format."]
            };
        }

        if (!ObjectId.TryParse(request.Id, out var itemObjectId))
        {
            return new BaseResponse<bool>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.BadRequest,
                Messages = ["Invalid ItemId format."]
            };
        }

        var success = await _repository.RemoveItemFromClient(clientObjectId, itemObjectId, cancellationToken);

        if (!success)
        {
            return new NotFoundResponse<bool>($"Item with ID {request.Id} not found or already deleted.");
        }

        return new SuccessResponse<bool>(true);
    }
}


