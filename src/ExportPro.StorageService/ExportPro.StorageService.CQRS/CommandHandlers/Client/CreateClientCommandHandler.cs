using System.Net;
using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;
using FluentValidation;

namespace ExportPro.StorageService.CQRS.CommandHandlers.Client;

public record CreateClientCommand(ClientDto Clientdto) : ICommand<ValidationModel<ClientResponse>>;

public class CreateClientCommandHandler(
    IClientRepository clientRepository,
    IValidator<CreateClientCommand> validator,
    IMapper _mapper
) : ICommandHandler<CreateClientCommand, ValidationModel<ClientResponse>>
{
    private readonly IClientRepository _clientRepository = clientRepository;
    private readonly IValidator<CreateClientCommand> _validator = validator;

    public async Task<BaseResponse<ValidationModel<ClientResponse>>> Handle(
        CreateClientCommand request,
        CancellationToken cancellationToken
    )
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new BaseResponse<ValidationModel<ClientResponse>>
            {
                Data = new(validationResult),
                ApiState = HttpStatusCode.BadRequest,
                IsSuccess = false,
            };
        }
        var creatingClient = await _clientRepository.AddClientFromClientDto(request.Clientdto);
        return new BaseResponse<ValidationModel<ClientResponse>>
        {
            Data = new(creatingClient),
            Messages = ["Client Created Successfully"],
            ApiState = HttpStatusCode.Created,
            IsSuccess = true,
        };
    }
}
