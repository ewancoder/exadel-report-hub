using ExportPro.Common.Shared.Extensions;
using ExportPro.StorageService.CQRS.CommandHandlers.InvoiceCommands;
using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;

namespace ExportPro.StorageService.Api.Validations.Invoice;

public class UpdateInvoiceValidator : AbstractValidator<UpdateInvoiceCommand>
{
    public UpdateInvoiceValidator(
        IClientRepository clientRepository,
        ICustomerRepository customerRepository,
        ICurrencyRepository currencyRepository
    )
    {
        RuleFor(x => x.InvoiceDto.Items).NotEmpty().WithMessage("Items cannot be empty.");
        RuleFor(x => x.InvoiceDto.InvoiceNumber).NotEmpty().WithMessage("Invoice number is required.");
        RuleFor(x => x.InvoiceDto.IssueDate).NotEmpty().WithMessage("The issue date is required");
        RuleFor(x => x)
            .Must(x =>
            {
                if (x.InvoiceDto.DueDate >= x.InvoiceDto.IssueDate)
                    return true;
                return false;
            })
            .WithMessage("Issue date cannot be earlier than due date.");
        RuleFor(x => x.InvoiceDto.CustomerId)
            .NotEmpty()
            .WithMessage("Customer  Id  cannot be empty.")
            .DependentRules(
                () =>
                    RuleFor(x => x.InvoiceDto.CustomerId)
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
        RuleFor(x => x.InvoiceDto.ClientId)
            .NotEmpty()
            .WithMessage("Client Id  cannot be empty.")
            .DependentRules(() =>
            {
                RuleFor(x => x.InvoiceDto.ClientId)
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
        RuleFor(x => x.InvoiceDto.CurrencyId)
            .NotEmpty()
            .WithMessage("Currency Id  cannot be empty.")
            .DependentRules(
                () =>
                    RuleFor(x => x.InvoiceDto.CurrencyId)
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
