using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Commands;
using ExportPro.StorageService.DataAccess.Services;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Handlers;

public class DeleteClientCommandHandler(IClientService clientService):ICommandHandler<DeleteClientCommand,string>
{
    private readonly IClientService _clientService=clientService;
    public async Task<BaseResponse<string>> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        var message = await _clientService.DeleteClient(request.ClientId);
        if (message == null) return null;
        return new SuccessResponse<string>(message);
    }
}