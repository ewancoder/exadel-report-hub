using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Helpers;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.PaginationParams;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.QueryHandlers.PlanQueries;

public sealed record GetClientPlansQuery(Guid ClientId, PaginationParameters paginationParameters)
    : IQuery<PaginatedList<PlansResponse>>,
        IPermissionedRequest
{
    public List<Guid>? ClientIds => [ClientId];

    public Resource Resource => Resource.Plans;

    public CrudAction Action => CrudAction.Read;
};

public sealed class GetClientsPlansQueryHandler(IClientRepository clientRepository)
    : IQueryHandler<GetClientPlansQuery, PaginatedList<PlansResponse>>
{
    public async Task<BaseResponse<PaginatedList<PlansResponse>>> Handle(
        GetClientPlansQuery request,
        CancellationToken cancellationToken
    )
    {
        var client = await clientRepository.GetOneAsync(
            x => x.Id == request.ClientId.ToObjectId() && !x.IsDeleted,
            cancellationToken
        );
        if (client == null)
            return new NotFoundResponse<PaginatedList<PlansResponse>>("Client Not Found");
        var plans = await clientRepository.GetClientPlans(
            request.ClientId.ToObjectId(),
            request.paginationParameters,
            cancellationToken
        );
        return new SuccessResponse<PaginatedList<PlansResponse>>(plans, "Client plans retrieved successfully");
    }
}
