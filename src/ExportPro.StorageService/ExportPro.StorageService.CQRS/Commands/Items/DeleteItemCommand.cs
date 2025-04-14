using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Repositories;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Commands.Items;

public record DeleteItemCommand(ObjectId Id) : ICommand<bool>;

public class DeleteItemCommandHandler(ItemRepository repository) : ICommandHandler<DeleteItemCommand, bool>
{
    private readonly ItemRepository _repository = repository;
    public async Task<BaseResponse<bool>> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (item == null)
        {
            return new NotFoundResponse<bool>("Item not found");
        }
        await _repository.DeleteAsync(request.Id, cancellationToken);
        return new SuccessResponse<bool>(true);
    }
}

