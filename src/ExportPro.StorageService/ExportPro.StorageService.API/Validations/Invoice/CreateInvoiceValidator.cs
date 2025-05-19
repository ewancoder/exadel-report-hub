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
        RuleFor(x => x.CreateInvoiceDto.InvoiceNumber).NotEmpty().WithMessage("Invoice number is required.");

        RuleFor(x => x.CreateInvoiceDto.IssueDate).NotEmpty().WithMessage("The issue date is required");

        RuleFor(x => x)
            .Must(x => x.CreateInvoiceDto.DueDate >= x.CreateInvoiceDto.IssueDate)
            .WithMessage("Due date cannot be earlier than issue date.");

        RuleFor(x => x.CreateInvoiceDto.CustomerId)
            .NotEmpty()
            .WithMessage("Customer Id cannot be empty.")
            .DependentRules(() =>
            {
                RuleFor(x => x.CreateInvoiceDto.CustomerId)
                    .MustAsync(
                        async (customerId, cancellationToken) =>
                        {
                            var customer = await customerRepository.GetOneAsync(
                                x => x.Id == customerId.ToObjectId() && !x.IsDeleted,
                                cancellationToken
                            );
                            return customer != null;
                        }
                    )
                    .WithMessage("The Customer Id does not exist");
            });

        RuleFor(x => x.CreateInvoiceDto.ClientId)
            .NotEmpty()
            .WithMessage("Client Id cannot be empty.")
            .DependentRules(() =>
            {
                RuleFor(x => x.CreateInvoiceDto.ClientId)
                    .MustAsync(
                        async (clientId, cancellationToken) =>
                        {
                            var client = await clientRepository.GetOneAsync(
                                x => x.Id == clientId.ToObjectId() && !x.IsDeleted,
                                cancellationToken
                            );
                            return client != null;
                        }
                    )
                    .WithMessage("The Client Id does not exist");

                RuleForEach(x => x.CreateInvoiceDto.Items)
                    .NotEmpty()
                    .WithMessage("Item Id cannot be empty.")
                    .MustAsync(
                        async (command, itemId, cancellationToken) =>
                        {
                            var client = await clientRepository.GetOneAsync(
                                x => x.Id == command.CreateInvoiceDto.ClientId.ToObjectId() && !x.IsDeleted,
                                cancellationToken
                            );
                            return client?.Items.Any(x => x.Id == itemId.ToObjectId()) ?? false;
                        }
                    )
                    .WithMessage("The Item Id does not exist .");
            });
    }
}
