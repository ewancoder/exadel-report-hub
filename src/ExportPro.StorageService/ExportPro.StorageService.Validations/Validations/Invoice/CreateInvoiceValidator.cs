using ExportPro.StorageService.CQRS.CommandHandlers.InvoiceCommands;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations.Invoice;

public sealed class CreateInvoiceValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceValidator(
        IClientRepository clientRepository,
        ICustomerRepository customerRepository,
        ICurrencyRepository currencyRepository
    )
    {
        RuleFor(x => x.Items).NotEmpty().WithMessage("Items cannot be empty.");
        RuleFor(x => x.InvoiceNumber).NotEmpty().WithMessage("Invoice number is required.");
        RuleFor(x => x.IssueDate).NotEmpty().WithMessage("The issue date is required");
        RuleFor(x => x)
            .Must(x =>
            {
                if (x.DueDate >= x.IssueDate)
                    return true;
                return false;
            })
            .WithMessage("Issue date cannot be earlier than due date.");
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer  Id  cannot be empty.")
            .DependentRules(
                () =>
                    RuleFor(x => x.CustomerId)
                        .MustAsync(
                            async (customer, cancellationToken) =>
                            {
                                var client = await customerRepository.GetOneAsync(
                                    x => x.Id == customer.ToObjectId() && !x.IsDeleted,
                                    cancellationToken
                                );
                                return client != null;
                            }
                        )
                        .WithMessage("The Customer Id does not exist")
            );
        RuleFor(x => x.ClientId)
            .NotEmpty()
            .WithMessage("Client Id  cannot be empty.")
            .DependentRules(() =>
            {
                RuleFor(x => x.ClientId)
                    .MustAsync(
                        async (id, cancellationToken) =>
                        {
                            var client = await clientRepository.GetOneAsync(
                                x => x.Id == id.ToObjectId() && !x.IsDeleted,
                                cancellationToken
                            );
                            return client != null;
                        }
                    )
                    .WithMessage("The Client Id does not exist");
            });
        RuleFor(x => x.CurrencyId)
            .NotEmpty()
            .WithMessage("Currency Id  cannot be empty.")
            .DependentRules(
                () =>
                    RuleFor(x => x.CurrencyId)
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
