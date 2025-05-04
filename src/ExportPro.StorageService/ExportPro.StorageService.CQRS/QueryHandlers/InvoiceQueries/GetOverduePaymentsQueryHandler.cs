using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.QueryHandlers.InvoiceQueries;


public record GetOverduePaymentsQuery: IQuery<OverduePaymentsResponse>;

public sealed class GetOverduePaymentsQueryHandler(IInvoiceRepository invoiceRepository) : IQueryHandler<GetOverduePaymentsQuery, OverduePaymentsResponse>
{
    public async Task<BaseResponse<OverduePaymentsResponse>> Handle(GetOverduePaymentsQuery request, CancellationToken cancellationToken)
    {
        var overdueInvoices = await invoiceRepository.GetOverdueInvoices(cancellationToken);
        if(overdueInvoices == null || overdueInvoices.Count == 0)
            return new BadRequestResponse<OverduePaymentsResponse>("No invoices issued in selected period.");
        var result = new OverduePaymentsResponse
        {
            OverdueInvoicesCount = overdueInvoices.Count,
            TotalOverdueAmount = overdueInvoices.Sum(x => x.Amount)
        };
        return new SuccessResponse<OverduePaymentsResponse>(result);
    }
}

