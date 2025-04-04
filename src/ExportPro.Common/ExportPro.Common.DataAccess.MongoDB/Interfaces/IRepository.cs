using System.Linq.Expressions;
using ExportPro.Common.Models.MongoDB;
using MongoDB.Bson;

namespace ExportPro.Common.DataAccess.MongoDB.Interfaces;

public interface IRepository<TDocument> where TDocument : IModel
{
    public Task AddOneAsync(TDocument entity, CancellationToken cancellationToken);
    public Task AddManyAsync(ICollection<TDocument> entities, CancellationToken cancellationToken);
    public Task UpdateOneAsync(TDocument entity, CancellationToken cancellationToken);
    public Task UpdateManyAsync(ICollection<TDocument> entities, CancellationToken cancellationToken);
    public Task<TDocument> GetByIdAsync(ObjectId id, CancellationToken cancellationToken);
    public Task<TDocument> GetOneAsync(Expression<Func<TDocument, bool>> filter, CancellationToken cancellationToken);
}