using ExportPro.StorageService.CQRS.CommandHandlers.CountryCommands;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations.Country;

public sealed class CreateCountryCommandValidator : AbstractValidator<CreateCountryCommand>
{
    public CreateCountryCommandValidator(ICurrencyRepository currencyRepository)
    {
        RuleFor(x => x.CountryDto.Name).NotEmpty().WithMessage("The Country name is required");
        RuleFor(x => x.CountryDto.CurrencyId)
            .NotEmpty()
            .WithMessage("The Currency Id is required")
            .DependentRules(
                () =>
                    RuleFor(x => x.CountryDto.CurrencyId)
                        .MustAsync(
                            async (currency, cancellationToken) =>
                            {
                                var client = await currencyRepository.GetOneAsync(
                                    x => x.Id == currency.ToObjectId() && !x.IsDeleted,
                                    cancellationToken
                                );
                                return client != null;
                            }
                        )
                        .WithMessage("The Currency Id does not exist")
            );
    }
}
