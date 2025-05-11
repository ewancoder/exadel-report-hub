using ExportPro.Common.Shared.Extensions;
using ExportPro.StorageService.CQRS.CommandHandlers.CustomerCommands;
using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations.Customer;

public sealed class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator(ICountryRepository countryRepository, ICustomerRepository repository)
    {
        RuleFor(x => x.Customer.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters.")
            .MustAsync(
                async (name, cancellation) =>
                {
                    var customer = await repository.GetOneAsync(x => x.Name == name && !x.IsDeleted, cancellation);
                    if (customer == null)
                        return true;
                    return false;
                }
            )
            .WithMessage("Name already exists.");
        RuleFor(x => x.Customer.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .MustAsync(
                async (email, cancellation) =>
                {
                    var customer = await repository.GetOneAsync(x => x.Email == email && !x.IsDeleted, cancellation);
                    if (customer == null)
                        return true;
                    return false;
                }
            )
            .WithMessage("Email already exists.");
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("The id is required")
            .MustAsync(
                async (id, cancellationToken) =>
                {
                    var client = await repository.GetOneAsync(
                        x => x.Id == id.ToObjectId() && !x.IsDeleted,
                        cancellationToken
                    );
                    return client != null;
                }
            )
            .WithMessage("The customer id does not exist");
        RuleFor(x => x.Customer.CountryId)
            .MustAsync(
                async (id, token) =>
                {
                    var country = await countryRepository.GetOneAsync(
                        x => x.Id == id.ToObjectId() && !x.IsDeleted,
                        token
                    );
                    return country != null;
                }
            )
            .WithMessage("The country id does not exist");
    }
}
