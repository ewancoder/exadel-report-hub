using System.Net;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Queries.InvoiceQueries;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;

namespace ExportPro.StorageService.CQRS.Handlers.InvoiceHandlers;

public class GetAllInvoicesHandler(IInvoiceRepository repository) : IQueryHandler<GetAllInvoicesQuery, List<Invoice>>
{
    private readonly IInvoiceRepository _repository = repository;

    public async Task<BaseResponse<List<Invoice>>> Handle(GetAllInvoicesQuery request, CancellationToken cancellationToken)
    {
        var all = await _repository.GetAllAsync(cancellationToken);
        return new BaseResponse<List<Invoice>>
        {
            ApiState = HttpStatusCode.OK,
            Data = all
        };
    }
}

