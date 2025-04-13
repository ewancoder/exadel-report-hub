using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.StorageService.Models.Models;

namespace ExportPro.StorageService.DataAccess.Interfaces;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<List<Customer>> GetAllAsync(CancellationToken cancellationToken);
}
