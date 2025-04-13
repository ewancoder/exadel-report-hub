using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Queries;
using ExportPro.StorageService.DataAccess.Services;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.Handlers;

public class GetClientByIdIncludingSoftDeletedQueryHandler(IClientService clientService):IQueryHandler<GetClientByIdIncludingSoftDeletedQuery, ClientResponse>
{
    private readonly IClientService _clientService=clientService;
    public async Task<BaseResponse<ClientResponse>> Handle(GetClientByIdIncludingSoftDeletedQuery request, CancellationToken cancellationToken)
    {
        var client =await _clientService.GetClientByIdIncludingSoftDeleted(request.Id);
        if (client == null) return null;
        return new SuccessResponse<ClientResponse>(client);
    }
}