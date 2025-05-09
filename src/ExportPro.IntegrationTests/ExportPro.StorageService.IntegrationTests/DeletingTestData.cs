using ExportPro.Shared.IntegrationTests.MongoDbContext;
using ExportPro.StorageService.Models.Models;
using MongoDB.Driver;
using TechTalk.SpecFlow;

namespace ExportPro.StorageService.IntegrationTests;

[Binding]
public class DeletingTestData
{
    private IMongoDbContext<Currency> _mongoDbContextCurrency = new MongoDbContext<Currency>();
    private IMongoDbContext<Client> _mongoDbContextClient = new MongoDbContext<Client>();
    private IMongoDbContext<Country> _mongoDbContextCountry = new MongoDbContext<Country>();
    private IMongoDbContext<Customer> _mongoDbContextCustomer = new MongoDbContext<Customer>();
    private IMongoDbContext<Invoice> _mongoDbContextInvoice = new MongoDbContext<Invoice>();

    [AfterScenario]
    public async Task CleanUp()
    {
        await _mongoDbContextClient.Collection.DeleteManyAsync(x => x.CreatedBy == "SuperAdminTest");

        await _mongoDbContextCountry.Collection.DeleteManyAsync(x => x.CreatedBy == "OwnerUserTest");
        await _mongoDbContextCountry.Collection.DeleteManyAsync(x => x.CreatedBy == "ClientAdminTest");
        await _mongoDbContextCountry.Collection.DeleteManyAsync(x => x.CreatedBy == "OperatorTest");
        await _mongoDbContextCurrency.Collection.DeleteManyAsync(x => x.CreatedBy == "OwnerUserTest");
        await _mongoDbContextCurrency.Collection.DeleteManyAsync(x => x.CreatedBy == "ClientAdminTest");
        await _mongoDbContextCurrency.Collection.DeleteManyAsync(x => x.CreatedBy == "OperatorTest");
        await _mongoDbContextCustomer.Collection.DeleteManyAsync(x => x.CreatedBy == "OwnerUserTest");
        await _mongoDbContextCustomer.Collection.DeleteManyAsync(x => x.CreatedBy == "ClientAdminTest");
        await _mongoDbContextCustomer.Collection.DeleteManyAsync(x => x.CreatedBy == "OperatorTest");
        await _mongoDbContextInvoice.Collection.DeleteManyAsync(x => x.CreatedBy == "OwnerUserTest");
    }
}
