

using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.StorageService.Models.Models;

namespace ExportPro.StorageService.DataAccess.Interfaces;

public interface IItemRepository : IRepository<Item>
{
    Task<List<Item>> GetFilteredItemsAsync(string? invoiceId = null, string? clientId = null, string? customerId = null, CancellationToken cancellationToken = default);
    Task<List<Item>> GetItemsAsync(CancellationToken cancellationToken = default);
}

