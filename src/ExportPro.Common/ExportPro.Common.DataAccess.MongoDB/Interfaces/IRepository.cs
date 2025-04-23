using System.Linq.Expressions;
using ExportPro.Common.Models.MongoDB;
using MongoDB.Bson;

namespace ExportPro.Common.DataAccess.MongoDB.Interfaces;

public interface IRepository<TDocument> where TDocument : IModel
{
    public Task<TDocument> AddOneAsync(TDocument entity, CancellationToken cancellationToken);
    public Task<TDocument> DeleteAsync(ObjectId id, CancellationToken cancellationToken);
    public Task<TDocument> SoftDeleteAsync(ObjectId id, CancellationToken cancellationToken);
    public Task<ICollection<TDocument>> AddManyAsync(ICollection<TDocument> entities, CancellationToken cancellationToken);
    public Task<TDocument> UpdateOneAsync(TDocument entity, CancellationToken cancellationToken);
    public Task<ICollection<TDocument>> UpdateManyAsync(ICollection<TDocument> entities, CancellationToken cancellationToken);
    public Task<TDocument> GetByIdAsync(ObjectId id, CancellationToken cancellationToken);
    public Task<TDocument> GetOneAsync(Expression<Func<TDocument, bool>> filter, CancellationToken cancellationToken);
}