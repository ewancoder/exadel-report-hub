using System.Net;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Commands.InvoiceCommands;
using ExportPro.StorageService.DataAccess.Interfaces;

namespace ExportPro.StorageService.CQRS.Handlers.InvoiceHandlers;

public class DeleteInvoiceHandler(IInvoiceRepository repository) : ICommandHandler<DeleteInvoiceCommand, bool>
{
    private readonly IInvoiceRepository _repository = repository;

    public async Task<BaseResponse<bool>> Handle(DeleteInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (invoice == null)
        {
            return new BaseResponse<bool>
            {
                ApiState = HttpStatusCode.NotFound,
                IsSuccess = false,
                Data = false,
                Messages = new List<string> { "Invoice not found." }
            };
        }

        await _repository.SoftDeleteAsync(request.Id, cancellationToken);
        return new BaseResponse<bool>
        {
            ApiState = HttpStatusCode.OK,
            Data = true,
            Messages = new List<string> { "Invoice deleted successfully." }
        };
    }
}

