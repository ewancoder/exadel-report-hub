using ExportPro.Shared.IntegrationTests.Auth;
using ExportPro.Shared.IntegrationTests.Helpers;
using ExportPro.Shared.IntegrationTests.MongoDbContext;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Refit;
using MongoDB.Driver;
using Refit;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace ExportPro.StorageService.IntegrationTests.Steps.ClientSteps;

[Binding]
[Scope(Tag = "CreateClient")]
public class CreateClientSteps
{
    private readonly IMongoDbContext<Client> _mongoDbContext = new MongoDbContext<Client>();
    private IClientApi? _clientApi;
    private ClientDto? _clientDto;

    [Given("The user is logged in with email  and password  and has necessary permissions")]
    public async Task HaveValidUserToken(Table table)
    {
        var jwtToken = await UserLogin.Login(table.Rows[0]["Email"], table.Rows[0]["Password"]);
        var httpClient = HttpClientForRefit.GetHttpClient(jwtToken, 1500);
        _clientApi = RestService.For<IClientApi>(httpClient);
    }

    [Given(@"The user have a client with name and description")]
    public void GivenIHaveAClientWithNameAndDescription(Table table)
    {
        _clientDto = table.CreateInstance<ClientDto>();
    }

    [When(@"the user sends the client creation request")]
    public async Task WhenTheUserSendsTheClientCreationRequest()
    {
        await _clientApi!.CreateClient(_clientDto!);
    }

    [Then(@"the client should be saved in the database")]
    public async Task ThenTheClientShouldBeSavedInTheDatabase()
    {
        var client = await _mongoDbContext.Collection.Find(x => x.Name == _clientDto!.Name).FirstOrDefaultAsync();
        Assert.That(client, Is.Not.Null);
        Assert.That(client.Name, Is.EqualTo(_clientDto!.Name));
        Assert.That(client.Description, Is.EqualTo(_clientDto.Description));
    }

    [AfterScenario("@CreateClient")]
    public async Task CleanUp()
    {
        await _mongoDbContext.Collection.DeleteOneAsync(x =>
            x.CreatedBy == "SuperAdminTest" && x.Name == _clientDto!.Name
        );
    }
}
