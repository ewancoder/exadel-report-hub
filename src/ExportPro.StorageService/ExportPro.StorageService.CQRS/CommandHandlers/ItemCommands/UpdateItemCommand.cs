using System.Net;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.CommandHandlers.ItemCommands;

public sealed record UpdateItemCommand(Guid ClientId, Item Item) : ICommand<bool>;

public sealed class UpdateItemCommandHandler(IClientRepository repository) : ICommandHandler<UpdateItemCommand, bool>
{
    public async Task<BaseResponse<bool>> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
    {
        if (request.Item.Id == ObjectId.Empty)
            return new BaseResponse<bool>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.BadRequest,
                Messages = ["Item is null or missing valid Id."],
            };

        var updated = await repository.UpdateItemInClient(
            request.ClientId.ToObjectId(),
            request.Item,
            cancellationToken
        );

        if (!updated)
            return new NotFoundResponse<bool>($"Item with ID {request.Item.Id} not found in client.");

        return new SuccessResponse<bool>(true);
    }
}
