using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Services;
using ExportPro.Common.Models.MongoDB;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace ExportPro.Shared.IntegrationTests.MongoDbContext;

public class MongoDbContext<T> : IMongoDbContext<T>
    where T : IModel
{
    public MongoDbContext(string? name = null)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new[]
                {
                    new KeyValuePair<string, string>("ConnectionStrings:MongoDB", "mongodb://localhost:27017/"),
                    new KeyValuePair<string, string>("MongoDB:DatabaseName", "ExportProDb"),
                }!
            )
            .Build();
        IMongoDbConnectionFactory connectionFactory = new MongoDbConnectionFactory(configuration);
        ICollectionProvider collectionProvider = new DefaultCollectionProvider(connectionFactory);
        Collection = collectionProvider.GetCollection<T>(name);
    }

    public IMongoCollection<T> Collection { get; }
}
