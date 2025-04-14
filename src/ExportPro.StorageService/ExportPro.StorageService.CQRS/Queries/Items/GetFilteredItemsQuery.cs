using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Repositories;
using ExportPro.StorageService.Models.Models;


namespace ExportPro.StorageService.CQRS.Queries.Items;

public record GetFilteredItemsQuery(string? CustomerId, string? ClientId, string? InvoiceId): IQuery<List<Item>>;


public class GetFilteredItemsQueryHandler(ItemRepository repository) : IQueryHandler<GetFilteredItemsQuery, List<Item>>
{
    private readonly ItemRepository _repository = repository;
    public async Task<BaseResponse<List<Item>>> Handle(GetFilteredItemsQuery request, CancellationToken cancellationToken)
    {
        var items = await _repository.GetFilteredItemsAsync(request.CustomerId, request.ClientId, request.InvoiceId, cancellationToken);
        return new SuccessResponse<List<Item>>(items);
    }
}