using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;
using ExportPro.StorageService.SDK.PaginationParams;
using ExportPro.StorageService.SDK.Responses;
using Refit;

namespace ExportPro.StorageService.SDK.Refit;

public interface IInvoiceController
{
    [Post("/api/Invoice")]
    Task<BaseResponse<InvoiceResponse>> Create(
        [Body] CreateInvoiceDto invoice,
        CancellationToken cancellationToken = default
    );

    [Put("/api/Invoice/{id}")]
    Task<BaseResponse<InvoiceResponse>> Update(
        Guid id,
        [Body] CreateInvoiceDto invoice,
        CancellationToken cancellationToken = default
    );

    [Delete("/api/Invoice/{id}")]
    Task<BaseResponse<bool>> Delete(Guid id, CancellationToken cancellationToken = default);

    [Get("/api/Invoice/{id}")]
    Task<BaseResponse<InvoiceDto>> GetById(Guid id, CancellationToken cancellationToken = default);

    [Get("/api/Invoice")]
    Task<BaseResponse<PaginatedListDto<InvoiceDto>>> GetInvoices(
        CancellationToken cancellationToken = default,
        [Query] int pageNumber = 1,
        [Query] int pageSize = 10
    );
}
