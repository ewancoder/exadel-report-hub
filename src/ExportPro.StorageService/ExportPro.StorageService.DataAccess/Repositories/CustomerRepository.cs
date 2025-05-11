using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Repository;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.PaginationParams;
using MongoDB.Driver;

namespace ExportPro.StorageService.DataAccess.Repositories;

public sealed class CustomerRepository(ICollectionProvider collectionProvider)
    : BaseRepository<Customer>(collectionProvider),
        ICustomerRepository
{
    public async Task<PaginatedList<Customer>> GetAllPaginatedAsync(
        PaginationParameters parameters,
        CancellationToken cancellationToken = default
    )
    {
        var filter = Builders<Customer>.Filter.Eq(c => c.IsDeleted, false);
        // Get total count for pagination
        var totalCount = await Collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        // Apply pagination
        var customers = await Collection
            .Find(filter)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Limit(parameters.PageSize)
            .SortBy(c => c.Name)
            .ToListAsync(cancellationToken);

        return new PaginatedList<Customer>(customers, (int)totalCount, parameters.PageNumber, parameters.PageSize);
    }
}
