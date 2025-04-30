using ExportPro.StorageService.CQRS.CommandHandlers.CustomerCommands;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;
using MongoDB.Bson;

namespace ExportPro.StorageService.Validations.Validations.Customer;

public sealed class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator(ICountryRepository countryRepository)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters.");
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.");
        RuleFor(x => x.CountryId)
            .NotEmpty()
            .WithMessage("The country id is required")
            .DependentRules(() =>
            {
                RuleFor(x => x.CountryId)
                    .MustAsync(
                        async (countryId, cancellation) =>
                        {
                            var country = await countryRepository.GetOneAsync(
                                x => x.Id == countryId.ToObjectId(),
                                cancellation
                            );
                            if (country == null)
                            {
                                return false;
                            }
                            return true;
                        }
                    )
                    .WithMessage("CountryId does not exist.");
            });
    }
}
