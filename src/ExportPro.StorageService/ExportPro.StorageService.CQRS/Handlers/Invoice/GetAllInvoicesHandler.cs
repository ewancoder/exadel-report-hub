using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Queries.Invoice;
using ExportPro.StorageService.DataAccess.Interfaces;
using System.Net;

namespace ExportPro.StorageService.CQRS.Handlers.Invoice;

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

