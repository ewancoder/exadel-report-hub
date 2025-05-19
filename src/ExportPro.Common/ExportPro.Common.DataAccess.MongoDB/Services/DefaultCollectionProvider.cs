using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.Models.MongoDB;
using MongoDB.Driver;

namespace ExportPro.Common.DataAccess.MongoDB.Services;

public class DefaultCollectionProvider(IMongoDbConnectionFactory connectionFactory) : ICollectionProvider
{
    private readonly IMongoDbConnectionFactory _connectionFactory = connectionFactory;

    public IMongoCollection<TDocument> GetCollection<TDocument>(string? name = null)
        where TDocument : IModel
    {
        if (string.IsNullOrEmpty(name))
            name = GetCollectionName(typeof(TDocument));

        var db = _connectionFactory.GetDatabase();
        return db.GetCollection<TDocument>(name);
    }

    protected virtual string GetCollectionName(Type type)
    {
        return $"{type.Name}";
    }
}
