using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Repositories;
using ExportPro.StorageService.Models.Models;

namespace ExportPro.StorageService.CQRS.Queries.Items;

public record GetItemsQuery(): IQuery<List<Item>>;

public class GetItemsQueryHandler(ItemRepository repository) : IQueryHandler<GetItemsQuery, List<Item>>
{
    private readonly ItemRepository _repository = repository;
    public async Task<BaseResponse<List<Item>>> Handle(GetItemsQuery request, CancellationToken cancellationToken)
    {
        var items = await _repository.GetItemsAsync(cancellationToken);
        if(items == null || items.Count == 0)
        {
            return new NotFoundResponse<List<Item>>("No items found.");
        }
        return new SuccessResponse<List<Item>>(items);
    }
}
