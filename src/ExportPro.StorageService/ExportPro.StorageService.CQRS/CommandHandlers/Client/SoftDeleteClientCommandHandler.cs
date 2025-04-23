using System.Net;
using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;
using FluentValidation;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.CommandHandlers.Client;

public record SoftDeleteClientCommand(string ClientId) : ICommand<ValidationModel<ClientResponse>>;

public class SoftDeleteClientCommandHandler(
    IClientRepository clientRepository,
    IMapper mapper,
    IValidator<SoftDeleteClientCommand> validator
) : ICommandHandler<SoftDeleteClientCommand, ValidationModel<ClientResponse>>
{
    private readonly IClientRepository _clientRepository = clientRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IValidator<SoftDeleteClientCommand> _validator = validator;

    public async Task<BaseResponse<ValidationModel<ClientResponse>>> Handle(
        SoftDeleteClientCommand request,
        CancellationToken cancellationToken
    )
    {
        var validationResult = await _validator.ValidateAsync(request, CancellationToken.None);
        if (!validationResult.IsValid)
        {
            return new BaseResponse<ValidationModel<ClientResponse>>
            {
                Data = new ValidationModel<ClientResponse>(validationResult),
                ApiState = HttpStatusCode.BadRequest,
                IsSuccess = false,
            };
        }
        var client = await _clientRepository.GetClientById(request.ClientId);
        await _clientRepository.SoftDeleteClient(request.ClientId);
        return new BaseResponse<ValidationModel<ClientResponse>>
        {
            Data = new(_mapper.Map<ClientResponse>(client)),
            ApiState = HttpStatusCode.OK,
            Messages = ["Successfully deleted"],
            IsSuccess = true,
        };
    }
}
