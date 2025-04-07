using ExportPro.Common.Models.MongoDB;
using MongoDB.Driver;

namespace ExportPro.Common.DataAccess.MongoDB.Interfaces;

public interface ICollectionProvider
{
    IMongoCollection<TDocument> GetCollection<TDocument>(string? name = null) where TDocument : IModel;
}
