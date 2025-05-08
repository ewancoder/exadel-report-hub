using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.DataAccess.Interfaces;
using MediatR;

namespace ExportPro.StorageService.CQRS.CommandHandlers.CurrencyCommands;

public sealed record DeleteCurrencyCommand(Guid Id) : IRequest<BaseResponse<bool>>;

public sealed class DeleteCurrencyHandler(ICurrencyRepository repository)
    : IRequestHandler<DeleteCurrencyCommand, BaseResponse<bool>>
{
    public async Task<BaseResponse<bool>> Handle(DeleteCurrencyCommand request, CancellationToken cancellationToken)
    {
        await repository.SoftDeleteAsync(request.Id.ToObjectId(), cancellationToken);
        return new SuccessResponse<bool>(true);
    }
}
