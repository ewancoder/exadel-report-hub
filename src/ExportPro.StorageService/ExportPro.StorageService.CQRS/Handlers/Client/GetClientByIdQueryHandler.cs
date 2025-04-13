using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Queries.Client;
using ExportPro.StorageService.DataAccess.Services;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.Handlers.Client;

public class GetClientByIdQueryHandler(IClientService clientService) : IQueryHandler<GetClientByIdQuery, ClientResponse>
{
    private readonly IClientService _clientService = clientService;
    public async Task<BaseResponse<ClientResponse>> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        var client = await _clientService.GetClientById(request.Id);
        return new SuccessResponse<ClientResponse>(client, message: "Client found");
    }
}