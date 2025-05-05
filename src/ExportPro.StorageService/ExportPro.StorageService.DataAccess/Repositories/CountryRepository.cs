using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Repository;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.PaginationParams;
using MongoDB.Driver;

namespace ExportPro.StorageService.DataAccess.Repositories;

public sealed class CountryRepository(ICollectionProvider collectionProvider)
    : BaseRepository<Country>(collectionProvider),
        ICountryRepository
{
    public async Task<PaginatedList<Country>> GetAllPaginatedAsync(
        PaginationParameters parameters,
        CancellationToken cancellationToken
    )
    {
        var filter = Builders<Country>.Filter.Eq(x => x.IsDeleted, false);
        var total = (int)await Collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var items = await Collection
            .Find(filter)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Limit(parameters.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<Country>(items, total, parameters.PageNumber, parameters.PageSize);
    }
}
