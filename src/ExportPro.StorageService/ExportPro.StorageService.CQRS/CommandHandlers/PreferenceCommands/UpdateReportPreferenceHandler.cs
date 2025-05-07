using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Export.Job.ServiceHost.Helpers;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;
using MediatR;

namespace ExportPro.StorageService.CQRS.CommandHandlers.PreferenceCommands;

public sealed record UpdateReportPreferenceCommand(
    Guid Id,
    ReportFormat ReportFormat,
    ReportScheduleDto Schedule,
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
        {
            return new NotFoundResponse<ReportPreferenceResponse>
            {
                Messages = ["Report preference not found."]
            };
        }

        string cronExpression;
        try
        {
            cronExpression = CronHelper.ToCron(request.Schedule);
        }
        catch (Exception ex)
        {
            return new BaseResponse<ReportPreferenceResponse>
            {
                Messages = [$"Failed to generate cron expression: {ex.Message}"]
            };
        }

        preference.ReportFormat = request.ReportFormat;
        preference.CronExpression = cronExpression;
        preference.IsEnabled = request.IsEnabled;
        preference.UpdatedAt = DateTime.UtcNow;

        await repository.UpdateOneAsync(preference, cancellationToken);

        var response = mapper.Map<ReportPreferenceResponse>(preference);
        return new BaseResponse<ReportPreferenceResponse> { Data = response };
    }
}