using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Repository;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using MongoDB.Driver;

namespace ExportPro.StorageService.DataAccess.Repositories;

public class CountryRepository(ICollectionProvider collectionProvider) : MongoRepositoryBase<Country>(collectionProvider), ICountryRepository
{
    public async Task<List<Country>> GetAllAsync(CancellationToken cancellationToken)
    {
        var filter = Builders<Country>.Filter.Eq(c => c.IsDeleted, false);
        return await Collection.Find(filter).ToListAsync(cancellationToken);
    }
}