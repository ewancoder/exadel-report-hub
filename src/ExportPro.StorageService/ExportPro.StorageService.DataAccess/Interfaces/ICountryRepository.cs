using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.PaginationParams;

namespace ExportPro.StorageService.DataAccess.Interfaces;

public interface ICountryRepository : IRepository<Country>
{
    Task<PaginatedList<Country>> GetAllPaginatedAsync(PaginationParameters parameters, bool includeDeleted, CancellationToken cancellationToken);
}