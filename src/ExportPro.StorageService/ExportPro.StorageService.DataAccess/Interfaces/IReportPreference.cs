using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.StorageService.Models.Models;
using MongoDB.Bson;

namespace ExportPro.StorageService.DataAccess.Interfaces;

public interface IReportPreference : IRepository<ReportPreference>
{
    Task<List<ReportPreference>> GetAllByClientIdAsync(ObjectId clientId, CancellationToken cancellationToken);
}