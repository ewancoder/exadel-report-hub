using System.Net;
using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ExportPro.StorageService.CQRS.QueryHandlers.Client;

public record GetClientsQuery(int top, int skip) : IQuery<ValidationModel<List<ClientResponse>>>;

public class GetClientsQueryHandler(
    IClientRepository clientRepository,
    IMapper mapper,
    IValidator<GetClientsQuery> validator
) : IQueryHandler<GetClientsQuery, ValidationModel<List<ClientResponse>>>
{
    private readonly IClientRepository _clientRepository = clientRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IValidator<GetClientsQuery> _validator = validator;

    public async Task<BaseResponse<ValidationModel<List<ClientResponse>>>> Handle(
        GetClientsQuery request,
        CancellationToken cancellationToken
    )
    {
      
        var clientresponse = _clientRepository.GetClients(request.top, request.skip);
        var message = clientresponse.Messages;
        if (message[0] == "There is no such document")
        {
            return new BaseResponse<ValidationModel<List<ClientResponse>>>
            {
                Data = new(),
                ApiState = HttpStatusCode.BadRequest,
                IsSuccess = false,
                Messages = message,
            };
        }
        var clients = await clientresponse.Data;
        var plans = new List<Models.Models.Plans>();
        foreach (var client in clients)
        {
            foreach (var j in client.Plans)
            {
                if (!j.IsDeleted)
                {
                    plans.Add(j);
                }
            }
            client.Plans = plans;
        }
        var clientresp = clients.Select(x => _mapper.Map<ClientResponse>(x)).ToList();
        return new BaseResponse<ValidationModel<List<ClientResponse>>>
        {
            Data = new(clientresp),
            ApiState = HttpStatusCode.OK,
            IsSuccess = true,
            Messages = message,
        };
    }
}
