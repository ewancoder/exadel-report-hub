using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using MongoDB.Driver;

namespace ExportPro.Common.DataAccess.MongoDB.Contexts;

public class ExportProMongoContext
{
    private readonly IMongoDatabase _database;

    public ExportProMongoContext(IMongoDbConnectionFactory connectionFactory)
    {
        _database = connectionFactory.GetDatabase();
    }

    // Collections
    //public IMongoCollection<Invoice> Invoices => _database.GetCollection<Invoice>("Invoices");
    // Add other collections as needed
}
