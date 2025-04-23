using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.DataAccess.Interfaces;
using MediatR;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.CommandHandlers.CurrencyCommands;

public record DeleteCurrencyCommand(ObjectId Id) : IRequest<BaseResponse<bool>>;
public class DeleteCurrencyHandler(ICurrencyRepository repository) : IRequestHandler<DeleteCurrencyCommand, BaseResponse<bool>>
{
    private readonly ICurrencyRepository _repository = repository;

    public async Task<BaseResponse<bool>> Handle(DeleteCurrencyCommand request, CancellationToken cancellationToken)
    {
        await _repository.SoftDeleteAsync(request.Id, cancellationToken);
        return new BaseResponse<bool> { Data = true };
    }
}