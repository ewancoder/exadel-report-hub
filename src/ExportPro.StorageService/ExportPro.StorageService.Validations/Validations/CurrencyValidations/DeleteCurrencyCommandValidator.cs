using ExportPro.StorageService.CQRS.CommandHandlers.CurrencyCommands;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations.CurrencyValidations;

public class DeleteCurrencyCommandValidator : AbstractValidator<DeleteCurrencyCommand>
{
    public DeleteCurrencyCommandValidator()
    {
        RuleFor(x => x.Id.ToString()).IsValidObjectId();
    }
}
