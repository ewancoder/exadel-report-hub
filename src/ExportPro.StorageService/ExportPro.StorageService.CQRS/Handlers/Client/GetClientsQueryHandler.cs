using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Services;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.Handlers.Client;
public record GetClientsQuery : IQuery<List<ClientResponse>>;

public class GetClientsQueryHandler(IClientService clientService) : IQueryHandler<GetClientsQuery, List<ClientResponse>>
{
    private readonly IClientService _clientService =clientService;

    public async Task<BaseResponse<List<ClientResponse>>> Handle(GetClientsQuery request, CancellationToken cancellationToken)
    {
        var clientresponse = await _clientService.GetClientsService();
        return new SuccessResponse<List<ClientResponse>>(clientresponse, message: "Clients Retrieved");
    }
}