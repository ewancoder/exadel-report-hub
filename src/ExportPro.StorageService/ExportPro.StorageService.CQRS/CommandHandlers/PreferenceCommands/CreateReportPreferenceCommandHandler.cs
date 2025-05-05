using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;
using MediatR;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.CommandHandlers.PreferenceCommands;

public sealed record CreateReportPreferenceCommand(
    string UserId,
    Guid ClientId,
    ReportFormat Format,
    ReportFrequency Frequency,
    DayOfWeek? DayOfWeek,
    int? DayOfMonth,
    TimeOnly TimeToSend
) : IRequest<BaseResponse<ReportPreferenceResponse>>;

public sealed class CreateReportPreferenceHandler(IReportPreference repository, IMapper mapper, IClientRepository clientRepository)
    : IRequestHandler<CreateReportPreferenceCommand, BaseResponse<ReportPreferenceResponse>>
{
    public async Task<BaseResponse<ReportPreferenceResponse>> Handle(
        CreateReportPreferenceCommand request,
        CancellationToken cancellationToken
    )
    {
        if (!ObjectId.TryParse(request.UserId, out var userObjectId))
            return new BaseResponse<ReportPreferenceResponse> { Messages = [ "Could not parse the ID" ] };

        var client = await clientRepository.GetByIdAsync(request.ClientId.ToObjectId(), cancellationToken);
        
        if (client is null)
        {
            return new NotFoundResponse<ReportPreferenceResponse> { Messages = [$"Client wiwh Id: {request.ClientId} not found."] };
        }

        var preference = new ReportPreference
        {
            UserId = userObjectId,
            ClientId = request.ClientId.ToObjectId(),
            Format = request.Format,
            Frequency = request.Frequency,
            DayOfWeek = request.DayOfWeek,
            DayOfMonth = request.DayOfMonth,
            SendTime = request.TimeToSend,
            CreatedAt = DateTime.UtcNow,
            IsEnabled = true
        };

        await repository.AddOneAsync(preference, cancellationToken);
        var response = mapper.Map<ReportPreferenceResponse>(preference);
        return new BaseResponse<ReportPreferenceResponse> { Data = response };
    }
}