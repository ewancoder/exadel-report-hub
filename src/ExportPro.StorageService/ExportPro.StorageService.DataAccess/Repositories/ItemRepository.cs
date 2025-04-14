using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Repository;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using MongoDB.Driver;

namespace ExportPro.StorageService.DataAccess.Repositories;

public class ItemRepository(ICollectionProvider collectionProvider) : MongoRepositoryBase<Item>(collectionProvider), IItemRepository
{
    
    public Task<List<Item>> GetFilteredItemsAsync(string? invoiceId = null, string? clientId = null, string? customerId = null, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Item>.Filter.Empty;

        if (!string.IsNullOrEmpty(invoiceId))
            filter &= Builders<Item>.Filter.Eq(x => x.InvoiceId, invoiceId);

        if (!string.IsNullOrEmpty(clientId))
            filter &= Builders<Item>.Filter.Eq(x => x.ClientId, clientId);

        if (!string.IsNullOrEmpty(customerId))
            filter &= Builders<Item>.Filter.Eq(x => x.CustomerId, customerId);

        return Collection.Find(filter).ToListAsync(cancellationToken: cancellationToken);
    }

    public Task<List<Item>> GetItemsAsync(CancellationToken cancellationToken = default)
    {
        var filter = Builders<Item>.Filter.Eq(x => x.IsDeleted, false);
        return Collection.Find(filter).ToListAsync(cancellationToken: cancellationToken);
    }
}

