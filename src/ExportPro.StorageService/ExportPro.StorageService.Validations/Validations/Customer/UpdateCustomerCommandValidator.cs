using ExportPro.Common.Shared.Extensions;
using ExportPro.StorageService.CQRS.CommandHandlers.CustomerCommands;
using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations.Customer;

public sealed class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator(ICountryRepository countryRepository, ICustomerRepository repository)
    {
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
