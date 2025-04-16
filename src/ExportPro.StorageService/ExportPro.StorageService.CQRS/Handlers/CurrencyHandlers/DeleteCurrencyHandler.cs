using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.Commands.CurrencyCommand;
using ExportPro.StorageService.DataAccess.Interfaces;
using MediatR;

namespace ExportPro.StorageService.CQRS.Handlers.CurrencyHandlers;

public class DeleteCurrencyHandler(ICurrencyRepository repository) : IRequestHandler<DeleteCurrencyCommand, BaseResponse<bool>>
{
    private readonly ICurrencyRepository _repository = repository;

    public async Task<BaseResponse<bool>> Handle(DeleteCurrencyCommand request, CancellationToken cancellationToken)
    {
        await _repository.SoftDeleteAsync(request.Id, cancellationToken);
        return new BaseResponse<bool> { Data = true };
    }
}