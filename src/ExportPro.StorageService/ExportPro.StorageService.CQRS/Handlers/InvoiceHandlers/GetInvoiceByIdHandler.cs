using System.Net;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Queries.InvoiceQueries;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;

namespace ExportPro.StorageService.CQRS.Handlers.InvoiceHandlers;

public class GetInvoiceByIdHandler(IInvoiceRepository repository) : IQueryHandler<GetInvoiceByIdQuery, Invoice>
{
    private readonly IInvoiceRepository _repository = repository;

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

        return new BaseResponse<Invoice>
        {
            ApiState = HttpStatusCode.OK,
            Data = invoice
        };
    }
}

