using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using MediatR;

namespace ExportPro.StorageService.CQRS.CommandHandlers.PreferenceCommands;

public sealed record RemoveReportPreferenceCommand(Guid Id)
    : IRequest<BaseResponse<string>>;

public sealed class RemoveReportPreferenceHandler(IReportPreference repository)
    : IRequestHandler<RemoveReportPreferenceCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(
        RemoveReportPreferenceCommand request,
        CancellationToken cancellationToken
    )
    {
        var preference = await repository.GetByIdAsync(request.Id.ToObjectId(), cancellationToken);

        if (preference is null)
            return new NotFoundResponse<string>("Report preference not found");

        await repository.SoftDeleteAsync(preference.Id, cancellationToken);
        return new BaseResponse<string> { Data = "Report preference successfully removed." };
    }
}