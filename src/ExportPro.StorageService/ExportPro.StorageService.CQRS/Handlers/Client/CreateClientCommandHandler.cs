using System.Net;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ExportPro.StorageService.CQRS.Handlers.Client;

public record CreateClientCommand(ClientDto Clientdto) : ICommand<ValidationModel<ClientResponse>>;

public class CreateClientCommandHandler(IClientRepository clientRepository, IValidator<CreateClientCommand> validator)
    : ICommandHandler<CreateClientCommand, ValidationModel<ClientResponse>>
{
    private readonly IClientRepository _clientRepository = clientRepository;

    public async Task<BaseResponse<ValidationModel<ClientResponse>>> Handle(
        CreateClientCommand request,
        CancellationToken cancellationToken
    )
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new BaseResponse<ValidationModel<ClientResponse>>
            {
                Data = new(validationResult),
                ApiState = HttpStatusCode.BadRequest,
                IsSuccess = false,
            };
        }
        var CreatingClient = await _clientRepository.AddClientFromClientDto(request.Clientdto);
        return new BaseResponse<ValidationModel<ClientResponse>>
        {
            Data = new(CreatingClient),
            Messages = ["Client Created Successfully"],
            ApiState = HttpStatusCode.Created,
            IsSuccess = true,
        };
    }
}
