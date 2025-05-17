using ExportPro.Common.Shared.Extensions;
using ExportPro.StorageService.CQRS.CommandHandlers.InvoiceCommands;
using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;

namespace ExportPro.StorageService.Api.Validations.Invoice;

public sealed class CreateInvoiceValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceValidator(
        IClientRepository clientRepository,
        ICustomerRepository customerRepository,
        ICurrencyRepository currencyRepository
    )
    {
        RuleFor(x => x.CreateInvoiceDto.Items).NotEmpty().WithMessage("Items cannot be empty.");
        RuleFor(x => x.CreateInvoiceDto.InvoiceNumber).NotEmpty().WithMessage("Invoice number is required.");
        RuleFor(x => x.CreateInvoiceDto.IssueDate).NotEmpty().WithMessage("The issue date is required");
        RuleFor(x => x)
            .Must(x =>
            {
                if (x.CreateInvoiceDto.DueDate >= x.CreateInvoiceDto.IssueDate)
                    return true;
                return false;
            })
            .WithMessage("Issue date cannot be earlier than due date.");
        RuleFor(x => x.CreateInvoiceDto.CustomerId)
            .NotEmpty()
            .WithMessage("Customer  Id  cannot be empty.")
            .DependentRules(
                () =>
                    RuleFor(x => x.CreateInvoiceDto.CustomerId)
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
        RuleFor(x => x.CreateInvoiceDto.ClientId)
            .NotEmpty()
            .WithMessage("Client Id  cannot be empty.")
            .DependentRules(() =>
            {
                RuleFor(x => x.CreateInvoiceDto.ClientId)
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
    }
}
