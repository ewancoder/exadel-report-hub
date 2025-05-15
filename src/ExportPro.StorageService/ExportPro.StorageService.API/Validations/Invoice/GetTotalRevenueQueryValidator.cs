using ExportPro.StorageService.CQRS.QueryHandlers.InvoiceQueries;
using FluentValidation;

namespace ExportPro.StorageService.Api.Validations.Invoice;

public class GetTotalRevenueQueryValidator : AbstractValidator<GetTotalRevenueQuery>
{
    public GetTotalRevenueQueryValidator()
    {
        RuleFor(x => x.RevenueDto.StartDate)
            .LessThanOrEqualTo(x => x.RevenueDto.EndDate)
            .WithMessage("StartDate must be before EndDate.");
    }
}
