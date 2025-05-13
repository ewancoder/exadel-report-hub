using System.Security.Claims;
using AutoMapper;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.Export.Job.ServiceHost.Helpers;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.CommandHandlers.PreferenceCommands;

public sealed record CreateReportPreferenceCommand(CreateReportPreferencesDTO dto) : ICommand<ReportPreferenceResponse>;

public sealed class CreateReportPreferenceHandler(
    IReportPreference repository,
    IMapper mapper,
    IHttpContextAccessor httpContext,
    IClientRepository clientRepository
) : ICommandHandler<CreateReportPreferenceCommand, ReportPreferenceResponse>
{
    public async Task<BaseResponse<ReportPreferenceResponse>> Handle(
        CreateReportPreferenceCommand request,
        CancellationToken cancellationToken
    )
    {
        var userId = httpContext.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!ObjectId.TryParse(userId, out var userObjectId))
        {
            return new BaseResponse<ReportPreferenceResponse> { Messages = ["Could not parse the UserId"] };
        }

        var client = await clientRepository.GetByIdAsync(request.dto.ClientId.ToObjectId(), cancellationToken);

        if (client is null)
        {
            return new NotFoundResponse<ReportPreferenceResponse>
            {
                Messages = [$"Client with Id: {request.dto.ClientId} not found."],
            };
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
        var preference = new ReportPreference
        {
            UserId = userObjectId,
            ClientId = request.dto.ClientId.ToObjectId(),
            Email = request.dto.Email,
            ReportFormat = request.dto.ReportFormat,
            CronExpression = cronExpression,
            HumanReadableCronExpression = CronToTextHelper.ToReadableText(cronExpression),
            IsDelivered = false,
            IsEnabled = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null,
            CreatedBy = httpContext.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value
        };

        await repository.AddOneAsync(preference, cancellationToken);
        var response = mapper.Map<ReportPreferenceResponse>(preference);

        return new SuccessResponse<ReportPreferenceResponse>(response, "Successfully created!");
    }
}
