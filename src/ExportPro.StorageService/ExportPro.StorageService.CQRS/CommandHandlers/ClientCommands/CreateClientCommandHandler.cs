using System.Net;
using System.Security.Claims;
using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace ExportPro.StorageService.CQRS.CommandHandlers.ClientCommands;

public record CreateClientCommand(string Name, string? Description) : ICommand<ClientResponse>;

public class CreateClientCommandHandler(
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
        ClientDto clientDto = new() { Name = request.Name, Description = request.Description };
        var userName = httpContext.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
        var client = mapper.Map<Client>(clientDto);
        client.CreatedBy = userName;
        var created = await clientRepository.AddOneAsync(client, cancellationToken);
        var clientResponse = mapper.Map<ClientResponse>(client);
        return new SuccessResponse<ClientResponse>(clientResponse, "Client Created Successfully");
    }
}
