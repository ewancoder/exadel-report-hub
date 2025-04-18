using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.SDK.DTOs.CustomerDTO;
using ExportPro.StorageService.SDK.PaginationParams;

namespace ExportPro.StorageService.CQRS.Queries.CustomerQueries;

public class GetPaginatedCustomersQuery : IQuery<PaginatedListDto<CustomerDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool IncludeDeleted { get; set; } = false;
}
