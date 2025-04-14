using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;

using ExportPro.StorageService.DataAccess.Services;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.Handlers.Client;
public record UpdateClientCommand(ClientUpdateDto clientUpdateDto, string ClientId) : ICommand<ClientResponse>;

public class UpdateClientCommandHandler(IClientService clientService) : ICommandHandler<UpdateClientCommand, ClientResponse>
{
    IClientService _clientService = clientService;
    public async Task<BaseResponse<ClientResponse>> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        var update = await clientService.UpdateClient(request.clientUpdateDto, request.ClientId);
        return new SuccessResponse<ClientResponse>(update);
    }
}