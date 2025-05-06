using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.CommandHandlers.InvoiceCommands;

public sealed record UpdateInvoiceCommand(Guid Id, CreateInvoiceDto InvoiceDto) : ICommand<InvoiceResponse>;

public sealed class UpdateInvoiceHandler(IInvoiceRepository repository, IMapper mapper)
    : ICommandHandler<UpdateInvoiceCommand, InvoiceResponse>
{
    public async Task<BaseResponse<InvoiceResponse>> Handle(
        UpdateInvoiceCommand request,
        CancellationToken cancellationToken
    )
    {
        var existing = await repository.GetOneAsync(
            x => x.Id == request.Id.ToObjectId() && !x.IsDeleted,
            cancellationToken
        );
        if (existing == null)
            return new NotFoundResponse<InvoiceResponse>("Invoice not found.");
        existing.InvoiceNumber = request.InvoiceDto.InvoiceNumber;
        existing.IssueDate = request.InvoiceDto.IssueDate;
        existing.DueDate = request.InvoiceDto.DueDate;
        existing.CurrencyId = request.InvoiceDto.CurrencyId.ToObjectId();
        existing.PaymentStatus = request.InvoiceDto.PaymentStatus;
        existing.BankAccountNumber = request.InvoiceDto.BankAccountNumber;
        existing.ClientId = request.InvoiceDto.ClientId.ToObjectId();
        await repository.UpdateOneAsync(existing, cancellationToken);
        var invoiceResponse = mapper.Map<InvoiceResponse>(request.InvoiceDto);
        invoiceResponse.Id = existing.Id.ToGuid();
        return new SuccessResponse<InvoiceResponse>(invoiceResponse, "Invoice updated successfully.");
    }
}
