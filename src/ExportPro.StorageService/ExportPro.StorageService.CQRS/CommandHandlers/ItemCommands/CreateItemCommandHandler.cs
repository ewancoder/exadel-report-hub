using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.Models.Models;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.CommandHandlers.ItemCommands;

public sealed record CreateItemCommand(
    string? Name,
    string? Description,
    double Price,
    Status Status,
    Guid CurrencyId,
    Guid ClientId
) : ICommand<string>;

public sealed class CreateItemCommandHandler(IClientRepository clientRepository)
    : ICommandHandler<CreateItemCommand, string>
{
    public async Task<BaseResponse<string>> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        var client = await clientRepository.GetByIdAsync(request.ClientId.ToObjectId(), cancellationToken);
        if (client == null || client.IsDeleted)
            return new NotFoundResponse<string>("Client not found");
        var item = new Item
        {
            Id = ObjectId.GenerateNewId(),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Status = request.Status,
            CurrencyId = request.CurrencyId.ToObjectId(),
        };
        client.Items ??= new List<Item>();
        client.Items.Add(item);
        client.UpdatedAt = DateTime.UtcNow;
        await clientRepository.AddItem(client.Id, client, cancellationToken);
        return new SuccessResponse<string>(item.Id.ToString());
    }
}
