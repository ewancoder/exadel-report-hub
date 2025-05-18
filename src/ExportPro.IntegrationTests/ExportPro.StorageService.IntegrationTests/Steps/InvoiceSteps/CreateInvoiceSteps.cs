using System.Text.Json;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Shared.IntegrationTests.Auth;
using ExportPro.Shared.IntegrationTests.Helpers;
using ExportPro.Shared.IntegrationTests.MongoDbContext;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.DTOs.CustomerDTO;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;
using ExportPro.StorageService.SDK.Refit;
using MongoDB.Driver;
using Refit;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace ExportPro.StorageService.IntegrationTests.Steps.InvoiceSteps;

[Binding]
[Scope(Tag = "CreateInvoice")]
public class CreateInvoiceSteps
{
    private readonly IMongoDbContext<Invoice> _mongoDbContext = new MongoDbContext<Invoice>();
    private readonly IMongoDbContext<Client> _mongoDbContextClient = new MongoDbContext<Client>();
    private readonly IMongoDbContext<Country> _mongoDbContextCountry = new MongoDbContext<Country>();
    private readonly IMongoDbContext<Currency> _mongoDbContextCurrency = new MongoDbContext<Currency>();
    private readonly IMongoDbContext<Customer> _mongoDbContextCustomer = new MongoDbContext<Customer>();
    private IClientController? _clientApi;
    private Guid _clientId;
    private ICountryController? _countryApi;
    private Guid _countryId;
    private ICurrencyController? _currencyApi;
    private Guid _currencyId;
    private Guid _currencyIdForItem;
    private ICustomerController? _customerApi;
    private Guid _customerId;
    private IInvoiceController? _invoiceApi;
    private CreateInvoiceDto? _invoiceDto;

    [Given(@"The user is logged in with email '(.*)' and password '(.*)' and has necessary permissions")]
    public async Task GivenTheUserIsLoggedInWithEmailAndPasswordAndHasNecessaryPermissions(
        string email,
        string password
    )
    {
        var jwtToken = await UserLogin.Login(email, password);
        var jwtTokenForClient = await UserLogin.Login("SuperAdminTest@gmail.com", "SuperAdminTest2@");
        var httpClient = HttpClientForRefit.GetHttpClient(jwtToken, 1500);
        var httpClientForClient = HttpClientForRefit.GetHttpClient(jwtTokenForClient, 1500);
        _countryApi = RestService.For<ICountryController>(httpClient);
        _currencyApi = RestService.For<ICurrencyController>(httpClient);
        _invoiceApi = RestService.For<IInvoiceController>(
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
        _customerApi = RestService.For<ICustomerController>(httpClient);
        _clientApi = RestService.For<IClientController>(httpClientForClient);
    }

    [Given("The user has valid client id")]
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

    // [Given("The user created following currency for invoice and stored the currency id")]
    // public async Task GivenTheUserCreatedFollowingCurrencyForInvoiceAndStoredTheCurrencyId(Table table)
    // {
    //     var cur = table.CreateInstance<CurrencyDto>();
    //     var currency = await _currencyApi!.Create(cur);
    //     var currencyExists = await _mongoDbContextCurrency
    //         .Collection.Find(x =>
    //             x.CurrencyCode == currency.Data!.CurrencyCode && x.CreatedBy == currency.Data.CreatedBy
    //         )
    //         .FirstOrDefaultAsync();
    //     Assert.That(currencyExists, Is.Not.EqualTo(null));
    //     Assert.That(currencyExists.CurrencyCode, Is.EqualTo(cur.CurrencyCode));
    //     _currencyId = currency.Data!.Id;
    // }
    //
    // [Given("The user created following currency for item and stored the currency id")]
    // public async Task GivenTheUserCreatedFollowingCurrencyForItemAndStoredTheCurrencyId(Table table)
    // {
    //     var cur = table.CreateInstance<CurrencyDto>();
    //     var currency = await _currencyApi!.Create(cur);
    //     var currencyExists = await _mongoDbContextCurrency
    //         .Collection.Find(x =>
    //             x.CurrencyCode == currency.Data!.CurrencyCode && x.CreatedBy == currency.Data.CreatedBy
    //         )
    //         .FirstOrDefaultAsync();
    //     Assert.That(currencyExists, Is.Not.EqualTo(null));
    //     Assert.That(currencyExists.CurrencyCode, Is.EqualTo(cur.CurrencyCode));
    //     _currencyIdForItem = currency.Data!.Id;
    // }

    // [Given("The user created following country and stored the country id")]
    // public async Task GivenTheUserCreatedFollowingCountryAndStoredTheCountryId(Table table)
    // {
    //     var countryDto = table.CreateInstance<CreateCountryDto>();
    //     countryDto.CurrencyId = _currencyId;
    //     var country = await _countryApi!.Create(countryDto);
    //     var countryExists = await _mongoDbContextCountry
    //         .Collection.Find(x => x.Name == country.Data!.Name)
    //         .FirstOrDefaultAsync();
    //     Assert.That(countryExists, Is.Not.EqualTo(null));
    //     Assert.That(countryExists.Name, Is.EqualTo(country.Data!.Name));
    //     _countryId = country.Data.Id;
    // }

    [Given("The user created following customer and stored the customer id")]
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

    [Given("The user wants to create following invoice")]
    public Task GivenTheUserCreatedFollowingInvoiceAndStoredTheInvoiceId(Table table)
    {
        _invoiceDto = table.CreateInstance<CreateInvoiceDto>();
        _invoiceDto.CustomerId = _customerId;
        _invoiceDto.ClientId = _clientId;
        _invoiceDto.CurrencyId = _currencyId;
        return Task.CompletedTask;
    }

    [Given("the invoice contains the following items")]
    public Task GivenTheInvoiceContainsTheFollowingItems(Table table)
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
        return Task.CompletedTask;
    }

