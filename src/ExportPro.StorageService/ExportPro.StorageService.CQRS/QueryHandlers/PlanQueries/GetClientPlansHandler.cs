using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.QueryHandlers.PlanQueries;

public record GetClientPlansQuery(string ClientId, int Top, int Skip) : IQuery<List<PlansResponse>>;

public class GetClientsPlansQueryHandler(IClientRepository clientRepository)
    : IQueryHandler<GetClientPlansQuery, List<PlansResponse>>
{
    public async Task<BaseResponse<List<PlansResponse>>> Handle(
        GetClientPlansQuery request,
        CancellationToken cancellationToken
    )
    {
        var plans = await clientRepository.GetClientPlans(
            request.ClientId,
            request.Top,
            request.Skip,
            cancellationToken
        );
        return new SuccessResponse<List<PlansResponse>>(plans, "Client plans retrieved successfully");
    }
}
