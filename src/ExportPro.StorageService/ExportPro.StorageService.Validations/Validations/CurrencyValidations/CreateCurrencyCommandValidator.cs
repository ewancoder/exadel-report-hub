using ExportPro.StorageService.CQRS.CommandHandlers.CurrencyCommands;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations.CurrencyValidations;

public class CreateCurrencyCommandValidator : AbstractValidator<CreateCurrencyCommand>
{
    public CreateCurrencyCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Currency code is required.")
            .Length(3, 3).WithMessage("Currency code must be between 3 and 3 characters.");
    }
}