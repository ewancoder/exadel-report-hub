using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.Handlers.Client;
public record UpdateClientCommand(ClientUpdateDto clientUpdateDto, string ClientId) : ICommand<ClientResponse>;

public class UpdateClientCommandHandler(IClientRepository clientRepository) : ICommandHandler<UpdateClientCommand, ClientResponse>
{
    IClientRepository _clientRepository = clientRepository;
    public async Task<BaseResponse<ClientResponse>> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        var update = await clientRepository.UpdateClient(request.clientUpdateDto, request.ClientId);
        return new SuccessResponse<ClientResponse>(update);
    }
}