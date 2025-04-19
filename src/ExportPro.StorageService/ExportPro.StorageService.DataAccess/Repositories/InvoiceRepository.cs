using MongoDB.Driver;
using MongoDB.Bson;
using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Repository;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.PaginationParams;

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

    public async Task<PaginatedList<Invoice>> GetAllPaginatedAsync(
        PaginationParameters parameters,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<Invoice>.Filter.Empty;

        if (!includeDeleted && typeof(Invoice).GetProperty("IsDeleted") != null)
        {
            filter = Builders<Invoice>.Filter.Eq("IsDeleted", false);
        }

        var totalCount = await Collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var invoices = await Collection.Find(filter)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Limit(parameters.PageSize)
            .SortByDescending(i => i.IssueDate) 
            .ToListAsync(cancellationToken);

        return new PaginatedList<Invoice>(
            invoices,
            (int)totalCount,
            parameters.PageNumber,
            parameters.PageSize);
    }

    public async Task<bool> ExistsAsync(ObjectId id, CancellationToken cancellationToken)
    {
        var filter = Builders<Invoice>.Filter.Eq(x => x.Id, id);
        return await Collection.Find(filter).AnyAsync(cancellationToken);
    }

    public Task<List<Invoice>> GetAllAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}