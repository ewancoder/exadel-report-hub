using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;

using ExportPro.StorageService.DataAccess.Services;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Handlers.Client;
public record SoftDeleteClientCommand(ObjectId ClientId) : ICommand<string>;

public class SoftDeleteClientCommandHandler(IClientService clientService) : ICommandHandler<SoftDeleteClientCommand, string>
{
    private IClientService _clientService = clientService;
    public async Task<BaseResponse<string>> Handle(SoftDeleteClientCommand request, CancellationToken cancellationToken)
    {
        var res = await _clientService.SoftDeleteClient(request.ClientId);
        return new SuccessResponse<string>(res);
    }
}