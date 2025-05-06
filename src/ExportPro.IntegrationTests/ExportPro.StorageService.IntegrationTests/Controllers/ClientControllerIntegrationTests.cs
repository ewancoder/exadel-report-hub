using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Services;
using ExportPro.StorageService.Models.Models;
using Microsoft.Extensions.Configuration;

namespace ExportPro.StorageService.IntegrationTests.Controllers;

[TestFixture]
public class ClientControllerIntegrationTests
{
    [SetUp]
    public void Setup()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new[]
                {
                    new KeyValuePair<string, string>("ConnectionStrings:MongoDB", "mongodb://localhost:27017"),
                    new KeyValuePair<string, string>("MongoDB:DatabaseName", "ExportProDb"),
                }!
            )
            .Build();
        IMongoDbConnectionFactory connectionFactory = new MongoDbConnectionFactory(configuration);
        ICollectionProvider collectionProvider = new DefaultCollectionProvider(connectionFactory);
        // var client = collectionProvider.GetCollection<Client>();
    }
}