    [When("the user sends the invoice creation request")]
    public async Task WhenTheUserSendsTheInvoiceCreationRequest()
    {
        try
        {
            await _invoiceApi!.Create(_invoiceDto!);
        }
        catch (ApiException e)
        {
            Console.WriteLine(e.Content);
            throw;
        }
    }

    [Then("the invoice should be saved in the database")]
    public async Task ThenTheInvoiceShouldBeSavedInTheDb()
    {
        var invoice = await _mongoDbContext
            .Collection.Find(x => x.InvoiceNumber == _invoiceDto!.InvoiceNumber)
            .FirstOrDefaultAsync();
        Assert.That(invoice, Is.Not.EqualTo(null));
        Assert.That(invoice.Items!.Count, Is.EqualTo(1));
        Assert.That(invoice.Items[0].Name, Is.EqualTo(_invoiceDto!.Items![0].Name));
    }

    [AfterScenario("@CreateInvoice")]
    public async Task CleanUp()
    {
        await _mongoDbContextCountry.Collection.DeleteOneAsync(x => x.Id == _countryId.ToObjectId());
        await _mongoDbContextCurrency.Collection.DeleteOneAsync(x => x.Id == _currencyId.ToObjectId());
        await _mongoDbContextCurrency.Collection.DeleteOneAsync(x => x.Id == _currencyIdForItem.ToObjectId());
        await _mongoDbContextCustomer.Collection.DeleteOneAsync(x => x.Id == _customerId.ToObjectId());
        await _mongoDbContextClient.Collection.DeleteOneAsync(x => x.Id == _clientId.ToObjectId());
        await _mongoDbContext.Collection.DeleteOneAsync(x =>
            x.InvoiceNumber == _invoiceDto!.InvoiceNumber
            && (x.CreatedBy == "OwnerUserTest" | x.CreatedBy == "ClientAdminTest" || x.CreatedBy == "OperatorTest")
        );
    }
}
