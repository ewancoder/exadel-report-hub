using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.Responses;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.CommandHandlers.ClientCommands;

public sealed record SoftDeleteClientCommand(ObjectId ClientId) : ICommand<ClientResponse>;

public sealed class SoftDeleteClientCommandHandler(IClientRepository clientRepository, IMapper mapper)
    : ICommandHandler<SoftDeleteClientCommand, ClientResponse>
{
    public async Task<BaseResponse<ClientResponse>> Handle(
        SoftDeleteClientCommand request,
        CancellationToken cancellationToken
    )
    {
        var client = await clientRepository.GetOneAsync(
            x => x.Id == request.ClientId && !x.IsDeleted,
            cancellationToken
        );
        if (client == null)
            return new NotFoundResponse<ClientResponse>("Client Not Found");
        var clientDeleted = await clientRepository.SoftDeleteAsync(request.ClientId, cancellationToken);
        var clientResponse = mapper.Map<ClientResponse>(clientDeleted);
        return new SuccessResponse<ClientResponse>(clientResponse, "Client Deleted Successfully");
    }
}
