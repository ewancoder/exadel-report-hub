using System.Security.Claims;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Helpers;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.Models.Models;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.CommandHandlers.ItemCommands;

public sealed record CreateItemCommand(
    string? Name,
    string? Description,
    double Price,
    Status Status,
    Guid CurrencyId,
    Guid ClientId
) : ICommand<Guid>, IPermissionedRequest
{
    public List<Guid>? ClientIds { get; init; } = [ClientId];
    public Resource Resource { get; init; } = Resource.Items;
    public CrudAction Action { get; init; } = CrudAction.Create;
}

public sealed class CreateItemCommandHandler(IHttpContextAccessor httpContext, IClientRepository clientRepository)
    : ICommandHandler<CreateItemCommand, Guid>
{
    public async Task<BaseResponse<Guid>> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        var client = await clientRepository.GetOneAsync(
            x => x.Id == request.ClientId.ToObjectId() && !x.IsDeleted,
            cancellationToken
        );
        if (client == null || client.IsDeleted)
            return new NotFoundResponse<Guid>("Client not found");
        var item = new Item
        {
            Id = ObjectId.GenerateNewId(),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Status = request.Status,
            CreatedBy = httpContext.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value,
            CurrencyId = request.CurrencyId.ToObjectId(),
        };
        client.Items ??= new List<Item>();
        client.Items.Add(item);
        client.UpdatedAt = DateTime.UtcNow;
        await clientRepository.AddItem(client.Id, client, cancellationToken);
        return new SuccessResponse<Guid>(item.Id.ToGuid());
    }
}
