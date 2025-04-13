using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Commands.Invoice;
using System.Net;

namespace ExportPro.StorageService.CQRS.Handlers.Invoice;

public class DeleteInvoiceHandler(IRepository<Models.Models.Invoice> repository) : ICommandHandler<DeleteInvoiceCommand, bool>
{
    private readonly IRepository<Models.Models.Invoice> _repository = repository;

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

