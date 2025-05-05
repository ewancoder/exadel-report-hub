using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.SDK.Responses;
using MediatR;

namespace ExportPro.StorageService.CQRS.CommandHandlers.PreferenceCommands;

public sealed record UpdateReportPreferenceCommand(
    Guid Id,
    ReportFormat ReportFormat,
    ReportFrequency Frequency,
    DayOfWeek? DayOfWeek,
    int? DayOfMonth,
    TimeOnly SendTime,
    bool IsEnabled
) : IRequest<BaseResponse<ReportPreferenceResponse>>;

public sealed class UpdateReportPreferenceHandler(IReportPreference repository, IMapper mapper)
    : IRequestHandler<UpdateReportPreferenceCommand, BaseResponse<ReportPreferenceResponse>>
{
    public async Task<BaseResponse<ReportPreferenceResponse>> Handle(
        UpdateReportPreferenceCommand request,
        CancellationToken cancellationToken
    )
    {
        var preference = await repository.GetByIdAsync(request.Id.ToObjectId(), cancellationToken);

        if (preference is null)
            return new NotFoundResponse<ReportPreferenceResponse> { Messages = ["Report preference not found."] };

        preference.Format = request.ReportFormat;
        preference.Frequency = request.Frequency;
        preference.DayOfWeek = request.DayOfWeek;
        preference.DayOfMonth = request.DayOfMonth;
        preference.SendTime = request.SendTime;
        preference.IsEnabled = request.IsEnabled;
        preference.UpdatedAt = DateTime.UtcNow;

        await repository.UpdateOneAsync(preference, cancellationToken);

        var response = mapper.Map<ReportPreferenceResponse>(preference);
        return new BaseResponse<ReportPreferenceResponse> { Data = response };
    }
}