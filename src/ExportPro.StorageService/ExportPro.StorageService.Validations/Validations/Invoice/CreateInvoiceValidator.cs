using ExportPro.StorageService.CQRS.CommandHandlers.InvoiceCommands;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Validations.Validations.Client;
using FluentValidation;
using MongoDB.Bson;

namespace ExportPro.StorageService.Validations.Validations.Invoice;

public sealed class CreateInvoiceValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceValidator(
        IClientRepository clientRepository,
        ICustomerRepository customerRepository,
        ICurrencyRepository currencyRepository
    )
    {
        RuleFor(x => x.InvoiceNumber).NotEmpty().WithMessage("Invoice number is required.");
        RuleFor(x => x.IssueDate).NotEmpty().WithMessage("The issue date is required");
        RuleFor(x => x)
            .Must(x =>
            {
                if (x.DueDate >= x.IssueDate)
                {
                    return true;
                }
                return false;
            })
            .WithMessage("Issue date cannot be earlier than due date.");
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer  Id  cannot be empty.")
            .Must(id =>
            {
                return ObjectId.TryParse(id, out _);
            })
            .WithMessage("The Customer Id is not valid in format.")
            .DependentRules(
                () =>
                    RuleFor(x => x.CustomerId)
                        .MustAsync(
                            async (customer, _) =>
                            {
                                var client = await customerRepository.GetByIdAsync(ObjectId.Parse(customer), _);
                                return client != null;
                            }
                        )
                        .WithMessage("The Customer Id does not exist")
            );
        RuleFor(x => x.ClientId)
            .NotEmpty()
            .WithMessage("Client Id  cannot be empty.")
            .Must(id =>
            {
                return ObjectId.TryParse(id, out _);
            })
            .WithMessage("The Client Id is not valid in format.")
            .DependentRules(() =>
            {
                RuleFor(x => x.ClientId)
                    .MustAsync(
                        async (id, _) =>
                        {
                            var client = await clientRepository.GetClientById(id);
                            return client != null;
                        }
                    )
                    .WithMessage("The Client Id does not exist");
            });
        RuleFor(x => x.CurrencyId)
            .NotEmpty()
            .WithMessage("Currency Id  cannot be empty.")
            .Must(id =>
            {
                return ObjectId.TryParse(id, out _);
            })
            .WithMessage("The Currency Id is not valid in format.")
            .DependentRules(
                () =>
                    RuleFor(x => x.CurrencyId)
                        .MustAsync(
                            async (currency, _) =>
                            {
                                var client = await currencyRepository.GetByIdAsync(ObjectId.Parse(currency), _);
                                return client != null;
                            }
                        )
                        .WithMessage("The Currency Id does not exist")
            );
    }
}
