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

namespace ExportPro.Export.IntegrationTests.Steps;

[Binding]
[Scope(Tag = "InvoiceExport")]
public class InvoiceExportSteps
{
    private readonly IMongoDbContext<Invoice> _mongoDbContext = new MongoDbContext<Invoice>();
    private readonly IMongoDbContext<Country> _mongoDbContextCountry = new MongoDbContext<Country>();
    private readonly IMongoDbContext<Currency> _mongoDbContextCurrency = new MongoDbContext<Currency>();
    private readonly IMongoDbContext<Customer> _mongoDbContextCustomer = new MongoDbContext<Customer>();
    private readonly IMongoDbContext<Client> _mongoDbContextClient = new MongoDbContext<Client>();
    private HttpResponseMessage _invoicepdf;
    private ICountryApi _countryController;
    private ICurrencyApi _currencyController;
    private ICustomerApi _customerController;
    private IInvoiceApi _invoiceController;
    private IClientApi _clientController;
    private IInvoiceExport _invoiceExport;
    private CreateInvoiceDto _invoiceDto;
    private Guid _countryId;
    private Guid _currencyId;
    private Guid _currencyIdForItem;
    private Guid _customerId;
    private Guid _clientId;
    private Guid _invoiceId;

    [Given(@"The user is logged in with email '(.*)' and password '(.*)' and has necessary permissions")]
    public async Task GivenTheUserIsLoggedInWithEmailAndPasswordAndHasNecessaryPermissions(
        string email,
        string password
    )
    {
        string jwtToken = await UserLogin.Login(email, password);
        string jwtTokenForClient = await UserLogin.Login("SuperAdminTest@gmail.com", "SuperAdminTest2@");
        HttpClient httpClient = HttpClientForRefit.GetHttpClient(jwtToken, 1200);
        var httpClientForClient = HttpClientForRefit.GetHttpClient(jwtTokenForClient, 1500);
        _countryController = RestService.For<ICountryApi>(httpClient);
        _currencyController = RestService.For<ICurrencyApi>(httpClient);
        _invoiceExport = RestService.For<IInvoiceExport>(httpClient);
        _invoiceController = RestService.For<IInvoiceApi>(
            httpClient,
            new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(
                    new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                    }
                ),
            }
        );
        _customerController = RestService.For<ICustomerApi>(httpClient);
        _clientController = RestService.For<IClientApi>(httpClientForClient);
    }

    [Given("The user has valid client id")]
    public async Task GivenTheUserHasValidClientId()
    {
        ClientDto clientDto = new() { Name = "ClientISInvoiceTest######", Description = "Description" };
        var clientResponse = await _clientController.CreateClient(clientDto);
        var clientExists = await _mongoDbContextClient
            .Collection.Find(x => x.Name == clientResponse.Data!.Name)
            .FirstOrDefaultAsync();
        Assert.That(clientExists, Is.Not.EqualTo(null));
        Assert.That(clientExists.Name, Is.EqualTo(clientResponse.Data!.Name));
        _clientId = clientResponse.Data.Id;
    }

    [Given("The user created following currency for invoice and stored the currency id")]
    public async Task GivenTheUserCreatedFollowingCurrencyForInvoiceAndStoredTheCurrencyId(Table table)
    {
        CurrencyDto cur = table.CreateInstance<CurrencyDto>();
        var currency = await _currencyController.Create(cur);
        var currencyExists = await _mongoDbContextCurrency
            .Collection.Find(x =>
                x.CurrencyCode == currency.Data!.CurrencyCode && x.CreatedBy == currency.Data.CreatedBy
            )
            .FirstOrDefaultAsync();
        Assert.That(currencyExists, Is.Not.EqualTo(null));
        Assert.That(currencyExists.CurrencyCode, Is.EqualTo(cur.CurrencyCode));
        _currencyId = currency.Data!.Id;
    }

    [Given("The user created following currency for item and stored the currency id")]
    public async Task GivenTheUserCreatedFollowingCurrencyForItemAndStoredTheCurrencyId(Table table)
    {
        CurrencyDto cur = table.CreateInstance<CurrencyDto>();
        var currency = await _currencyController.Create(cur);
        var currencyExists = await _mongoDbContextCurrency
            .Collection.Find(x =>
                x.CurrencyCode == currency.Data!.CurrencyCode && x.CreatedBy == currency.Data.CreatedBy
            )
            .FirstOrDefaultAsync();
        Assert.That(currencyExists, Is.Not.EqualTo(null));
        Assert.That(currencyExists.CurrencyCode, Is.EqualTo(cur.CurrencyCode));
        _currencyIdForItem = currency.Data!.Id;
    }

    [Given("The user created following country and stored the country id")]
    public async Task GivenTheUserCreatedFollowingCountryAndStoredTheCountryId(Table table)
    {
        CreateCountryDto countryDto = table.CreateInstance<CreateCountryDto>();
        countryDto.CurrencyId = _currencyId;
        var country = await _countryController.Create(countryDto);
        var countryExists = await _mongoDbContextCountry
            .Collection.Find(x => x.Name == country.Data!.Name)
            .FirstOrDefaultAsync();
        Assert.That(countryExists, Is.Not.EqualTo(null));
        Assert.That(countryExists.Name, Is.EqualTo(country.Data!.Name));
        _countryId = country.Data.Id;
    }

    [Given("The user created following customer and stored the customer id")]
    public async Task GivenTheUserCreatedFollowingCustomerAndStoredTheCustomerId(Table table)
    {
        CreateUpdateCustomerDto customerDto = table.CreateInstance<CreateUpdateCustomerDto>();
        customerDto.CountryId = _countryId;
        var customer = await _customerController.Create(customerDto);
        var customerExists = await _mongoDbContextCustomer
            .Collection.Find(x => x.Name == customer.Data!.Name)
            .FirstOrDefaultAsync();
        Assert.That(customerExists, Is.Not.EqualTo(null));
        Assert.That(customerExists.Name, Is.EqualTo(customer.Data!.Name));
        _customerId = customer.Data.Id;
    }

    [Given("The user wants to create following invoice")]
    public Task GivenTheUserCreatedFollowingInvoiceAndStoredTheInvoiceId(Table table)
    {
        _invoiceDto = table.CreateInstance<CreateInvoiceDto>();
        _invoiceDto.CustomerId = _customerId;
        _invoiceDto.ClientId = _clientId;
        _invoiceDto.CurrencyId = _currencyId;
        _invoiceDto.ClientCurrencyId = _currencyIdForItem;
        return Task.CompletedTask;
    }

    [Given("the invoice contains the following items")]
    public Task GivenTheInvoiceContainsTheFollowingItems(Table table)
    {
        List<ItemDtoForClient> items = new List<ItemDtoForClient>();
        ItemDtoForClient item = new()
        {
            Name = table.Rows[0]["Name"],
            Description = table.Rows[0]["Description"],
            Price = double.Parse(table.Rows[0]["Price"]),
            Status = Enum.Parse<Status>(table.Rows[0]["Status"]),
            CurrencyId = _currencyIdForItem,
        };
        items.Add(item);
        _invoiceDto.Items = items;
        return Task.CompletedTask;
    }

    [Given("The user created invoice and  stored invoice id")]
    public async Task GivenTheUserCreatedInvoiceAndStoredInvoiceId()
    {
        var invoice = await _invoiceController.Create(_invoiceDto);
        _invoiceId = invoice.Data!.Id;
    }

    [When("The user sends the invoice export request")]
    public async Task WhenTheUserSendsTheInvoiceExportRequest()
    {
        _invoicepdf = await _invoiceExport.GetInvoiceExportPdf(_invoiceId);
    }

    [Then("The invoice should be exported as pdf")]
    public Task ThenTheInvoiceShouldBeExportedAsPdf()
    {
        var responselen = _invoicepdf.Content.Headers.ContentLength;
        var contentType = _invoicepdf.Content.Headers.ContentType!.MediaType;
        Assert.That(_invoicepdf, Is.Not.EqualTo(null));
        Assert.That(responselen, Is.GreaterThan(500));
        Assert.That(contentType, Is.EqualTo("application/pdf"));
        _invoicepdf.Dispose();
        return Task.CompletedTask;
    }

    [AfterScenario("@InvoiceExport")]
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
