using AutoMapper;
using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Repository;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.PaginationParams;
using ExportPro.StorageService.SDK.Responses;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ExportPro.StorageService.DataAccess.Repositories;

public sealed class CurrencyRepository(ICollectionProvider collectionProvider, IMapper mapper)
    : BaseRepository<Currency>(collectionProvider),
        ICurrencyRepository
{
    public async Task<PaginatedList<CurrencyResponse>> GetPaginated(
        PaginationParameters paginationParameters,
        CancellationToken cancellationToken = default
    )
    {
        var filter = Builders<Currency>.Filter.Eq(x => x.IsDeleted, false);
        var currencies = await Collection.Find(filter).ToListAsync(cancellationToken);
        var currency = currencies.Select(mapper.Map<CurrencyResponse>).ToList();
        return currency.ToPaginatedList(
            pageNumber: paginationParameters.PageNumber,
            pageSize: paginationParameters.PageSize
        );
    }

    public Task<Currency?> GetCurrencyCodeById(ObjectId id)
    {
        return GetOneAsync(x => x.Id == id && !x.IsDeleted, CancellationToken.None);
    }

    public async Task<Currency> GetByCodeAsync(CancellationToken cancellationToken)
    {
        var filter = Builders<Currency>.Filter.Eq(c => c.IsDeleted, false);
        return await Collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }
}
