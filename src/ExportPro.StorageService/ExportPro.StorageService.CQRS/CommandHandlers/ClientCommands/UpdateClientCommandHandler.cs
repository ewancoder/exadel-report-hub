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
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.CommandHandlers.ClientCommands;

public record UpdateClientCommand(string Name, string? Description, string clientId) : ICommand<ClientResponse>;

public class UpdateClientCommandHandler(
    IHttpContextAccessor httpContext,
    IClientRepository clientRepository,
    IMapper mapper
) : ICommandHandler<UpdateClientCommand, ClientResponse>
{
    public async Task<BaseResponse<ClientResponse>> Handle(
        UpdateClientCommand request,
        CancellationToken cancellationToken
    )
    {
        var client = await clientRepository.GetOneAsync(
            x => x.Id == ObjectId.Parse(request.clientId),
            cancellationToken
        );
        if (request.Name != null)
            client.Name = request.Name;
        if (request.Description != null)
            client.Description = request.Description;
        client.UpdatedBy = httpContext.HttpContext?.User?.FindFirst(ClaimTypes.Name).Value;
        client.UpdatedAt = DateTime.UtcNow;
        var clientResponse = mapper.Map<ClientResponse>(client);
        return new SuccessResponse<ClientResponse>(clientResponse, "Client Updated Successfully");
    }
}
