using ExportPro.Common.Shared.Extensions;
using ExportPro.Shared.IntegrationTests.Auth;
using ExportPro.Shared.IntegrationTests.Configs;
using ExportPro.Shared.IntegrationTests.Helpers;
using ExportPro.Shared.IntegrationTests.MongoDbContext;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Refit;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Refit;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace ExportPro.StorageService.IntegrationTests.Steps.ClientSteps;

[Binding]
[Scope(Tag = "ClientManagement")]
public class PositiveClientOperationsSteps(FeatureContext featureContext)
{
    private static readonly IMongoDbContext<Client> _mongoDbContext = new MongoDbContext<Client>();
    private static Guid _clientId;
    private IClientController? _clientApi;
    private ClientDto? _clientDto;
    private readonly IConfiguration _config = LoadingConfig.LoadConfig();

    [Given(@"The ""(.*)"" user is logged in with email and password and has necessary permissions")]
    public async Task GivenTheUserIsLoggedInWithEmailAndPasswordAndHasNecessaryPermissions(string user)
    {
        var jwtToken = await UserLogin.Login(
            _config.GetSection($"Users:{user}:Email").Value!,
            _config.GetSection($"Users:{user}:Password").Value!
        );
        var httpClient = HttpClientForRefit.GetHttpClient(jwtToken, 1500);
        _clientApi = RestService.For<IClientController>(httpClient);
    }

    [Given(@"The user has a client with name and description")]
    public void GivenIHaveAClientWithNameAndDescription(Table table)
    {
        _clientDto = table.CreateInstance<ClientDto>();
    }

    [When(@"the user sends the client creation request")]
    public async Task WhenTheUserSendsTheClientCreationRequest()
    {
        var client = await _clientApi!.CreateClient(_clientDto!);
        featureContext["ClientId"] = client.Data!.Id;
    }

    [Then(@"the client should be saved in the database")]
    public async Task ThenTheClientShouldBeSavedInTheDatabase()
    {
        var client = await _mongoDbContext.Collection.Find(x => x.Name == _clientDto!.Name).FirstOrDefaultAsync();
        Assert.That(client, Is.Not.Null);
        Assert.That(client.Name, Is.EqualTo(_clientDto!.Name));
        Assert.That(client.Description, Is.EqualTo(_clientDto.Description));
    }

    [When("The user sends a delete request with client id")]
    public async Task WhenUserSendsDeleteRequestWithClientId()
    {
        _clientId = (Guid)featureContext["ClientId"];
        await _clientApi!.SoftDeleteClient(_clientId);
    }

    [Then("The client should be deleted")]
    public async Task ThenTheClientShouldBeDeleted()
    {
        var client = await _mongoDbContext.Collection.Find(x => x.Id == _clientId.ToObjectId()).FirstOrDefaultAsync();
        Assert.That(client, Is.Not.Null);
        Assert.That(client.IsDeleted, Is.EqualTo(true));
    }

    [AfterFeature("@ClientManagement")]
    public static async Task CleanUp()
    {
        await _mongoDbContext.Collection.DeleteOneAsync(x => x.Id == _clientId.ToObjectId());
    }
}
