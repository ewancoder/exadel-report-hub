

using ExportPro.Common.Models.MongoDB;
using MongoDB.Bson;
using System.Linq.Expressions;

namespace ExportPro.Common.DataAccess.MongoDB.Interfaces
{
    public interface IRepository<TDocument> where TDocument : IModel
    {
        Task AddOneAsync<TDocument>(TDocument entity, CancellationToken cancellationToken);
        Task AddManyAsync<TDocument>(ICollection<TDocument> entities, CancellationToken cancellationToken);
        Task UpdateOneAsync<TDocument>(TDocument entity, CancellationToken cancellationToken);
        Task UpdateManyAsync<TDocument>(ICollection<TDocument> entities, CancellationToken cancellationToken);
        Task GetByIdAsync<TDocument>(ObjectId id, CancellationToken cancellationToken);
        Task GetOneAsync<TDocument>(Expression<Func<TDocument, bool>> filter, CancellationToken cancellationToken);

    }
}
