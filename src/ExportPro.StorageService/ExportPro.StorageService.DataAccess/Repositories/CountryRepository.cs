using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Repository;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.PaginationParams;
using MongoDB.Driver;

namespace ExportPro.StorageService.DataAccess.Repositories;

public class CountryRepository(ICollectionProvider collectionProvider) : BaseRepository<Country>(collectionProvider), ICountryRepository
{
    public async Task<PaginatedList<Country>> GetAllPaginatedAsync(PaginationParameters parameters, bool includeDeleted, CancellationToken cancellationToken)
    {
        var filter = includeDeleted ? Builders<Country>.Filter.Empty : Builders<Country>.Filter.Eq(x => x.IsDeleted, false);
        var total = (int)await Collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var items = await Collection.Find(filter)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Limit(parameters.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<Country>(items, total, parameters.PageNumber, parameters.PageSize);
    }
}