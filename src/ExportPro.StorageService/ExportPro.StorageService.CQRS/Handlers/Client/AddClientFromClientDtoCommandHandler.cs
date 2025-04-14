using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;

using ExportPro.StorageService.DataAccess.Services;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.Handlers.Client;
public record AddClientFromClientDtoCommand(ClientDto Clientdto) : ICommand<ClientResponse>;

public class AddClientFromClientDtoCommandHandler(IClientService clientService) : ICommandHandler<AddClientFromClientDtoCommand, ClientResponse>
{
    private readonly IClientService _clientService = clientService;
    public async Task<BaseResponse<ClientResponse>> Handle(AddClientFromClientDtoCommand request, CancellationToken cancellationToken)
    {
        var CreatingClient = await _clientService.AddClientFromClientDto(request.Clientdto);

        return new SuccessResponse<ClientResponse>(CreatingClient, message: "Successfully added client");
    }
}