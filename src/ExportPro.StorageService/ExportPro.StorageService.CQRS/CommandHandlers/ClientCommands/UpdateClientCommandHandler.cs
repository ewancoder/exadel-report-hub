using System.Security.Claims;
using AutoMapper;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;
using Microsoft.AspNetCore.Http;

namespace ExportPro.StorageService.CQRS.CommandHandlers.ClientCommands;

public record UpdateClientCommand(ClientDto Client, Guid ClientId) : ICommand<ClientResponse>;

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
        if (client == null)
            return new NotFoundResponse<ClientResponse>("Client Not Found");
        client.Name = request.Client.Name;
        if (request.Client.Description != null)
            client.Description = request.Client.Description;
        client.UpdatedBy = httpContext.HttpContext?.User.FindFirst(ClaimTypes.Name)!.Value;
        client.UpdatedAt = DateTime.UtcNow;
        await clientRepository.UpdateOneAsync(client, cancellationToken);
        var clientResponse = mapper.Map<ClientResponse>(client);
        return new SuccessResponse<ClientResponse>(clientResponse, "Client Updated Successfully");
    }
}
