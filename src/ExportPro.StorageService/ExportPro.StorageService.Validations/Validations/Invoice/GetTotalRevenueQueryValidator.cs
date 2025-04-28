using ExportPro.StorageService.CQRS.QueryHandlers.InvoiceQueries;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations.Invoice;

public class GetTotalRevenueQueryValidator : AbstractValidator<GetTotalRevenueQuery>
{
    public GetTotalRevenueQueryValidator()
    {
        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate)
            .WithMessage("StartDate must be before EndDate.");
    }
}