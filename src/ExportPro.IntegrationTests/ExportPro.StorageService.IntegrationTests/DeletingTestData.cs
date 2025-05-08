using ExportPro.Shared.IntegrationTests.MongoDbContext;
using ExportPro.StorageService.Models.Models;
using MongoDB.Driver;
using TechTalk.SpecFlow;

namespace ExportPro.StorageService.IntegrationTests;

[Binding]
public class DeletingTestData
{
    private IMongoDbContext<Currency> _mongoCollectionCurrency = new MongoDbContext<Currency>();
    private IMongoDbContext<Client> _mongoCollectionClient = new MongoDbContext<Client>();

    [AfterScenario]
    public async Task CleanUp()
    {
        var client = await _mongoCollectionClient!.Collection.Find(x => x.Name == "ClientISme").FirstOrDefaultAsync();
        if (client != null)
        {
            await _mongoCollectionClient.Collection.DeleteOneAsync(x => x.Id == client.Id);
        }

        var clientDelete = await _mongoCollectionClient
            .Collection.Find(x => x.Name == "DeleteIsI")
            .FirstOrDefaultAsync();
        if (clientDelete != null)
        {
            await _mongoCollectionClient.Collection.DeleteOneAsync(x => x.Id == clientDelete.Id);
        }
        var currency = await _mongoCollectionCurrency
            .Collection.Find(x => x.CurrencyCode == "DDD")
            .FirstOrDefaultAsync();
        if (currency != null)
        {
            await _mongoCollectionCurrency.Collection.DeleteOneAsync(x => x.Id == currency.Id);
        }
        var currencyDelete = await _mongoCollectionCurrency
            .Collection.Find(x => x.CurrencyCode == "ZZZ")
            .FirstOrDefaultAsync();
        if (currencyDelete != null)
        {
            await _mongoCollectionCurrency.Collection.DeleteOneAsync(x => x.Id == currencyDelete.Id);
        }
    }
}
