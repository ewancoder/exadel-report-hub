using ExportPro.StorageService.CQRS.CommandHandlers.CountryCommands;
using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;
using MongoDB.Bson;

namespace ExportPro.StorageService.Validations.Validations.Country;

public class CreateCountryCommandValidator:AbstractValidator<CreateCountryCommand>
{
    public  CreateCountryCommandValidator(ICurrencyRepository currencyRepository)
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("The Country name is required");
        RuleFor(x => x.CurrencyId)
            .NotEmpty()
            .WithMessage("The Currency Id is required")
            .Must(id => { return ObjectId.TryParse(id, out _); }).WithMessage("The Currency Id is not valid in format.")
            .DependentRules(() =>
                RuleFor(x => x.CurrencyId)
                    .MustAsync(async (currency, _) =>
                    {
                        var client = await currencyRepository.GetByIdAsync(ObjectId.Parse(currency), _);
                        return client != null;
                    }
                    ).WithMessage("The Currency Id does not exist"));


    }
}
