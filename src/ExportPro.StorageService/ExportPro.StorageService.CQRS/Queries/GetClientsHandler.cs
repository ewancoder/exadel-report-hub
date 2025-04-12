using System.Runtime.CompilerServices;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Repositories;
using ExportPro.StorageService.SDK.Mapping;
using ExportPro.StorageService.SDK.Responses;
using MediatR;

namespace ExportPro.StorageService.CQRS.Queries;
public record class GetClientsQuery() : IQuery<List<ClientResponse>>;
public class GetClientsHandler(ClientRepository clientRepository) : IQueryHandler<GetClientsQuery, List<ClientResponse>>
{
    public async Task<BaseResponse<List<ClientResponse>>> Handle(GetClientsQuery request, CancellationToken cancellationToken)
    {
        var clients = await clientRepository.GetClients();
        List<ClientResponse> clientResponse = clients.Select(x => ClientToClientResponse.ClientToClientReponse(x)).ToList();
        return new SuccessResponse<List<ClientResponse>>(clientResponse);
    } 
}

