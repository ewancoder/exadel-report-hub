using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Commands;
using ExportPro.StorageService.DataAccess.Services;

namespace ExportPro.StorageService.CQRS.Handlers;

public class SoftDeleteClientCommandHandler(IClientService clientService):ICommandHandler<SoftDeleteClientCommand, string>
{
    private IClientService _clientService = clientService;
    public async Task<BaseResponse<string>> Handle(SoftDeleteClientCommand request, CancellationToken cancellationToken)
    {
        var res = await _clientService.SoftDeleteClient(request.ClientId);
        return new SuccessResponse<string>(res);
    }
}