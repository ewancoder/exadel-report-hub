using AutoMapper;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Helpers;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;

namespace ExportPro.StorageService.CQRS.CommandHandlers.ItemCommands;

public sealed record CreateItemsCommand(Guid ClientId, List<ItemDtoForClient> Items) : ICommand<bool>, IPermissionedRequest
{
    public List<Guid>? ClientIds { get; init; } = [ClientId];
    public Resource Resource { get; init; } = Resource.Items;
    public CrudAction Action { get; init; } = CrudAction.Create;
};

public sealed class CreateItemsCommandHandler(IClientRepository repository, IMapper mapper)
    : ICommandHandler<CreateItemsCommand, bool>
{
    public async Task<BaseResponse<bool>> Handle(CreateItemsCommand request, CancellationToken cancellationToken)
    {
        var client = await repository.GetOneAsync(
            x => x.Id == request.ClientId.ToObjectId() && !x.IsDeleted,
            cancellationToken
        );
        if (client == null || client.IsDeleted)
            return new NotFoundResponse<bool>("Client not found");
        var result = await repository.AddItems(
            request.ClientId.ToObjectId(),
            mapper.Map<List<Item>>(request.Items),
            cancellationToken
        );
        if (!result)
            return new NotFoundResponse<bool>("Failed to add items to client");
        return new SuccessResponse<bool>(true);
    }
}
