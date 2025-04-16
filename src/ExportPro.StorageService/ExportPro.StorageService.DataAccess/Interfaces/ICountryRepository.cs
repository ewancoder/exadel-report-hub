using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Repository;
using ExportPro.StorageService.Models.Models;

namespace ExportPro.StorageService.DataAccess.Interfaces;

public interface ICountryRepository : IRepository<Country>
{
    Task<List<Country>> GetAllAsync(CancellationToken cancellationToken);
}