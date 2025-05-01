using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.DataAccess.Repositories;
using ExportPro.StorageService.Models.Models;
using MongoDB.Bson;
using System.Net;

namespace ExportPro.StorageService.CQRS.CommandHandlers.Items;

public record UpdateItemsCommand(string ClientId, List<Item> Items) : ICommand<bool>;

public class UpdateItemsCommandHandler(IClientRepository repository) : ICommandHandler<UpdateItemsCommand, bool>
{
    private readonly IClientRepository _repository = repository;
    public async Task<BaseResponse<bool>> Handle(UpdateItemsCommand request, CancellationToken cancellationToken)
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

        if (request.Items == null || request.Items.Count == 0)
        {
            return new BaseResponse<bool>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.BadRequest,
                Messages = ["No items provided for update."]
            };
        }

        var updatedCount = await _repository.UpdateItemsInClient(clientObjectId, request.Items, cancellationToken);

        if (updatedCount < request.Items.Count)
        {
            return new NotFoundResponse<bool>($"Not all of the items were found or updated. Num of updated item: {updatedCount}");
        }

        return new SuccessResponse<bool>(true)
        {
            Messages = [$"{updatedCount} items successfully updated."]
        };
    }
}
