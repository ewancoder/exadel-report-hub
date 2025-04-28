using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using MongoDB.Driver;
using System.Net;

namespace ExportPro.StorageService.CQRS.QueryHandlers.InvoiceQueries;

public class GetTotalInvoicesQuery : IQuery<long>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? ClientId { get; set; }
    public string? CustomerId { get; set; }
}
public class GetTotalInvoicesHandler(
    IInvoiceRepository invoiceRepository
) : IQueryHandler<GetTotalInvoicesQuery, long>
{
    private readonly IInvoiceRepository _invoiceRepository = invoiceRepository;

    public async Task<BaseResponse<long>> Handle(GetTotalInvoicesQuery request, CancellationToken cancellationToken)
    {
        var filter = Builders<Invoice>.Filter.Gte(x => x.IssueDate, request.StartDate) &
                     Builders<Invoice>.Filter.Lte(x => x.IssueDate, request.EndDate);

        if (!string.IsNullOrEmpty(request.ClientId))
        {
            filter &= Builders<Invoice>.Filter.Eq(x => x.ClientId, request.ClientId);
        }

        if (!string.IsNullOrEmpty(request.CustomerId))
        {
            filter &= Builders<Invoice>.Filter.Eq(x => x.CustomerId, request.CustomerId);
        }

        var count = await _invoiceRepository.CountAsync(filter, cancellationToken);

        if (count == 0)
        {
            return new BaseResponse<long>
            {
                Data = 0,
                IsSuccess = true,
                ApiState = HttpStatusCode.OK,
                Messages = ["No invoices issued in selected period."]
            };
        }

        return new BaseResponse<long>
        {
            Data = count,
            IsSuccess = true,
            ApiState = HttpStatusCode.OK,
            Messages = ["Total invoices fetched successfully."]
        };
    }
}