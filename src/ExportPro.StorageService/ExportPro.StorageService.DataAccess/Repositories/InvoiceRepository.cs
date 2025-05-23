﻿using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Repository;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.PaginationParams;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ExportPro.StorageService.DataAccess.Repositories;

public sealed class InvoiceRepository(ICollectionProvider collectionProvider)
    : BaseRepository<Invoice>(collectionProvider),
        IInvoiceRepository
{
    public async Task<List<Invoice>> GetAllByClientIdAsync(ObjectId clientId, CancellationToken cancellationToken)
    {
        var filter = Builders<Invoice>.Filter.Eq(x => x.ClientId, clientId);
        return await Collection.Find(filter).ToListAsync(cancellationToken);
    }

    public Task<List<Invoice>> GetOverdueInvoices(ObjectId ClientId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Invoice>.Filter.And(
            Builders<Invoice>.Filter.Eq(x => x.PaymentStatus, Status.Unpaid),
            Builders<Invoice>.Filter.Lt(x => x.DueDate, DateTime.UtcNow),
            Builders<Invoice>.Filter.Eq("IsDeleted", false),
            Builders<Invoice>.Filter.Eq(x => x.ClientId, ClientId)
        );

        return Collection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<List<Invoice>> GetByStatusAsync(Status status, CancellationToken cancellationToken)
    {
        var filter = Builders<Invoice>.Filter.Eq(x => x.PaymentStatus, status);
        return await Collection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<PaginatedList<Invoice>> GetAllPaginatedAsync(
        PaginationParameters parameters,
        CancellationToken cancellationToken = default
    )
    {
        var filter = Builders<Invoice>.Filter.Empty;

        if (typeof(Invoice).GetProperty("IsDeleted") != null)
            filter = Builders<Invoice>.Filter.Eq("IsDeleted", false);

        var totalCount = await Collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var invoices = await Collection
            .Find(filter)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Limit(parameters.PageSize)
            .SortByDescending(i => i.IssueDate)
            .ToListAsync(cancellationToken);

        return new PaginatedList<Invoice>(invoices, (int)totalCount, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<bool> ExistsAsync(ObjectId id, CancellationToken cancellationToken)
    {
        var filter = Builders<Invoice>.Filter.Eq(x => x.Id, id);
        return await Collection.Find(filter).AnyAsync(cancellationToken);
    }

    public async Task<List<Invoice>> GetInvoicesInDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await Collection.Find(x => x.IssueDate >= startDate && x.IssueDate <= endDate).ToListAsync();
    }

    public async Task<long> CountAsync(FilterDefinition<Invoice> filter, CancellationToken cancellationToken)
    {
        return await Collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
    }

    public Task<List<Invoice>> GetAllAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
