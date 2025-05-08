using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Repository;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ExportPro.StorageService.DataAccess.Repositories;

public sealed class ReportPreferenceRepository(ICollectionProvider collectionProvider)
    : BaseRepository<ReportPreference>(collectionProvider), IReportPreference
{
    public async Task<List<ReportPreference>> GetAllByClientIdAsync(ObjectId clientId, CancellationToken cancellationToken)
    {
        return await Collection.Find(x => x.ClientId == clientId && !x.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ReportPreference>> GetAllPreferences(CancellationToken cancellationToken)
    {
        return await Collection.Find(c => (c.IsDeleted == false || c.IsDeleted == null) && (c.IsEnabled == false || c.IsEnabled == null)).ToListAsync(cancellationToken);
    }
}