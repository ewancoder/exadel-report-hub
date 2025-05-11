using AutoMapper;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.Responses;
using MediatR;

namespace ExportPro.StorageService.CQRS.QueryHandlers.ReportPreferenceQueries;
public sealed record GetReportPreferenceByClientQuery(Guid ClientId)
    : IRequest<BaseResponse<List<ReportPreferenceResponse>>>;

public sealed class GetReportPreferenceByClientHandler(IReportPreference repository, IMapper mapper)
    : IRequestHandler<GetReportPreferenceByClientQuery, BaseResponse<List<ReportPreferenceResponse>>>
{
    public async Task<BaseResponse<List<ReportPreferenceResponse>>> Handle(
        GetReportPreferenceByClientQuery request,
        CancellationToken cancellationToken
    )
    {
        var clientId = request.ClientId.ToObjectId();
        var preferences = await repository.GetAllByClientIdAsync(clientId, cancellationToken);
        var response = preferences.Select(mapper.Map<ReportPreferenceResponse>).ToList();
        return new BaseResponse<List<ReportPreferenceResponse>> { Data = response };
    }
}