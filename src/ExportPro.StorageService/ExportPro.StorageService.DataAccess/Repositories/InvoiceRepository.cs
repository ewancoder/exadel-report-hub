using MongoDB.Driver;
using MongoDB.Bson;
using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Repository;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.Models.Models;

namespace ExportPro.StorageService.DataAccess.Repositories;

public class InvoiceRepository(ICollectionProvider collectionProvider) : MongoRepositoryBase<Invoice>(collectionProvider), IInvoiceRepository
{
    public async Task<List<Invoice>> GetAllByClientIdAsync(string clientId, CancellationToken cancellationToken)
    {
        var filter = Builders<Invoice>.Filter.Eq(x => x.ClientId, clientId);
        return await Collection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<List<Invoice>> GetByStatusAsync(Status status, CancellationToken cancellationToken)
    {
        var filter = Builders<Invoice>.Filter.Eq(x => x.PaymentStatus, status);
        return await Collection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<List<Invoice>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await Collection.Find(_ => true).ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(ObjectId id, CancellationToken cancellationToken)
    {
        var filter = Builders<Invoice>.Filter.Eq(x => x.Id, id);
        return await Collection.Find(filter).AnyAsync(cancellationToken);
    }
}