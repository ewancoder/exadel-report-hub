using ExportPro.StorageService.CQRS.CommandHandlers.CustomerCommands;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations.Customer;

public sealed class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator(ICountryRepository countryRepository)
    {
        RuleFor(x => x.CustomerDto.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters.");
        RuleFor(x => x.CustomerDto.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.");
        RuleFor(x => x.CustomerDto.CountryId)
            .NotEmpty()
            .WithMessage("The country id is required")
            .DependentRules(() =>
            {
                RuleFor(x => x.CustomerDto.CountryId)
                    .MustAsync(
                        async (countryId, cancellation) =>
                        {
                            var country = await countryRepository.GetOneAsync(
                                x => x.Id == countryId.ToObjectId() && !x.IsDeleted,
                                cancellation
                            );
                            if (country == null)
                                return false;
                            return true;
                        }
                    )
                    .WithMessage("CountryId does not exist.");
            });
    }
}
