using System.Net;
using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Queries.invoice;
using ExportPro.StorageService.Models.Models;

namespace ExportPro.StorageService.CQRS.Handlers.invoiceHandler;

public class GetInvoiceByIdHandler(IRepository<Invoice> repository) : IQueryHandler<GetInvoiceByIdQuery, Models.Models.Invoice>
{
    private readonly IRepository<Invoice> _repository = repository;

    public async Task<BaseResponse<Invoice>> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        var invoice = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (invoice == null)
        {
            return new BaseResponse<Invoice>
            {
                ApiState = HttpStatusCode.NotFound,
                IsSuccess = false,
                Messages = new List<string> { "Invoice not found." }
            };
        }

        return new BaseResponse<Models.Models.Invoice>
        {
            ApiState = HttpStatusCode.OK,
            Data = invoice
        };
    }
}

