using AutoMapper;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Helpers;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.Responses;


namespace ExportPro.StorageService.CQRS.CommandHandlers.ClientCommands;

public sealed record SoftDeleteClientCommand(Guid ClientId) : ICommand<ClientResponse>, IPermissionedRequest
{
    public List<Guid>? ClientIds => [ClientId];

    public Resource Resource => Resource.Clients;

    public CrudAction Action => CrudAction.Delete;
}

public sealed class SoftDeleteClientCommandHandler(IClientRepository clientRepository, IMapper mapper)
    : ICommandHandler<SoftDeleteClientCommand, ClientResponse>
{
    public async Task<BaseResponse<ClientResponse>> Handle(
        SoftDeleteClientCommand request,
        CancellationToken cancellationToken
    )
    {
        var client = await clientRepository.GetOneAsync(
            x => x.Id == request.ClientId.ToObjectId() && !x.IsDeleted,
            cancellationToken
        );
        if (client == null)
            return new NotFoundResponse<ClientResponse>("Client Not Found");
        var clientDeleted = await clientRepository.SoftDeleteAsync(request.ClientId.ToObjectId(), cancellationToken);
        var clientResponse = mapper.Map<ClientResponse>(clientDeleted);
        return new SuccessResponse<ClientResponse>(clientResponse, "Client Deleted Successfully");
    }
}