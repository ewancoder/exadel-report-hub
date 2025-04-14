using System.Net;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Queries.invoice;
using ExportPro.StorageService.DataAccess.Interfaces;

namespace ExportPro.StorageService.CQRS.Handlers.invoiceHandler;

public class GetAllInvoicesHandler(IInvoiceRepository repository) : IQueryHandler<GetAllInvoicesQuery, List<Models.Models.Invoice>>
{
    private readonly IInvoiceRepository _repository = repository;

    public async Task<BaseResponse<List<Models.Models.Invoice>>> Handle(GetAllInvoicesQuery request, CancellationToken cancellationToken)
    {
        var all = await _repository.GetAllAsync(cancellationToken);
        return new BaseResponse<List<Models.Models.Invoice>>
        {
            ApiState = HttpStatusCode.OK,
            Data = all
        };
    }
}

