using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using System.Linq.Expressions;
using ExportPro.Common.Models.MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;

namespace ExportPro.Common.DataAccess.MongoDB.Repository;
public abstract class Repository<TDocument> : IRepository<TDocument> where TDocument : IModel
{
    protected readonly IMongoCollection<TDocument> _collection;

    protected Repository(IMongoDbConnectionFactory connectionFactory, string collectionName)
    {
        _collection = connectionFactory.GetDatabase().GetCollection<TDocument>(collectionName);
    }

    public virtual async Task AddOneAsync(TDocument entity, CancellationToken cancellationToken)
    {
        await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
    }

    public virtual async Task AddManyAsync(ICollection<TDocument> entities, CancellationToken cancellationToken)
    {
        await _collection.InsertManyAsync(entities, cancellationToken: cancellationToken);
    }

    public virtual async Task UpdateOneAsync(TDocument entity, CancellationToken cancellationToken)
    {
        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, entity.Id);
        await _collection.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken);
    }

    public virtual async Task UpdateManyAsync(ICollection<TDocument> entities, CancellationToken cancellationToken)
    {
        foreach (var entity in entities)
        {
            await UpdateOneAsync(entity, cancellationToken);
        }
    }

    public virtual async Task<TDocument> GetByIdAsync(ObjectId id, CancellationToken cancellationToken)
    {
        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);
        return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<TDocument> GetOneAsync(Expression<Func<TDocument, bool>> filter, CancellationToken cancellationToken)
    {
        return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }
}