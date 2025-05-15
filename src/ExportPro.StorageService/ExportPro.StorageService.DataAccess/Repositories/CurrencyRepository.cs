using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Repository;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.Models.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ExportPro.StorageService.DataAccess.Repositories;

public sealed class CurrencyRepository(ICollectionProvider collectionProvider)
    : BaseRepository<Currency>(collectionProvider),
        ICurrencyRepository
{
    public async Task<List<Currency>> GetPaginated(
        int top,
        int skip,
        OrderBy orderBy,
        CancellationToken cancellationToken = default
    )
    {
        var filter = Builders<Currency>.Filter.Eq(x => x.IsDeleted, false);
        var sort = orderBy switch
        {
            OrderBy.Ascending => Builders<Currency>.Sort.Ascending(x => x.CurrencyCode),
            OrderBy.Descending => Builders<Currency>.Sort.Descending(x => x.CurrencyCode),
            _ => throw new ArgumentOutOfRangeException(nameof(orderBy)),
        };
        return await Collection.Find(filter).Sort(sort).Skip(skip).Limit(top).ToListAsync(cancellationToken);
    }

    public Task<Currency?> GetCurrencyCodeById(ObjectId id)
    {
        return GetOneAsync(x => x.Id == id && !x.IsDeleted, CancellationToken.None);
    }

    public async Task<Currency> GetByCodeAsync(CancellationToken cancellationToken)
    {
        var filter = Builders<Currency>.Filter.Eq(c => c.IsDeleted, false);
        return await Collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }
}
