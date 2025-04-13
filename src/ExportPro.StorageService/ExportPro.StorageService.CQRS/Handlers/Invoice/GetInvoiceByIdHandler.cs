using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Queries.Invoice;
using System.Net;

namespace ExportPro.StorageService.CQRS.Handlers.Invoice;

public class GetInvoiceByIdHandler(IRepository<Models.Models.Invoice> repository) : IQueryHandler<GetInvoiceByIdQuery, Models.Models.Invoice>
{
    private readonly IRepository<Models.Models.Invoice> _repository = repository;

    public async Task<BaseResponse<Models.Models.Invoice>> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        var invoice = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (invoice == null)
        {
            return new BaseResponse<Models.Models.Invoice>
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

