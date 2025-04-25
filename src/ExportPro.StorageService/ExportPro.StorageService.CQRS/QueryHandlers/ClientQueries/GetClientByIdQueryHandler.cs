using System.Net;
using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;
using FluentValidation;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.QueryHandlers.ClientQueries;

public record GetClientByIdQuery(string ClientId) : IQuery<ClientResponse>;

public class GetClientByIdQueryHandler(IClientRepository clientRepository, IMapper mapper)
    : IQueryHandler<GetClientByIdQuery, ClientResponse>
{
    public async Task<BaseResponse<ClientResponse>> Handle(
        GetClientByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var client = await clientRepository.GetOneAsync(
            x => x.Id == ObjectId.Parse(request.ClientId) && !x.IsDeleted,
            cancellationToken
        );
        var clientResponse = mapper.Map<ClientResponse>(client);
        return new SuccessResponse<ClientResponse>(clientResponse, "Client Retrieved");
    }
}
