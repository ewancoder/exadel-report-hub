using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.DataAccess.Repositories;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.Models.Models;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Commands.Items;

public record CreateItemCommand(string Name,
    string Description, 
    double Price, 
    Status Status,
    Currency Currency,
    string ClientId): ICommand<string>;

public class CreateItemCommandHandler(IClientRepository clientRepository) : ICommandHandler<CreateItemCommand, string>
{
    private readonly IClientRepository  _clientRepository = clientRepository;
    public async Task<BaseResponse<string>> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(request.ClientId, out var objectId))
            return new NotFoundResponse<string>("Invalid client ID format");
        var client = await _clientRepository.GetByIdAsync(objectId, cancellationToken);
        if (client == null || client.IsDeleted)
            return new NotFoundResponse<string>("Client not found");
        var item = new Models.Models.Item
        {
            Id = ObjectId.GenerateNewId(),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Status = request.Status,
            Currency = request.Currency
        };
        client.Items ??= new List<Item>();
        client.Items.Add(item);
        client.UpdatedAt = DateTime.UtcNow;
        await _clientRepository.AddItem(client.Id, client, cancellationToken);
        return new SuccessResponse<string>(item.Id.ToString());
    }
}



