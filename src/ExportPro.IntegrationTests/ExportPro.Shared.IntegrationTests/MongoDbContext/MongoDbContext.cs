using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Services;
using ExportPro.Common.Models.MongoDB;
using ExportPro.Shared.IntegrationTests.Configs;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace ExportPro.Shared.IntegrationTests.MongoDbContext;

public class MongoDbContext<T> : IMongoDbContext<T>
    where T : IModel
{
    private readonly IConfiguration _config = LoadingConfig.LoadConfig();

    public MongoDbContext(string? name = null)
    {
        IMongoDbConnectionFactory connectionFactory = new MongoDbConnectionFactory(_config);
        ICollectionProvider collectionProvider = new DefaultCollectionProvider(connectionFactory);
        Collection = collectionProvider.GetCollection<T>(name);
    }

    public IMongoCollection<T> Collection { get; }
}
