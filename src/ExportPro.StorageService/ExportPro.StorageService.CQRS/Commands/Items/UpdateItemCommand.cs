using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using MongoDB.Bson;
using System.Net;

namespace ExportPro.StorageService.CQRS.Commands.Items;

public record UpdateItemCommand(string ClientId, Item Item) : ICommand<bool>;

public class UpdateItemCommandHandler(IClientRepository repository) : ICommandHandler<UpdateItemCommand, bool>
{
    private readonly IClientRepository _repository = repository;

    public async Task<BaseResponse<bool>> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
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

        if (request.Item is null || request.Item.Id == ObjectId.Empty)
        {
            return new BaseResponse<bool>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.BadRequest,
                Messages = ["Item is null or missing valid Id."]
            };
        }

        var updated = await _repository.UpdateItemInClient(clientObjectId, request.Item, cancellationToken);

        if (!updated)
        {
            return new NotFoundResponse<bool>($"Item with ID {request.Item.Id} not found in client.");
        }

        return new SuccessResponse<bool>(true);
    }
}