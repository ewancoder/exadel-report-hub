using System.Net;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.DataAccess.Repositories;
using ExportPro.StorageService.Models.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ExportPro.StorageService.CQRS.CommandHandlers.ItemCommands;

public record DeleteItemCommand(Guid ItemId, Guid ClientId) : ICommand<bool>;

public class DeleteItemCommandHandler(IClientRepository repository) : ICommandHandler<DeleteItemCommand, bool>
{
    public async Task<BaseResponse<bool>> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        var success = await repository.RemoveItemFromClient(
            request.ClientId.ToObjectId(),
            request.ItemId.ToObjectId(),
            cancellationToken
        );

        if (!success)
        {
            return new NotFoundResponse<bool>($"Item with ID {request.ItemId} not found or already deleted.");
        }

        return new SuccessResponse<bool>(true);
    }
}
