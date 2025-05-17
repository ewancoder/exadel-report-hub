using ExportPro.Common.Shared.Extensions;
using ExportPro.StorageService.CQRS.CommandHandlers.CustomerCommands;
using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;

namespace ExportPro.StorageService.Api.Validations.Customer;

public sealed class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator(ICountryRepository countryRepository, ICustomerRepository customerRepository)
    {
        RuleFor(x => x.CustomerDto.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters.")
            .MustAsync(
                async (name, cancellation) =>
                {
                    var country = await countryRepository.GetOneAsync(
                        x => x.Name == name && !x.IsDeleted,
                        cancellation
                    );
                    if (country == null)
                        return true;
                    return false;
                }
            )
            .WithMessage("Name already exists.");
        RuleFor(x => x.CustomerDto.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .MustAsync(
                async (email, cancellation) =>
                {
                    var customer = await customerRepository.GetOneAsync(
                        x => x.Email == email && !x.IsDeleted,
                        cancellation
                    );
                    if (customer == null)
                        return true;
                    return false;
                }
            )
            .WithMessage("Email already exists.");
        RuleFor(x => x.CustomerDto.Address)
            .NotEmpty()
            .WithMessage("Address is required.")
            .MaximumLength(200)
            .WithMessage("Address must not exceed 200 characters.")
            .MustAsync(
                async (address, cancellation) =>
                {
                    var customer = await customerRepository.GetOneAsync(
                        x => x.Address == address && !x.IsDeleted,
                        cancellation
                    );
                    if (customer == null)
                        return true;
                    return false;
                }
            )
            .WithMessage("Address already exists.");

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
