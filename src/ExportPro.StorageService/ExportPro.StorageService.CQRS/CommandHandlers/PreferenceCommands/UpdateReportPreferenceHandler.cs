using System.Security.Claims;
using AutoMapper;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.Export.Job.Utilities.Helpers;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;
using Microsoft.AspNetCore.Http;
using CronHelper = ExportPro.Export.Job.Utilities.Helpers.CronHelper;
using CronToTextHelper = ExportPro.Export.Job.Utilities.Helpers.CronToTextHelper;

namespace ExportPro.StorageService.CQRS.CommandHandlers.PreferenceCommands;

public sealed record UpdateReportPreferenceCommand(UpdateReportPreferenceDTO dto) : ICommand<ReportPreferenceResponse>;

public sealed class UpdateReportPreferenceHandler(
    IReportPreference repository,
    IHttpContextAccessor httpContext,
    IMapper mapper
) : ICommandHandler<UpdateReportPreferenceCommand, ReportPreferenceResponse>
{
    public async Task<BaseResponse<ReportPreferenceResponse>> Handle(
        UpdateReportPreferenceCommand request,
        CancellationToken cancellationToken
    )
    {
        var preference = await repository.GetByIdAsync(request.dto.Id.ToObjectId(), cancellationToken);

        if (preference is null)
        {
            return new NotFoundResponse<ReportPreferenceResponse> { Messages = ["Report preference not found."] };
        }

        string cronExpression;
        try
        {
            cronExpression = CronHelper.ToCron(request.dto.Schedule);
        }
        catch (Exception ex)
        {
            return new BaseResponse<ReportPreferenceResponse>
            {
                Messages = [$"Failed to generate cron expression: {ex.Message}"],
            };
        }

        preference.ReportFormat = request.dto.ReportFormat;
        preference.Email = request.dto.Email;
        preference.CronExpression = cronExpression;
        preference.HumanReadableCronExpression = CronToTextHelper.ToReadableText(cronExpression);
        preference.IsEnabled = request.dto.IsEnabled;
        preference.UpdatedAt = DateTime.UtcNow;
        preference.UpdatedBy = httpContext.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value;

        await repository.UpdateOneAsync(preference, cancellationToken);

        var response = mapper.Map<ReportPreferenceResponse>(preference);

        return new SuccessResponse<ReportPreferenceResponse>(response, "Report successfully updated!");
    }
}
