using System.Net;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;

namespace ExportPro.StorageService.CQRS.QueryHandlers.InvoiceQueries;

public class GetTotalRevenueQuery : IQuery<double>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class GetTotalRevenueHandler(
    IInvoiceRepository invoiceRepository,
    IValidator<GetTotalRevenueQuery> validator
) : IQueryHandler<GetTotalRevenueQuery, double>
{
    private readonly IInvoiceRepository _invoiceRepository = invoiceRepository;
    private readonly IValidator<GetTotalRevenueQuery> _validator = validator;

    public async Task<BaseResponse<double>> Handle(GetTotalRevenueQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new BaseResponse<double>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.BadRequest,
                Messages = validationResult.Errors.Select(x => x.ErrorMessage).ToList()
            };
        }

        var invoices = await _invoiceRepository.GetInvoicesInDateRangeAsync(request.StartDate, request.EndDate);

        if (invoices == null || !invoices.Any())
        {
            return new BaseResponse<double>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.NotFound,
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
