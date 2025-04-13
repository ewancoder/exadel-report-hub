using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Queries.Client;
using ExportPro.StorageService.DataAccess.Services;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.Handlers.Client;

public class GetAllCLientsIncludingSoftDeletedQueryHandler(IClientService clientService) : IQueryHandler<GetAllCLientsIncludingSoftDeletedQuery, List<ClientResponse>>
{
    IClientService _clientService = clientService;
    public async Task<BaseResponse<List<ClientResponse>>> Handle(GetAllCLientsIncludingSoftDeletedQuery request, CancellationToken cancellationToken)
    {
        var clients = await _clientService.GetAllCLientsIncludingSoftDeleted();
        return new SuccessResponse<List<ClientResponse>>(clients, message: "Clients are sucessfully retrieved");
    }
}