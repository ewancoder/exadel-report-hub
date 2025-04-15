using System.Net;
using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.Responses;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ExportPro.StorageService.CQRS.Handlers.Client;
public record GetClientsQuery(int client_size,int page,bool soft_deleted) : IQuery<List<ClientResponse>>;

public class GetClientsQueryHandler(IClientRepository clientRepository,IMapper mapper) : IQueryHandler<GetClientsQuery, List<ClientResponse>>
{
    private readonly IClientRepository _clientRepository=clientRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<BaseResponse<List<ClientResponse>>> Handle(GetClientsQuery request, CancellationToken cancellationToken)
    {
        var clientresponse =  _clientRepository.GetClients(request.client_size,request.page,request.soft_deleted);
        var message = clientresponse.Messages;
        if (message[0] == "There is no such document")
        {
            return new  BaseResponse<List<ClientResponse>> {Messages= message, Data= null, ApiState = HttpStatusCode.BadRequest, IsSuccess = false };
        }
        var clients = await clientresponse.Data;
        var clientresp =clients.Select(x=>_mapper.Map<ClientResponse>(x)).ToList();
        return new SuccessResponse<List<ClientResponse>>(clientresp, message: message[0]);
    }
}