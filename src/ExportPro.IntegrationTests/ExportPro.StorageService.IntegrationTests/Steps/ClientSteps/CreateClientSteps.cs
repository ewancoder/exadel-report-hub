using System.Net;
using ExportPro.Auth.SDK.Interfaces;
using ExportPro.Common.Shared.Library;
using ExportPro.Shared.IntegrationTests.Auth;
using ExportPro.Shared.IntegrationTests.MongoDbContext;
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
        private IMongoDbContext<Client>? _mongoDbContext = new MongoDbContext<Client>();
        private IStorageServiceApi? _storageServiceApi;
        private ClientDto? _clientDto;
        private BaseResponse<ClientResponse>? _refitClientDto;

        [Given(@"The user have a client with name  and description")]
        public void GivenIHaveAClientWithNameAndDescription()
        {
            _clientDto = new ClientDto { Name = "ClientISme", Description = "Description" };
        }

        [Given("The user have a valid  token")]
        public async Task HaveValidUserToken()
        {
            string jwtToken = await UserLogin.Login("SuperAdminTest@gmail.com", "SuperAdminTest2@");
            HttpClient httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:1500") };
            httpClient.DefaultRequestHeaders.Authorization = new("Bearer", jwtToken);
            _storageServiceApi = RestService.For<IStorageServiceApi>(httpClient);
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
        public void Cleanup()
        {
            var client = _mongoDbContext!
                .Collection.Find(x => x.Name == "ClientISme")
                .FirstOrDefaultAsync()
                .GetAwaiter()
                .GetResult();
            if (client != null)
            {
                _mongoDbContext.Collection.DeleteOne(x => x.Id == client.Id);
            }
        }
    }
}
