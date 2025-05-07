using ExportPro.Common.Models.MongoDB;
using MongoDB.Driver;

namespace ExportPro.StorageService.IntegrationTests.MongoDbContext;

public interface IMongoDbContext<T>
    where T : IModel
{
    public IMongoCollection<T> Collection { get; }
}
