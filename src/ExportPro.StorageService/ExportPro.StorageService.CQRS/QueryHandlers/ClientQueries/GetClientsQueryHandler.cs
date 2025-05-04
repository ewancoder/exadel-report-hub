using System.Net;
using AutoMapper;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Helpers;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.Common.Shared.Refit;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.QueryHandlers.ClientQueries;

public record GetClientsQuery(int Top, int Skip) : IQuery<List<ClientResponse>>, IPermissionedRequest
{
    public List<string>? ClientIds { get; init; } = null;
    public Resource Resource { get; init; } = Resource.Clients;
    public CrudAction Action { get; init; } = CrudAction.Read;
};

public class GetClientsQueryHandler(IClientRepository clientRepository, IMapper mapper, IACLSharedApi aclApi, IHttpContextAccessor httpContextAccessor)
    : IQueryHandler<GetClientsQuery, List<ClientResponse>>
{
    public async Task<BaseResponse<List<ClientResponse>>> Handle(
        GetClientsQuery request,
        CancellationToken cancellationToken
    )
    {
        var userRole = TokenHelper.GetUserRole(httpContextAccessor.HttpContext.User);
        if( userRole == Role.SuperAdmin)
        {
            var allClients = await clientRepository.GetAllClientsAsync(request.Top, request.Skip, cancellationToken);
            return new SuccessResponse<List<ClientResponse>>([.. allClients.Select(x => mapper.Map<ClientResponse>(x))]);
        }
        var availableClients = await aclApi.GetUserClientsAsync(cancellationToken);
        var clientIds = availableClients
                            .Where(id => ObjectId.TryParse(id, out _))
                            .Select(ObjectId.Parse)
                            .ToList();
        var clients = await clientRepository.GetClientsByIdsAsync(clientIds, request.Top, request.Skip, cancellationToken);
        if (clients.Count == 0)
            return new BadRequestResponse<List<ClientResponse>>("There is no such document");
        var clientsResponse = clients.Select(x => mapper.Map<ClientResponse>(x)).ToList();
        return new SuccessResponse<List<ClientResponse>>(clientsResponse, "Clients Retrieved");
    }
}
