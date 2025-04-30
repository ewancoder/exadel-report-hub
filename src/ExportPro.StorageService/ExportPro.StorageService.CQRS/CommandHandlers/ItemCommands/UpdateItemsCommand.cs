using System.Net;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.DataAccess.Repositories;
using ExportPro.StorageService.Models.Models;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.CommandHandlers.ItemCommands;

public sealed record UpdateItemsCommand(Guid ClientId, List<Item> Items) : ICommand<bool>;

public sealed class UpdateItemsCommandHandler(IClientRepository repository) : ICommandHandler<UpdateItemsCommand, bool>
{
    public async Task<BaseResponse<bool>> Handle(UpdateItemsCommand request, CancellationToken cancellationToken)
    {
        if (request.Items == null || request.Items.Count == 0)
        {
            return new BaseResponse<bool>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.BadRequest,
                Messages = ["No items provided for update."],
            };
        }

        var updatedCount = await repository.UpdateItemsInClient(
            request.ClientId.ToObjectId(),
            request.Items,
            cancellationToken
        );

        if (updatedCount < request.Items.Count)
        {
            return new NotFoundResponse<bool>(
                $"Not all of the items were found or updated. Num of updated item: {updatedCount}"
            );
        }

        return new SuccessResponse<bool>(true) { Messages = [$"{updatedCount} items successfully updated."] };
    }
}
