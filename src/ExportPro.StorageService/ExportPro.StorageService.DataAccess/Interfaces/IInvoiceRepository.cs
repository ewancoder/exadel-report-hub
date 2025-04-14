using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.Models.Models;
using MongoDB.Bson;

namespace ExportPro.StorageService.DataAccess.Interfaces;

public interface IInvoiceRepository : IRepository<Invoice>
{
    Task<List<Invoice>> GetAllByClientIdAsync(string clientId, CancellationToken cancellationToken);
    Task<List<Invoice>> GetByStatusAsync(Status status, CancellationToken cancellationToken);
    Task<List<Invoice>> GetAllAsync(CancellationToken cancellationToken);
    Task<bool> ExistsAsync(ObjectId id, CancellationToken cancellationToken);
}