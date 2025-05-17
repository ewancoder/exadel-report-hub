using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.SDK.DTOs.CustomerDTO;
using ExportPro.StorageService.SDK.PaginationParams;
using ExportPro.StorageService.SDK.Responses;
using Refit;

namespace ExportPro.StorageService.SDK.Refit;

public interface ICustomerController
{
    [Post("/api/Customer")]
    Task<BaseResponse<CustomerResponse>> Create(
        [Body] CreateUpdateCustomerDto customerDto,
        CancellationToken cancellationToken = default
    );

    [Put("/api/Customer/{id}")]
    Task<BaseResponse<CustomerResponse>> Update(
        Guid id,
        [Body] CreateUpdateCustomerDto customerDto,
        CancellationToken cancellationToken = default
    );

    [Post("/api/Customer/CreateBulk")]
    Task<BaseResponse<int>> CreateBulk(
        [Body] List<CreateUpdateCustomerDto> customers,
        CancellationToken cancellationToken = default
    );

    [Delete("/api/Customer/{id}")]
    Task<BaseResponse<bool>> Delete(Guid id, CancellationToken cancellationToken = default);

    [Get("/api/Customer/{id}")]
    Task<BaseResponse<CustomerDto>> GetById(Guid id, CancellationToken cancellationToken = default);

    [Get("/api/Customer")]
    Task<BaseResponse<PaginatedListDto<CustomerDto>>> GetAll(
        [Query] int pageNumber = 1,
        [Query] int pageSize = 10,
        CancellationToken cancellationToken = default
    );
}
