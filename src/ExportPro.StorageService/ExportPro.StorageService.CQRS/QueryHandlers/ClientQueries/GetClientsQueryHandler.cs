using System.Net;
using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ExportPro.StorageService.CQRS.QueryHandlers.ClientQueries;

public sealed record GetClientsQuery(int Top, int Skip) : IQuery<List<ClientResponse>>;

public sealed class GetClientsQueryHandler(IClientRepository clientRepository, IMapper mapper)
    : IQueryHandler<GetClientsQuery, List<ClientResponse>>
{
    public async Task<BaseResponse<List<ClientResponse>>> Handle(
        GetClientsQuery request,
        CancellationToken cancellationToken
    )
    {
        var clients = await clientRepository.GetClients(request.Top, request.Skip);
        var clientsResponse = clients.Select(x => mapper.Map<ClientResponse>(x)).ToList();
        return new SuccessResponse<List<ClientResponse>>(clientsResponse, "Clients Retrieved");
    }
}
