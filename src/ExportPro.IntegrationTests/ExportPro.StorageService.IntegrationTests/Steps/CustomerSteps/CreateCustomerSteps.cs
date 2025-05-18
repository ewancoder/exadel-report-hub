using ExportPro.Shared.IntegrationTests.Auth;
using ExportPro.Shared.IntegrationTests.Helpers;
using ExportPro.Shared.IntegrationTests.MongoDbContext;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs.CustomerDTO;
using ExportPro.StorageService.SDK.Refit;
using MongoDB.Driver;
using Refit;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace ExportPro.StorageService.IntegrationTests.Steps.CustomerSteps;

[Binding]
[Scope(Tag = "CreateCustomer")]
public class CreateCustomerSteps
{
    private readonly IMongoDbContext<Customer> _mongoDbContext = new MongoDbContext<Customer>();
    private readonly IMongoDbContext<Country> _mongoDbContextCountry = new MongoDbContext<Country>();
    private readonly IMongoDbContext<Currency> _mongoDbContextCurrency = new MongoDbContext<Currency>();
    private ICountryController? _countryApi;
    private Guid _countryId;
    private ICurrencyController? _currencyApi;
    private ICustomerController? _customerApi;
    private CreateUpdateCustomerDto? _customerDto;

    [Given(@"The user is logged in with email '(.*)' and password '(.*)' and has necessary permissions")]
    public async Task GivenTheUserIsLoggedInWithEmailAndPasswordAndHasNecessaryPermissions(
        string email,
        string password
    )
    {
        var jwtToken = await UserLogin.Login(email, password);
        var httpClient = HttpClientForRefit.GetHttpClient(jwtToken, 1500);
        _countryApi = RestService.For<ICountryController>(httpClient);
        _currencyApi = RestService.For<ICurrencyController>(httpClient);
        _customerApi = RestService.For<ICustomerController>(httpClient);
    }

    [Given(@"The user has following country ""(.*)""")]
    public async Task GivenTheUserHasFollowingCountry(string country)
    {
        var countryResponse = await _countryApi!.GetByCode(country, default);
        _countryId = countryResponse.Data!.Id;
    }

    [Given("The user wants to create following customer")]
    public Task GivenTheUserWantsToCreateFollowingCustomer(Table table)
    {
        _customerDto = table.CreateInstance<CreateUpdateCustomerDto>();
        _customerDto.CountryId = _countryId;
        return Task.CompletedTask;
    }

    [When("the user sends the customer creation request")]
    public Task WhenTheUserSendsTheCustomerCreationRequest()
    {
        return _customerApi!.Create(_customerDto!);
    }

    [Then("the customer should be saved in the database")]
    public async Task ThenTheCustomerShouldBeSavedInTheDb()
    {
        var customer = await _mongoDbContext
            .Collection.Find(x => x.Email == "TESTUSER####TESTCUSTOMER@gmail.com")
            .FirstOrDefaultAsync();
        Assert.That(customer, Is.Not.EqualTo(null));
        Assert.That(customer.Name, Is.EqualTo("TESTUSER####TESTCUSTOMER"));
    }

    [AfterScenario("@CreateCustomer")]
    public async Task CleanUp()
    {
        await _mongoDbContext.Collection.DeleteOneAsync(x =>
            (x.CreatedBy == "OwnerUserTest" || x.CreatedBy == "ClientAdminTest" || x.CreatedBy == "OperatorTest")
            && x.Name == _customerDto!.Name
        );
    }
}
