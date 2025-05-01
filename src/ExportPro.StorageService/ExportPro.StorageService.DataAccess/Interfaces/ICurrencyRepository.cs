using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.StorageService.Models.Models;
using MongoDB.Bson;

namespace ExportPro.StorageService.DataAccess.Interfaces;

public interface ICurrencyRepository : IRepository<Currency>
{
    Task<List<Currency>> GetAllAsync(CancellationToken cancellationToken);
    Task<Currency?> GetCurrencyCodeById(ObjectId id);
}
