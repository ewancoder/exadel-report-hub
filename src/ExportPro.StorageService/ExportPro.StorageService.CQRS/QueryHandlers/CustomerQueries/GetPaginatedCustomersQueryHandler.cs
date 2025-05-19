using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Helpers;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs.CustomerDTO;
using ExportPro.StorageService.SDK.PaginationParams;

namespace ExportPro.StorageService.CQRS.QueryHandlers.CustomerQueries;

public sealed record GetPaginatedCustomersQuery(int PageNumber = 1, int PageSize = 10)
    : IQuery<PaginatedListDto<CustomerDto>>,
        IPermissionedRequest
{
    public List<Guid>? ClientIds => null;
    public Resource Resource => Resource.Customers;
    public CrudAction Action => CrudAction.Read;
}

public sealed class GetPaginatedCustomersQueryHandler(ICustomerRepository repository)
    : IQueryHandler<GetPaginatedCustomersQuery, PaginatedListDto<CustomerDto>>
{
    public async Task<BaseResponse<PaginatedListDto<CustomerDto>>> Handle(
        GetPaginatedCustomersQuery request,
        CancellationToken cancellationToken
    )
    {
        var parameters = new PaginationParameters { PageNumber = request.PageNumber, PageSize = request.PageSize };

        var paginatedCustomers = await repository.GetAllPaginatedAsync(parameters, cancellationToken);

        var customerDtos = paginatedCustomers
            .Items.Select(c => new CustomerDto
            {
                Id = c.Id.ToGuid(),
                Name = c.Name,
                Email = c.Email,
                CountryId = c.CountryId.ToGuid(),
                IsDeleted = c.IsDeleted,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
            })
            .ToList();

        var dto = new PaginatedListDto<CustomerDto>(
            customerDtos,
            paginatedCustomers.TotalCount,
            paginatedCustomers.PageNumber,
            paginatedCustomers.TotalPages
        );

        return new SuccessResponse<PaginatedListDto<CustomerDto>>(dto, "Successfully retrieved customers.");
    }
}
