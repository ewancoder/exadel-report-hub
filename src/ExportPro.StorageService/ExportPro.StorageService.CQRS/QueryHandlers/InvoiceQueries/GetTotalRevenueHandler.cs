using System.Net;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;

namespace ExportPro.StorageService.CQRS.QueryHandlers.InvoiceQueries;

public sealed class GetTotalRevenueQuery : IQuery<double>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public sealed class GetTotalRevenueHandler(
    IInvoiceRepository repository
) : IQueryHandler<GetTotalRevenueQuery, double>
{
    private readonly IInvoiceRepository _repository = repository;

    public async Task<BaseResponse<double>> Handle(GetTotalRevenueQuery request, CancellationToken cancellationToken)
    {
        var invoices = await _repository.GetInvoicesInDateRangeAsync(request.StartDate, request.EndDate);

        if (invoices == null || !invoices.Any())
        {
            return new BaseResponse<double>
            {
                Data = 0,
                IsSuccess = true,
                ApiState = HttpStatusCode.OK,
                Messages = ["No invoices issued in selected period."]
            };
        }

        var totalRevenue = invoices.Sum(x => x.Amount ?? 0);

        return new BaseResponse<double>
        {
            Data = totalRevenue,
            IsSuccess = true,
            ApiState = HttpStatusCode.OK,
            Messages = ["Total revenue calculated successfully."]
        };
    }
}