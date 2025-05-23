﻿using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.PaginationParams;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ExportPro.StorageService.DataAccess.Interfaces;

public interface IInvoiceRepository : IRepository<Invoice>
{
    Task<List<Invoice>> GetAllByClientIdAsync(ObjectId clientId, CancellationToken cancellationToken);
    Task<List<Invoice>> GetByStatusAsync(Status status, CancellationToken cancellationToken);

    Task<PaginatedList<Invoice>> GetAllPaginatedAsync(
        PaginationParameters parameters,
        CancellationToken cancellationToken = default
    );

    Task<bool> ExistsAsync(ObjectId id, CancellationToken cancellationToken);
    Task<long> CountAsync(FilterDefinition<Invoice> filter, CancellationToken cancellationToken);
    Task<List<Invoice>> GetOverdueInvoices(ObjectId ClientId, CancellationToken cancellationToken = default);
    Task<List<Invoice>> GetInvoicesInDateRangeAsync(DateTime startDate, DateTime endDate);
}
