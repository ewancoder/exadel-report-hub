using ExportPro.Common.Models.MongoDB;
using MongoDB.Driver;

namespace ExportPro.Shared.IntegrationTests.MongoDbContext;

public interface IMongoDbContext<T>
{
    public IMongoCollection<T> Collection { get; }
}
