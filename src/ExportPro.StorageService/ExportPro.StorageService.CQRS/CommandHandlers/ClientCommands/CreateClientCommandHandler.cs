using System.Security.Claims;
using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;
using Microsoft.AspNetCore.Http;

namespace ExportPro.StorageService.CQRS.CommandHandlers.ClientCommands;

public sealed record CreateClientCommand(ClientDto ClientDto) : ICommand<ClientResponse>;

public sealed class CreateClientCommandHandler(
    IClientRepository clientRepository,
    IMapper mapper,
    IHttpContextAccessor httpContext
) : ICommandHandler<CreateClientCommand, ClientResponse>
{
    public async Task<BaseResponse<ClientResponse>> Handle(
        CreateClientCommand request,
        CancellationToken cancellationToken
    )
    {
        ClientDto clientDto = new() { Name = request.ClientDto.Name, Description = request.ClientDto.Description };
        var userName = httpContext.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value;
        var client = mapper.Map<Client>(clientDto);
        client.CreatedBy = userName;
        await clientRepository.AddOneAsync(client, cancellationToken);
        var clientResponse = mapper.Map<ClientResponse>(client);
        return new SuccessResponse<ClientResponse>(clientResponse, "Client Created Successfully");
    }
}
