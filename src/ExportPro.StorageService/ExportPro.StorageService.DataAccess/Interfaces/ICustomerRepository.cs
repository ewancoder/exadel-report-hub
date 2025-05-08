using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.PaginationParams;

namespace ExportPro.StorageService.DataAccess.Interfaces;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<PaginatedList<Customer>> GetAllPaginatedAsync(
        PaginationParameters parameters,
        CancellationToken cancellationToken = default
    );
}
