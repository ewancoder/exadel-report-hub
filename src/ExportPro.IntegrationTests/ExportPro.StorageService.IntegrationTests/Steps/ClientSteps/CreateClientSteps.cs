using System.Net;
using ExportPro.Common.Shared.Library;
using ExportPro.Shared.IntegrationTests.MongoDbContext;
using ExportPro.Shared.IntegrationTests.Seed;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Refit;
using ExportPro.StorageService.SDK.Responses;
using MongoDB.Driver;
using Refit;
using TechTalk.SpecFlow;

namespace ExportPro.StorageService.IntegrationTests.Steps.ClientSteps
{
    [Binding]
    public class CreateClientSteps
    {
        private IMongoDbContext<Client>? _mongoDbContext;
        private IStorageServiceApi? _storageServiceApi;
        private ClientDto? _clientDto;
        private BaseResponse<ClientResponse>? _refitClientDto;

        [BeforeScenario]
        public async Task Setup()
        {
            _mongoDbContext = new MongoDbContext<Client>();
            var jwtToken = await AddUser.AddClientAdmin();
            var httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:1500") };
            httpClient.DefaultRequestHeaders.Authorization = new("Bearer", jwtToken);
            _storageServiceApi = RestService.For<IStorageServiceApi>(httpClient);
        }

        [Given(@"I have a client with name  and description")]
        public void GivenIHaveAClientWithNameAndDescription()
        {
            _clientDto = new ClientDto { Name = "ClientISme", Description = "Description" };
        }

        [When(@"the user sends the client creation request")]
        public async Task WhenTheUserSendsTheClientCreationRequest()
        {
            _refitClientDto = await _storageServiceApi!.CreateClient(_clientDto!);
        }

        [Then("the response status should be Success")]
        public void ThenTheResponseStatusShouldBeSuccess()
        {
            Assert.That(_refitClientDto!.ApiState, Is.EqualTo(HttpStatusCode.OK));
        }

        [Then(@"the client should be saved in the database")]
        public async Task ThenTheClientShouldBeSavedInTheDatabase()
        {
            var client = await _mongoDbContext!.Collection.Find(x => x.Name == _clientDto!.Name).FirstOrDefaultAsync();
            Assert.That(client, Is.Not.Null);
            Assert.That(client.Name, Is.EqualTo(_clientDto!.Name));
            Assert.That(client.Description, Is.EqualTo(_clientDto.Description));
        }

        [AfterScenario]
        public async Task Cleanup()
        {
            var client = await _mongoDbContext!.Collection.Find(x => x.Name == _clientDto!.Name).FirstOrDefaultAsync();
            if (client != null)
            {
                await _mongoDbContext.Collection.DeleteOneAsync(x => x.Id == client.Id);
            }
        }
    }
}
