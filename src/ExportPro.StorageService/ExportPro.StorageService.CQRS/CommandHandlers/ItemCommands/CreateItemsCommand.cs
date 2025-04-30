using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.DataAccess.Repositories;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.CommandHandlers.ItemCommands;

public record CreateItemsCommand(Guid ClientId, List<ItemDtoForClient> Items) : ICommand<bool>;

public class CreateItemsCommandHandler(IClientRepository repository, IMapper mapper)
    : ICommandHandler<CreateItemsCommand, bool>
{
    public async Task<BaseResponse<bool>> Handle(CreateItemsCommand request, CancellationToken cancellationToken)
    {
        var client = await repository.GetByIdAsync(request.ClientId.ToObjectId(), cancellationToken);
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
