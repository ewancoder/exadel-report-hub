using System.Security.Claims;
using AutoMapper;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Helpers;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;
using ExportPro.StorageService.SDK.Responses;
using Microsoft.AspNetCore.Http;

namespace ExportPro.StorageService.CQRS.CommandHandlers.InvoiceCommands;

public sealed record UpdateInvoiceCommand(Guid Id, CreateInvoiceDto InvoiceDto) : ICommand<InvoiceResponse>, IPermissionedRequest
{
    public List<Guid>? ClientIds { get; init; } = [InvoiceDto.ClientId];
    public Resource Resource { get; init; } = Resource.Invoices;
    public CrudAction Action { get; init; } = CrudAction.Update;
};

public sealed class UpdateInvoiceHandler(
    IHttpContextAccessor httpContext,
    IInvoiceRepository repository,
    IMapper mapper
) : ICommandHandler<UpdateInvoiceCommand, InvoiceResponse>
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

        existing.PaymentStatus = request.InvoiceDto.PaymentStatus;
        existing.BankAccountNumber = request.InvoiceDto.BankAccountNumber;
        existing.ClientId = request.InvoiceDto.ClientId.ToObjectId();
        existing.UpdatedAt = DateTime.Now;
        existing.UpdatedBy = httpContext.HttpContext?.User.FindFirst(ClaimTypes.Name)!.Value;
        await repository.UpdateOneAsync(existing, cancellationToken);
        var invoiceResponse = mapper.Map<InvoiceResponse>(request.InvoiceDto);
        invoiceResponse.Id = existing.Id.ToGuid();
        return new SuccessResponse<InvoiceResponse>(invoiceResponse, "Invoice updated successfully.");
    }
}
