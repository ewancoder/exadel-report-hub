using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;

namespace ExportPro.StorageService.CQRS.CommandHandlers.ItemCommands;

public sealed record DeleteItemCommand(Guid ItemId, Guid ClientId) : ICommand<bool>;

public sealed class DeleteItemCommandHandler(IClientRepository repository) : ICommandHandler<DeleteItemCommand, bool>
{
    public async Task<BaseResponse<bool>> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        var success = await repository.RemoveItemFromClient(
            request.ClientId.ToObjectId(),
            request.ItemId.ToObjectId(),
            cancellationToken
        );

        if (!success)
            return new NotFoundResponse<bool>($"Item with ID {request.ItemId} not found or already deleted.");

        return new SuccessResponse<bool>(true);
    }
}
