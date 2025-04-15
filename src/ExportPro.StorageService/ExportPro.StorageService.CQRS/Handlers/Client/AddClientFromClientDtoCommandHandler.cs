using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.Handlers.Client;
public record AddClientFromClientDtoCommand(ClientDto Clientdto) : ICommand<ClientResponse>;

public class AddClientFromClientDtoCommandHandler(IClientRepository clientRepository) : ICommandHandler<AddClientFromClientDtoCommand, ClientResponse>
{
    private readonly IClientRepository _clientRepository = clientRepository;
    public async Task<BaseResponse<ClientResponse>> Handle(AddClientFromClientDtoCommand request, CancellationToken cancellationToken)
    {
        var CreatingClient = await _clientRepository.AddClientFromClientDto(request.Clientdto);

        return new SuccessResponse<ClientResponse>(CreatingClient, message: "Successfully added client");
    }
}