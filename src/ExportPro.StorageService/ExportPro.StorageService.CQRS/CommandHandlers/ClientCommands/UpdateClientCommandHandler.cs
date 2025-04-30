using System.Net;
using System.Security.Claims;
using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.CommandHandlers.ClientCommands;

public record UpdateClientCommand(ClientDto client, Guid ClientId) : ICommand<ClientResponse>;

public sealed class UpdateClientCommandHandler(
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
            x => x.Id == request.ClientId.ToObjectId() && !x.IsDeleted,
            cancellationToken
        );
        if (request.client.Name != null)
            client.Name = request.client.Name;
        if (request.client.Description != null)
            client.Description = request.client.Description;
        client.UpdatedBy = httpContext.HttpContext?.User?.FindFirst(ClaimTypes.Name).Value;
        client.UpdatedAt = DateTime.UtcNow;
        await clientRepository.UpdateOneAsync(client, cancellationToken);
        var clientResponse = mapper.Map<ClientResponse>(client);
        return new SuccessResponse<ClientResponse>(clientResponse, "Client Updated Successfully");
    }
}
