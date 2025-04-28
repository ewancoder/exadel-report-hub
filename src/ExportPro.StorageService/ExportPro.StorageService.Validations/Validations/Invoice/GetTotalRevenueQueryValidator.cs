namespace ExportPro.StorageService.Validations.Validations.Invoice;

using ExportPro.StorageService.CQRS.QueryHandlers.InvoiceQueries;
using FluentValidation;

public class GetTotalRevenueQueryValidator : AbstractValidator<GetTotalRevenueQuery>
{
    public GetTotalRevenueQueryValidator()
    {
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate)
            .NotEmpty()
            .GreaterThan(x => x.StartDate)
            .WithMessage("EndDate must be greater than StartDate.");
    }
}
