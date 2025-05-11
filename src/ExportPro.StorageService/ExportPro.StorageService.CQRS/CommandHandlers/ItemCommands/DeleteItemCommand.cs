using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Helpers;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;

namespace ExportPro.StorageService.CQRS.CommandHandlers.ItemCommands;

public sealed record DeleteItemCommand(Guid ItemId, Guid ClientId) : ICommand<bool>, IPermissionedRequest
{
    public List<Guid>? ClientIds { get; init; } = [ClientId];
    public Resource Resource { get; init; } = Resource.Items;
    public CrudAction Action { get; init; } = CrudAction.Delete;
};

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
