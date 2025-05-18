using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Helpers;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;

namespace ExportPro.StorageService.CQRS.CommandHandlers.InvoiceCommands;

public sealed record DeleteInvoiceCommand(Guid Id) : ICommand<bool>, IPermissionedRequest
{
    public List<Guid>? ClientIds { get; init; } = null;
    public Resource Resource { get; init; } = Resource.Invoices;
    public CrudAction Action { get; init; } = CrudAction.Delete;
};

public sealed class DeleteInvoiceHandler(IInvoiceRepository repository) : ICommandHandler<DeleteInvoiceCommand, bool>
{
    public async Task<BaseResponse<bool>> Handle(DeleteInvoiceCommand request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
            return new BadRequestResponse<bool>("Invalid invoice ID.");
        var invoice = await repository.GetOneAsync(
            x => x.Id == request.Id.ToObjectId() && !x.IsDeleted,
            cancellationToken
        );
        if (invoice == null)
            return new NotFoundResponse<bool>("Invoice not found.");
        await repository.SoftDeleteAsync(request.Id.ToObjectId(), cancellationToken);
        return new SuccessResponse<bool>(true, "Invoice deleted successfully.");
    }
}
