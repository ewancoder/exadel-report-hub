using System.Net;
using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;
using FluentValidation;

namespace ExportPro.StorageService.CQRS.QueryHandlers.Client;

public record GetClientByIdQuery(string Id) : IQuery<ValidationModel<ClientResponse>>;

public class GetClientByIdQueryHandler(
    IClientRepository clientRepository,
    IMapper mapper,
    IValidator<GetClientByIdQuery> validator
) : IQueryHandler<GetClientByIdQuery, ValidationModel<ClientResponse>>
{
    private readonly IClientRepository _clientRepository = clientRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IValidator<GetClientByIdQuery> _validator = validator;

    public async Task<BaseResponse<ValidationModel<ClientResponse>>> Handle(
        GetClientByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new BaseResponse<ValidationModel<ClientResponse>>
            {
                Data = new ValidationModel<ClientResponse>(validationResult),
                ApiState = HttpStatusCode.BadRequest,
                IsSuccess = false,
            };
        }
        var client = await _clientRepository.GetClientById(request.Id);
        var plans = new List<Models.Models.Plans>();
        foreach (var i in client.Plans)
        {
            if (!i.IsDeleted)
            {
                plans.Add(i);
            }
        }
        client.Plans = plans;
        var clientrep = _mapper.Map<ClientResponse>(client);
        return new BaseResponse<ValidationModel<ClientResponse>>
        {
            Data = new(clientrep),
            Messages = ["Client Retrieved Successfully"],
            ApiState = HttpStatusCode.Created,
            IsSuccess = true,
        };
    }
}
