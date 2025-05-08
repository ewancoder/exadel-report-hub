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

    [AfterScenario]
    public async Task CleanUp()
    {
        var client = await _mongoDbContextClient!.Collection.Find(x => x.Name == "ClientISme").FirstOrDefaultAsync();
        if (client != null)
        {
            await _mongoDbContextClient.Collection.DeleteOneAsync(x => x.Id == client.Id);
        }

        var clientDelete = await _mongoDbContextClient
            .Collection.Find(x => x.Name == "DeleteIsI")
            .FirstOrDefaultAsync();
        if (clientDelete != null)
        {
            await _mongoDbContextClient.Collection.DeleteOneAsync(x => x.Id == clientDelete.Id);
        }
        var currency = await _mongoDbContextCurrency
            .Collection.Find(x => x.CurrencyCode == "DDD")
            .FirstOrDefaultAsync();
        if (currency != null)
        {
            await _mongoDbContextCurrency.Collection.DeleteOneAsync(x => x.Id == currency.Id);
        }
        var currencyDelete = await _mongoDbContextCurrency
            .Collection.Find(x => x.CurrencyCode == "ZZZ")
            .FirstOrDefaultAsync();
        if (currencyDelete != null)
        {
            await _mongoDbContextCurrency.Collection.DeleteOneAsync(x => x.Id == currencyDelete.Id);
        }

        var createCountryCurrency = await _mongoDbContextCurrency
            .Collection.Find(x => x.CurrencyCode == "QQQ")
            .FirstOrDefaultAsync();
        if (createCountryCurrency != null)
        {
            await _mongoDbContextCurrency.Collection.DeleteOneAsync(x => x.Id == createCountryCurrency.Id);
        }

        var createCountry = await _mongoDbContextCountry
            .Collection.Find(x => x.Name == "TestUsa####")
            .FirstOrDefaultAsync();
        if (createCountry != null)
        {
            await _mongoDbContextCountry.Collection.DeleteOneAsync(x => x.Id == createCountry.Id);
        }

        var deleteCountryCurrency = await _mongoDbContextCurrency
            .Collection.Find(x => x.CurrencyCode == "QQQ")
            .FirstOrDefaultAsync();
        if (deleteCountryCurrency != null)
        {
            await _mongoDbContextCurrency.Collection.DeleteOneAsync(x => x.Id == deleteCountryCurrency.Id);
        }

        var deleteCountry = await _mongoDbContextCountry
            .Collection.Find(x => x.Name == "ShouldBeDeletedTest####")
            .FirstOrDefaultAsync();
        if (deleteCountry != null)
        {
            await _mongoDbContextCountry.Collection.DeleteOneAsync(x => x.Id == deleteCountry.Id);
        }

        var createCustomerCurrency = await _mongoDbContextCurrency
            .Collection.Find(x => x.CurrencyCode == "PPP")
            .FirstOrDefaultAsync();
        if (createCustomerCurrency != null)
        {
            await _mongoDbContextCurrency.Collection.DeleteOneAsync(x => x.Id == createCustomerCurrency.Id);
        }
        var createCustomerCountry = await _mongoDbContextCountry
            .Collection.Find(x => x.Name == "TestUsaCUSTOMER####")
            .FirstOrDefaultAsync();
        if (createCustomerCountry != null)
        {
            await _mongoDbContextCountry.Collection.DeleteOneAsync(x => x.Id == createCustomerCountry.Id);
        }
        var createCustomer = await _mongoDbContextCustomer
            .Collection.Find(x => x.Email == "TESTUSER####@gmail.com")
            .FirstOrDefaultAsync();
        if (createCustomer != null)
        {
            await _mongoDbContextCustomer.Collection.DeleteOneAsync(x => x.Id == createCustomer.Id);
        }
        var deleteCustomerCurrency = await _mongoDbContextCurrency
            .Collection.Find(x => x.CurrencyCode == "PPP")
            .FirstOrDefaultAsync();
        if (deleteCustomerCurrency != null)
        {
            await _mongoDbContextCurrency.Collection.DeleteOneAsync(x => x.Id == deleteCustomerCurrency.Id);
        }
        var deleteCustomerCountry = await _mongoDbContextCountry
            .Collection.Find(x => x.Name == "TestUsaCUSTOMERDeleting####")
            .FirstOrDefaultAsync();
        if (deleteCustomerCountry != null)
        {
            await _mongoDbContextCountry.Collection.DeleteOneAsync(x => x.Id == deleteCustomerCountry.Id);
        }
        var deleteCustomer = await _mongoDbContextCustomer
            .Collection.Find(x => x.Email == "TESTUSER####Deleting@gmail.com")
            .FirstOrDefaultAsync();
        if (deleteCustomer != null)
        {
            await _mongoDbContextCustomer.Collection.DeleteOneAsync(x => x.Id == deleteCustomer.Id);
        }
    }
}
