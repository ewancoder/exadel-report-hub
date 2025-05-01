using System.Net;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.CommandHandlers.InvoiceCommands;

public sealed record DeleteInvoiceCommand(Guid Id) : ICommand<bool>;

public sealed class DeleteInvoiceHandler(IInvoiceRepository repository) : ICommandHandler<DeleteInvoiceCommand, bool>
{
    public async Task<BaseResponse<bool>> Handle(DeleteInvoiceCommand request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
        {
            return new BadRequestResponse<bool>("Invalid invoice ID.");
        }
        var invoice = await repository.GetByIdAsync(request.Id.ToObjectId(), cancellationToken);
        if (invoice == null)
        {
            return new NotFoundResponse<bool>("Invoice not found.");
        }
        await repository.SoftDeleteAsync(request.Id.ToObjectId(), cancellationToken);
        return new SuccessResponse<bool>(true, "Invoice deleted successfully.");
    }
}
