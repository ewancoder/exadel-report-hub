using ExportPro.StorageService.CQRS.CommandHandlers.CurrencyCommands;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations.CurrencyValidations;

public class UpdateCurrencyCommandValidator : AbstractValidator<UpdateCurrencyCommand>
{
    public UpdateCurrencyCommandValidator()
    {
        RuleFor(x => x.Id.ToString()).IsValidObjectId();
        // Add other validation rules for UpdateCurrencyCommand properties
        RuleFor(x => x.Code).NotEmpty().MaximumLength(3);
    }
}