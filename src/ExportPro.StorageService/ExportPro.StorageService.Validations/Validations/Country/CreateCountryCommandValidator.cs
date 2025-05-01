using ExportPro.StorageService.CQRS.CommandHandlers.CountryCommands;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations.Country;

public sealed class CreateCountryCommandValidator : AbstractValidator<CreateCountryCommand>
{
    public CreateCountryCommandValidator(ICurrencyRepository currencyRepository)
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("The Country name is required");
        RuleFor(x => x.CurrencyId)
            .NotEmpty()
            .WithMessage("The Currency Id is required")
            .DependentRules(
                () =>
                    RuleFor(x => x.CurrencyId)
                        .MustAsync(
                            async (currency, CancellationToken_) =>
                            {
                                var client = await currencyRepository.GetByIdAsync(
                                    currency.ToObjectId(),
                                    CancellationToken_
                                );
                                return client != null;
                            }
                        )
                        .WithMessage("The Currency Id does not exist")
            );
    }
}
