using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Repositories;
using ExportPro.StorageService.Models.Models;

namespace ExportPro.StorageService.CQRS.Commands.Items;

public record UpdateItemsCommand(List<Item> Items) : ICommand<bool>;

public class UpdateItemsCommandHandler(ItemRepository repository) : ICommandHandler<UpdateItemsCommand, bool>
{
    private readonly ItemRepository _repository = repository;
    public async Task<BaseResponse<bool>> Handle(UpdateItemsCommand request, CancellationToken cancellationToken)
    {
        if (request.Items == null || request.Items.Count == 0)
        {
            return new NotFoundResponse<bool>("No items provided for update");
        }
        foreach (var item in request.Items)
        {
            var existingItem = await _repository.GetByIdAsync(item.Id, cancellationToken);
            if (existingItem == null)
            {
                return new NotFoundResponse<bool>($"Item with ID {item.Id} not found");
            }
        }
        await _repository.UpdateManyAsync(request.Items, cancellationToken);
        return new SuccessResponse<bool>(true);
    }
}
