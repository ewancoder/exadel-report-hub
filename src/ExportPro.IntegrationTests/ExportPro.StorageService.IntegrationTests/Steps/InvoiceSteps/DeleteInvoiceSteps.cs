using System.Text.Json;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Shared.IntegrationTests.Auth;
using ExportPro.Shared.IntegrationTests.Helpers;
using ExportPro.Shared.IntegrationTests.MongoDbContext;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.DTOs.CountryDTO;
using ExportPro.StorageService.SDK.DTOs.CustomerDTO;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;
using ExportPro.StorageService.SDK.Refit;
using MongoDB.Driver;
using Refit;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace ExportPro.StorageService.IntegrationTests.Steps.InvoiceSteps;

[Binding]
[Scope(Tag = "DeleteInvoice")]
public class DeleteInvoiceSteps
{
    private readonly IMongoDbContext<Invoice> _mongoDbContext = new MongoDbContext<Invoice>();
    private readonly IMongoDbContext<Client> _mongoDbContextClient = new MongoDbContext<Client>();
    private readonly IMongoDbContext<Country> _mongoDbContextCountry = new MongoDbContext<Country>();
    private readonly IMongoDbContext<Currency> _mongoDbContextCurrency = new MongoDbContext<Currency>();
    private readonly IMongoDbContext<Customer> _mongoDbContextCustomer = new MongoDbContext<Customer>();
    private IClientApi? _clientApi;
    private Guid _clientId;
    private ICountryApi? _countryApi;
    private Guid _countryId;
    private ICurrencyApi? _currencyApi;
    private Guid _currencyId;
    private Guid _currencyIdForItem;
    private ICustomerApi? _customerApi;
    private Guid _customerId;
    private IInvoiceApi? _invoiceApi;
    private CreateInvoiceDto? _invoiceDto;
    private Guid _invoiceId;

