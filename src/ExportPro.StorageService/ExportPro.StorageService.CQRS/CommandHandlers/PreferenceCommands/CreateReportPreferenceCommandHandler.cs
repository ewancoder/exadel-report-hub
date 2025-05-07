using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Export.Job.ServiceHost.Helpers;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;
using MediatR;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.CommandHandlers.PreferenceCommands;

public sealed record CreateReportPreferenceCommand(
    string UserId,
    Guid ClientId,
    ReportFormat Format,
    ReportScheduleDto Schedule
) : IRequest<BaseResponse<ReportPreferenceResponse>>;

public sealed class CreateReportPreferenceHandler(
    IReportPreference repository,
    IMapper mapper,
    IClientRepository clientRepository)
    : IRequestHandler<CreateReportPreferenceCommand, BaseResponse<ReportPreferenceResponse>>
{
    public async Task<BaseResponse<ReportPreferenceResponse>> Handle(
        CreateReportPreferenceCommand request,
        CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(request.UserId, out var userObjectId))
        {
            return new BaseResponse<ReportPreferenceResponse>
            {
                Messages = ["Could not parse the UserId"]
            };
        }

        var client = await clientRepository.GetByIdAsync(request.ClientId.ToObjectId(), cancellationToken);
        
        if (client is null)
        {
            return new NotFoundResponse<ReportPreferenceResponse>
            {
                Messages = [$"Client with Id: {request.ClientId} not found."]
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

        var preference = new ReportPreference
        {
            UserId = userObjectId,
            ClientId = request.ClientId.ToObjectId(),
            ReportFormat = request.Format,
            CronExpression = cronExpression,
            IsDelivered = false,
            IsEnabled = true,
            CreatedAt = DateTime.UtcNow
        };

        await repository.AddOneAsync(preference, cancellationToken);
        var response = mapper.Map<ReportPreferenceResponse>(preference);

        return new BaseResponse<ReportPreferenceResponse> { Data = response };
    }
}