using MongoDB.Driver;

namespace ExportPro.Common.DataAccess.MongoDB.Interfaces;

public interface IMongoDbConnectionFactory
{
    public IMongoDatabase GetDatabase();
    public IMongoClient GetClient();
}
