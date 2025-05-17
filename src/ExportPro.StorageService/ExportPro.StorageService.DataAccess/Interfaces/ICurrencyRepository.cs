using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.PaginationParams;
using ExportPro.StorageService.SDK.Responses;
using MongoDB.Bson;

namespace ExportPro.StorageService.DataAccess.Interfaces;

public interface ICurrencyRepository : IRepository<Currency>
{
    Task<PaginatedList<CurrencyResponse>> GetPaginated(
        PaginationParameters paginationParameters,
        CancellationToken cancellationToken = default
    );
    Task<Currency?> GetCurrencyCodeById(ObjectId id);
}
