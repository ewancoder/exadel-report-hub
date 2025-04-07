using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using System.Linq.Expressions;
using ExportPro.Common.Models.MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;

namespace ExportPro.Common.DataAccess.MongoDB.Repository;
public abstract class MongoRepositoryBase<TDocument> : IRepository<TDocument> where TDocument : IModel
{
    private readonly ICollectionProvider _collectionProvider;
    private readonly Lazy<IMongoCollection<TDocument>> _collectionAccessor;
    private readonly FilterDefinitionBuilder<TDocument> _fdb;

    protected MongoRepositoryBase(ICollectionProvider collectionProvider)
    {
        _collectionProvider = collectionProvider ?? throw new ArgumentNullException(nameof(collectionProvider));
        _collectionAccessor = new Lazy<IMongoCollection<TDocument>>(
            () => _collectionProvider.GetCollection<TDocument>(),
            LazyThreadSafetyMode.ExecutionAndPublication);

        _fdb = new FilterDefinitionBuilder<TDocument>();
    }

    protected virtual IMongoCollection<TDocument> Collection => _collectionAccessor.Value;

    public virtual async Task AddOneAsync(TDocument entity, CancellationToken cancellationToken)
    {
        await Collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
    }

    public virtual async Task AddManyAsync(ICollection<TDocument> entities, CancellationToken cancellationToken)
    {
        await Collection.InsertManyAsync(entities, cancellationToken: cancellationToken);
    }

    public virtual async Task UpdateOneAsync(TDocument entity, CancellationToken cancellationToken)
    {
        var filter = _fdb.Eq(doc => doc.Id, entity.Id);
        await Collection.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken);
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
        var filter = _fdb.Eq(doc => doc.Id, id);
        return await Collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<TDocument> GetOneAsync(Expression<Func<TDocument, bool>> filter, CancellationToken cancellationToken)
    {
        return await Collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task DeleteAsync(ObjectId id, CancellationToken cancellationToken)
    {
        var filter = _fdb.Eq(doc => doc.Id, id);
        await Collection.DeleteOneAsync(filter, cancellationToken);
    }

    public virtual async Task SoftDeleteAsync(ObjectId id, CancellationToken cancellationToken)
    {
        var filter = _fdb.Eq(doc => doc.Id, id);
        var update = Builders<TDocument>.Update.Set("IsDeleted", true);
        await Collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
    }
}