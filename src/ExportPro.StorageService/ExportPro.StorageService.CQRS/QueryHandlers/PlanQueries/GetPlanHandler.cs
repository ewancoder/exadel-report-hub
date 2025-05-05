using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.QueryHandlers.PlanQueries;

public sealed record GetPlanQuery(Guid PlanId) : IQuery<PlansResponse>;

public sealed class GetPlanQueryHandler(IClientRepository clientRepository) : IQueryHandler<GetPlanQuery, PlansResponse>
{
    public async Task<BaseResponse<PlansResponse>> Handle(GetPlanQuery request, CancellationToken cancellationToken)
    {
        var plan = await clientRepository.GetPlan(request.PlanId.ToObjectId(), cancellationToken);
        if (plan == null)
            return new NotFoundResponse<PlansResponse>("Plan Not Found");
        return new SuccessResponse<PlansResponse>(plan!, "Plan retrieved successfully");
    }
}
