using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Repository;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using MongoDB.Driver;

namespace ExportPro.StorageService.DataAccess.Repositories;

public class CustomerRepository(ICollectionProvider collectionProvider) : MongoRepositoryBase<Customer>(collectionProvider), ICustomerRepository
{
    public async Task<List<Customer>> GetAllAsync(CancellationToken cancellationToken)
    {
        var filter = Builders<Customer>.Filter.Eq(c => c.IsDeleted, false);
        return await Collection.Find(filter).ToListAsync(cancellationToken);
    }
}