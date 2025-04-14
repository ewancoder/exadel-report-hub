using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.StorageService.Models.Models;

namespace ExportPro.StorageService.DataAccess.Interfaces;

public interface IClientRepository: IRepository<Client>
{
    Task<List<Client>> GetClients();
    Task<Client> GetClientByName(string name);
};
