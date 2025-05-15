using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.Models.Models;
using MongoDB.Bson;

namespace ExportPro.StorageService.DataAccess.Interfaces;

public interface ICurrencyRepository : IRepository<Currency>
{
    Task<List<Currency>> GetPaginated(Filters filters, CancellationToken cancellationToken = default);
    Task<Currency?> GetCurrencyCodeById(ObjectId id);
}
