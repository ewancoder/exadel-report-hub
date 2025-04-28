namespace ExportPro.StorageService.Validations.Validations.Invoice;

using ExportPro.StorageService.CQRS.QueryHandlers.InvoiceQueries;
using FluentValidation;

public class GetInvoiceCountQueryValidator : AbstractValidator<GetTotalInvoicesQuery>
{
    public GetInvoiceCountQueryValidator()
    {
        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Start date is required.");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("End date is required.");

        RuleFor(x => x)
            .Must(x => x.EndDate >= x.StartDate)
            .WithMessage("End date must be greater than or equal to start date.");

        RuleFor(x => x.ClientId)
            .Must(BeValidObjectId)
            .When(x => !string.IsNullOrEmpty(x.ClientId))
            .WithMessage("ClientId must be a valid ObjectId.");

        RuleFor(x => x.CustomerId)
            .Must(BeValidObjectId)
            .When(x => !string.IsNullOrEmpty(x.CustomerId))
            .WithMessage("CustomerId must be a valid ObjectId.");
    }

    private bool BeValidObjectId(string id)
    {
        return MongoDB.Bson.ObjectId.TryParse(id, out _);
    }
}

