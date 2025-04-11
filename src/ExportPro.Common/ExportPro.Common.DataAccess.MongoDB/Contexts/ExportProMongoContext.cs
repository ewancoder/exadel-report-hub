using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using MongoDB.Driver;

namespace ExportPro.Common.DataAccess.MongoDB.Contexts;

public class ExportProMongoContext(IMongoDbConnectionFactory connectionFactory)
{
    private readonly IMongoDatabase _database = connectionFactory.GetDatabase();
}
