
using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Repository;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using MongoDB.Driver;

namespace ExportPro.StorageService.DataAccess.Repositories;

public class CurrencyRepository(ICollectionProvider collectionProvider) : MongoRepositoryBase<Currency>(collectionProvider), ICurrencyRepository
{
    public async Task<Currency> GetByCodeAsync(CancellationToken cancellationToken)
    {
        var filter = Builders<Currency>.Filter.Eq(c => c.IsDeleted, false);
        return await Collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Currency>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await Collection.Find(x => !x.IsDeleted).ToListAsync(cancellationToken);
    }
}