    [Given("The user is logged in with the following credentials and has necessary permissions")]
    public async Task GivenTheUserIsLoggedInWithEmailAndPasswordAndHasNecessaryPermissions(Table table)
    {
        var jwtToken = await UserLogin.Login(table.Rows[0]["Email"], table.Rows[0]["Password"]);
        var jwtTokenForClient = await UserLogin.Login("SuperAdminTest@gmail.com", "SuperAdminTest2@");
        var httpClient = HttpClientForRefit.GetHttpClient(jwtToken, 1500);
        var httpClientForClient = HttpClientForRefit.GetHttpClient(jwtTokenForClient, 1500);
        _countryApi = RestService.For<ICountryApi>(httpClient);
        _currencyApi = RestService.For<ICurrencyApi>(httpClient);
        _invoiceApi = RestService.For<IInvoiceApi>(
            httpClient,
            new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    }
                ),
            }
        );
        _customerApi = RestService.For<ICustomerApi>(httpClient);
        _clientApi = RestService.For<IClientApi>(httpClientForClient);
    }

    [Given("the user has valid client id")]
    public async Task GivenTheUserHasValidClientId()
    {
        ClientDto clientDto = new() { Name = "ClientISInvoiceTest######", Description = "Description" };
        var clientResponse = await _clientApi!.CreateClient(clientDto);
        var clientExists = await _mongoDbContextClient
            .Collection.Find(x => x.Name == clientResponse.Data!.Name)
            .FirstOrDefaultAsync();
        Assert.That(clientExists, Is.Not.EqualTo(null));
        Assert.That(clientExists.Name, Is.EqualTo(clientResponse.Data!.Name));
        _clientId = clientResponse.Data.Id;
    }

    [Given("The user created the following currency for invoice and stored the currency id")]
    public async Task GivenTheUserCreatedFollowingCurrencyForInvoiceAndStoredTheCurrencyId(Table table)
    {
        var cur = table.CreateInstance<CurrencyDto>();
        var currency = await _currencyApi!.Create(cur);
        var currencyExists = await _mongoDbContextCurrency
            .Collection.Find(x =>
                x.CurrencyCode == currency.Data!.CurrencyCode && x.CreatedBy == currency.Data.CreatedBy
            )
            .FirstOrDefaultAsync();
        Assert.That(currencyExists, Is.Not.EqualTo(null));
        Assert.That(currencyExists.CurrencyCode, Is.EqualTo(cur.CurrencyCode));
        _currencyId = currency.Data!.Id;
    }

    [Given("The user created the following currency for item and stored the currency id")]
    public async Task GivenTheUserCreatedFollowingCurrencyForItemAndStoredTheCurrencyId(Table table)
    {
        var cur = table.CreateInstance<CurrencyDto>();
        var currency = await _currencyApi!.Create(cur);
        var currencyExists = await _mongoDbContextCurrency
            .Collection.Find(x =>
                x.CurrencyCode == currency.Data!.CurrencyCode && x.CreatedBy == currency.Data.CreatedBy
            )
            .FirstOrDefaultAsync();
        Assert.That(currencyExists, Is.Not.EqualTo(null));
        Assert.That(currencyExists.CurrencyCode, Is.EqualTo(cur.CurrencyCode));
        _currencyIdForItem = currency.Data!.Id;
    }

    [Given("The user created the following country and stored the country id")]
    public async Task GivenTheUserCreatedFollowingCountryAndStoredTheCountryId(Table table)
    {
        var countryDto = table.CreateInstance<CreateCountryDto>();
        countryDto.CurrencyId = _currencyId;
        var country = await _countryApi!.Create(countryDto);
        var countryExists = await _mongoDbContextCountry
            .Collection.Find(x => x.Name == country.Data!.Name)
            .FirstOrDefaultAsync();
        Assert.That(countryExists, Is.Not.EqualTo(null));
        Assert.That(countryExists.Name, Is.EqualTo(country.Data!.Name));
        _countryId = country.Data.Id;
    }

    [Given("The user created the following customer and stored the customer id")]
    public async Task GivenTheUserCreatedFollowingCustomerAndStoredTheCustomerId(Table table)
    {
        var customerDto = table.CreateInstance<CreateUpdateCustomerDto>();
        customerDto.CountryId = _countryId;
        var customer = await _customerApi!.Create(customerDto);
        var customerExists = await _mongoDbContextCustomer
            .Collection.Find(x => x.Name == customer.Data!.Name)
            .FirstOrDefaultAsync();
        Assert.That(customerExists, Is.Not.EqualTo(null));
        Assert.That(customerExists.Name, Is.EqualTo(customer.Data!.Name));
        _customerId = customer.Data.Id;
    }

    [Given("The user has the following invoice")]
    public Task GivenTheUserHasFollowingInvoiceAndStoredTheInvoiceId(Table table)
    {
        _invoiceDto = table.CreateInstance<CreateInvoiceDto>();
        _invoiceDto.CustomerId = _customerId;
        _invoiceDto.ClientId = _clientId;
        _invoiceDto.CurrencyId = _currencyId;
        _invoiceDto.ClientCurrencyId = _currencyIdForItem;
        return Task.CompletedTask;
    }

    [Given("the invoice contains the following items and the invoice id is stored")]
    public async Task GivenTheInvoiceContainsTheFollowingItemsAndTheInvoiceIdIsStored(Table table)
    {
        var items = new List<ItemDtoForClient>();
        ItemDtoForClient item = new()
        {
            Name = table.Rows[0]["Name"],
            Description = table.Rows[0]["Description"],
            Price = double.Parse(table.Rows[0]["Price"]),
            Status = Enum.Parse<Status>(table.Rows[0]["Status"]),
            CurrencyId = _currencyIdForItem,
        };
        items.Add(item);
        _invoiceDto!.Items = items;
        var invoice = await _invoiceApi!.Create(_invoiceDto);
        var invoiceExists = await _mongoDbContext
            .Collection.Find(x => x.InvoiceNumber == invoice.Data!.InvoiceNumber)
            .FirstOrDefaultAsync();
        Assert.That(invoiceExists, Is.Not.EqualTo(null));
        Assert.That(invoiceExists.InvoiceNumber, Is.EqualTo(invoice.Data!.InvoiceNumber));
        _invoiceId = invoice.Data.Id;
    }

    [When("The user sends the invoice delete request")]
    public async Task WhenTheUserSendsTheInvoiceDeleteRequest()
    {
        await _invoiceApi!.Delete(_invoiceId);
    }

    [Then("The invoice should be deleted")]
    public async Task ThenTheInvoiceShouldBeDeleted()
    {
        var invoice = await _mongoDbContext.Collection.Find(x => x.Id == _invoiceId.ToObjectId()).FirstOrDefaultAsync();
        Assert.That(invoice, Is.Not.EqualTo(null));
        Assert.That(invoice.InvoiceNumber, Is.EqualTo(_invoiceDto!.InvoiceNumber));
        Assert.That(invoice.IsDeleted, Is.EqualTo(true));
    }

    [AfterScenario("@DeleteInvoice")]
    public async Task CleanUp()
    {
        await _mongoDbContextCountry.Collection.DeleteOneAsync(x => x.Id == _countryId.ToObjectId());
        await _mongoDbContextCurrency.Collection.DeleteOneAsync(x => x.Id == _currencyId.ToObjectId());
        await _mongoDbContextCurrency.Collection.DeleteOneAsync(x => x.Id == _currencyIdForItem.ToObjectId());
        await _mongoDbContextCustomer.Collection.DeleteOneAsync(x => x.Id == _customerId.ToObjectId());
        await _mongoDbContextClient.Collection.DeleteOneAsync(x => x.Id == _clientId.ToObjectId());
        await _mongoDbContext.Collection.DeleteOneAsync(x => x.Id == _invoiceId.ToObjectId());
    }
}
