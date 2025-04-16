using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Repository;
using ExportPro.StorageService.Models.Models;
using MongoDB.Driver;

namespace ExportPro.StorageService.DataAccess.Repositories;

public interface ICountryRepository : IRepository<Country>
{
    Task<List<Country>> GetAllAsync(CancellationToken cancellationToken);
}

public class CountryRepository : MongoRepositoryBase<Country>, ICountryRepository
{
    public CountryRepository(ICollectionProvider collectionProvider) : base(collectionProvider) { }

    public async Task<List<Country>> GetAllAsync(CancellationToken cancellationToken)
    {
        var filter = Builders<Country>.Filter.Eq(c => c.IsDeleted, false);
        return await Collection.Find(filter).ToListAsync(cancellationToken);
    }
}