using ExportPro.StorageService.Models.Models;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations.CurrencyValidations;

public class CreateCurrencyCommandValidator : AbstractValidator<Currency>
{
    public CreateCurrencyCommandValidator()
    {
        RuleFor(x => x.CurrencyCode)
            .NotEmpty().WithMessage("Currency code is required.")
            .Length(3, 5).WithMessage("Currency code must be between 3 and 5 characters.");

        RuleFor(x => x.CreatedAt)
            .NotEmpty().WithMessage("CreatedAt is required.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("CreatedAt cannot be in the future.");

        RuleFor(x => x.UpdatedAt)
            .Must((currency, updatedAt) =>
                !updatedAt.HasValue || updatedAt >= currency.CreatedAt)
            .WithMessage("UpdatedAt cannot be earlier than CreatedAt.");
    }
}