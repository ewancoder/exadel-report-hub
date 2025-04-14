using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Repositories;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Commands.Items;

public record UpdateItemCommand(
    ObjectId Id,
    string Name,
    string Description,
    double Price,
    string CustomerId,
    string InvoiceId,
    string ClientId) : ICommand<bool>;

public class UpdateItemCommandHandler(ItemRepository repository) : ICommandHandler<UpdateItemCommand, bool>
{
    private readonly ItemRepository _repository = repository;
    public async Task<BaseResponse<bool>> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (item == null)
        {
            return new NotFoundResponse<bool>("Item not found");
        }
        item.Name = request.Name;
        item.Description = request.Description;
        item.Price = request.Price;
        item.CustomerId = request.CustomerId;
        item.InvoiceId = request.InvoiceId;
        item.ClientId = request.ClientId;
        await _repository.UpdateOneAsync(item, cancellationToken);
        return new SuccessResponse<bool>(true);
    }
}