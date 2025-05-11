using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Helpers;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.QueryHandlers.PlanQueries;

public sealed record GetClientPlansQuery(Guid ClientId, int Top, int Skip) : IQuery<List<PlansResponse>>, IPermissionedRequest
{
    public List<Guid>? ClientIds => [ClientId];

    public Resource Resource => Resource.Plans;

    public CrudAction Action => CrudAction.Read;
};

public sealed class GetClientsPlansQueryHandler(IClientRepository clientRepository)
    : IQueryHandler<GetClientPlansQuery, List<PlansResponse>>
{
    public async Task<BaseResponse<List<PlansResponse>>> Handle(
        GetClientPlansQuery request,
        CancellationToken cancellationToken
    )
    {
        var client = await clientRepository.GetOneAsync(
            x => x.Id == request.ClientId.ToObjectId() && !x.IsDeleted,
            cancellationToken
        );
        if (client == null)
            return new NotFoundResponse<List<PlansResponse>>("Client Not Found");
        var plans = await clientRepository.GetClientPlans(
            request.ClientId.ToObjectId(),
            request.Top,
            request.Skip,
            cancellationToken
        );
        return new SuccessResponse<List<PlansResponse>>(plans, "Client plans retrieved successfully");
    }
}
