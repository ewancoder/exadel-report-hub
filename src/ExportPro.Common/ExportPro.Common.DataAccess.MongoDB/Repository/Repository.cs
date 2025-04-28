using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using System.Linq.Expressions;
using ExportPro.Common.Models.MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;

namespace ExportPro.Common.DataAccess.MongoDB.Repository;
public abstract class BaseRepository<TDocument> : IRepository<TDocument> where TDocument : IModel
{
    private readonly ICollectionProvider _collectionProvider;
    private readonly Lazy<IMongoCollection<TDocument>> _collectionAccessor;
    private readonly FilterDefinitionBuilder<TDocument> _fdb;

    protected BaseRepository(ICollectionProvider collectionProvider)
    {
        _collectionProvider = collectionProvider ?? throw new ArgumentNullException(nameof(collectionProvider));
        _collectionAccessor = new Lazy<IMongoCollection<TDocument>>(
            () => _collectionProvider.GetCollection<TDocument>(),
            LazyThreadSafetyMode.ExecutionAndPublication);

        _fdb = new FilterDefinitionBuilder<TDocument>();
    }

    protected virtual IMongoCollection<TDocument> Collection => _collectionAccessor.Value;

    public virtual async Task<TDocument> AddOneAsync(TDocument entity, CancellationToken cancellationToken = default)
    {
        await Collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
        return entity;
    }

    public virtual async Task<ICollection<TDocument>> AddManyAsync(ICollection<TDocument> entities, CancellationToken cancellationToken = default)
    {
        await Collection.InsertManyAsync(entities, cancellationToken: cancellationToken);
        return entities;
    }

    public virtual async Task<TDocument> UpdateOneAsync(TDocument entity, CancellationToken cancellationToken = default)
    {
        var filter = _fdb.Eq(doc => doc.Id, entity.Id);
        await Collection.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken);
        return entity;
    }

    public virtual async Task<ICollection<TDocument>> UpdateManyAsync(ICollection<TDocument> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            await UpdateOneAsync(entity, cancellationToken);
        }
        return entities;
    }

    public virtual async Task<TDocument> GetByIdAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        var filter = _fdb.Eq(doc => doc.Id, id);
        return await Collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<TDocument> GetOneAsync(Expression<Func<TDocument, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await Collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<TDocument> DeleteAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        var filter = _fdb.Eq(doc => doc.Id, id);
        var document = await GetByIdAsync(id, cancellationToken);
     
        await Collection.DeleteOneAsync(filter, cancellationToken);
        return document;
    }

    public virtual async Task<TDocument> SoftDeleteAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        var filter = _fdb.Eq(doc => doc.Id, id);
        var update = Builders<TDocument>.Update.Set("IsDeleted", true);
        var document = await GetByIdAsync(id, cancellationToken);

        await Collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        return document;
    }
}