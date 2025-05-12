using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.QueryHandlers.ReportPreferenceQueries;

public sealed record GetReportPreferenceByClientQuery(Guid ClientId) : IQuery<List<ReportPreferenceResponse>>;

public sealed class GetReportPreferenceByClientHandler(IReportPreference repository, IMapper mapper)
    : IQueryHandler<GetReportPreferenceByClientQuery, List<ReportPreferenceResponse>>
{
    public async Task<BaseResponse<List<ReportPreferenceResponse>>> Handle(
        GetReportPreferenceByClientQuery request,
        CancellationToken cancellationToken
    )
    {
        var clientId = request.ClientId.ToObjectId();
        var preferences = await repository.GetAllByClientIdAsync(clientId, cancellationToken);
        var response = preferences.Select(mapper.Map<ReportPreferenceResponse>).ToList();
        return new SuccessResponse<List<ReportPreferenceResponse>>(response);
    }
}
