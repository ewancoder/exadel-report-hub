using AutoMapper;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.CommandHandlers.PreferenceCommands;

public sealed record RemoveReportPreferenceCommand(Guid Id) : ICommand<ReportPreferenceResponse>;

public sealed class RemoveReportPreferenceHandler(IReportPreference repository, IMapper mapper)
    : ICommandHandler<RemoveReportPreferenceCommand, ReportPreferenceResponse>
{
    public async Task<BaseResponse<ReportPreferenceResponse>> Handle(
        RemoveReportPreferenceCommand request,
        CancellationToken cancellationToken
    )
    {
        var preference = await repository.GetByIdAsync(request.Id.ToObjectId(), cancellationToken);

        if (preference is null)
            return new NotFoundResponse<ReportPreferenceResponse>("Report preference not found");

        var prefDeleted = await repository.SoftDeleteAsync(preference.Id, cancellationToken);
        var prefResponse = mapper.Map<ReportPreferenceResponse>(prefDeleted);

        return new SuccessResponse<ReportPreferenceResponse>(prefResponse, "Report preference successfully removed.");
    }
}
