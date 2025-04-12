using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Repository;
using ExportPro.StorageService.Models.Models;
using MongoDB.Driver;
namespace ExportPro.StorageService.DataAccess.Repositories;

public class ClientRepository : MongoRepositoryBase<Client>
{
    private  IMongoCollection<Client> _clients;
    private readonly IMongoDbConnectionFactory _mongoDbConnectionFactory;

    public ClientRepository(IMongoDbConnectionFactory mongoDbConnectionFactory,ICollectionProvider collectionProvider) : base(collectionProvider)
    {
        _clients=collectionProvider.GetCollection<Client>("Client");
    }
    public Task<List<Client>> GetClients()
    {
        return _clients.Find(_=>true).ToListAsync(); 
    }
    public Task<Client> GetClientByName(string name)
    {
        return _clients.Find(x => x.Name == name).FirstOrDefaultAsync();
    }
}
