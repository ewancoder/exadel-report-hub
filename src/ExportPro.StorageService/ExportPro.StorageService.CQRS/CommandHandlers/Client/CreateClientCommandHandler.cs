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

public record CreateClientCommand(ClientDto Client) : ICommand<ClientResponse>;

public class CreateClientCommandHandler(
    IClientRepository clientRepository,
    IValidator<CreateClientCommand> validator,
    IMapper _mapper
) : ICommandHandler<CreateClientCommand, ClientResponse>
{
    private readonly IClientRepository _clientRepository = clientRepository;
    public async Task<BaseResponse<ClientResponse>> Handle(
        CreateClientCommand request,
        CancellationToken cancellationToken
    )
    {
        var creatingClient = await _clientRepository.AddClientFromClientDto(request.Client);
        return new BaseResponse<ClientResponse>
        {
            Data = creatingClient,
            Messages = ["Client Created Successfully"],
            ApiState = HttpStatusCode.Created,
            IsSuccess = true,
        };
    }
}