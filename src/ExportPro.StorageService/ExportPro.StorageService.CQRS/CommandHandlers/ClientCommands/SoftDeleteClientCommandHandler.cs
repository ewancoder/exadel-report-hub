using System.Net;
using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;
using FluentValidation;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.CommandHandlers.ClientCommands;

public record SoftDeleteClientCommand(ObjectId ClientId) : ICommand<ClientResponse>;

public class SoftDeleteClientCommandHandler(IClientRepository clientRepository, IMapper mapper)
    : ICommandHandler<SoftDeleteClientCommand, ClientResponse>
{
    public async Task<BaseResponse<ClientResponse>> Handle(
        SoftDeleteClientCommand request,
        CancellationToken cancellationToken
    )
    {
        var clientDeleted = await clientRepository.SoftDeleteAsync(request.ClientId, cancellationToken);
        var clientResponse = mapper.Map<ClientResponse>(clientDeleted);
        return new SuccessResponse<ClientResponse>(clientResponse, "Client Deleted Successfully");
    }
}
