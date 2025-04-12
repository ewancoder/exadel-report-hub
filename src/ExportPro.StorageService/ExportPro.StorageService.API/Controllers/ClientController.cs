using System.ComponentModel.DataAnnotations;
using ExportPro.Auth.SDK.DTOs;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ExportPro.Auth.SDK.Interfaces;
using ExportPro.StorageService.DataAccess.Repositories;
using ExportPro.StorageService.Models.Models;
using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.SDK.DTOs;
using MongoDB.Bson;
using Refit;
using Microsoft.AspNetCore.Authorization;
using ExportPro.StorageService.SDK.Responses;
using ExportPro.StorageService.SDK.Mapping;
using MediatR;
using ExportPro.StorageService.DataAccess.Services;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using MongoDB.Driver;

namespace ExportPro.StorageService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClientController : ControllerBase
{
    private readonly IAuth _auth;
    private readonly ClientRepository _clientRepository;
    private readonly IClientService _clientService;
    public ClientController(IAuth authApi, ClientRepository clientRepository, IClientService clientService)
    {
        _auth = authApi;
        _clientRepository = clientRepository;
        _clientService = clientService;
    }
    [HttpPost("dummyregister")]
    [SwaggerOperation(Summary = "Register a new user")]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    public async Task<IActionResult> DummyRegister([FromBody] UserRegisterDto dto)
    {
        var Register = await _auth.RegisterAsync(dto);
        return Ok(Register);
    }
    [HttpPost("dummylogin")]
    [SwaggerOperation(Summary = "Login a user")]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    public async Task<IActionResult> DummyLogin([FromBody] UserLoginDto dto)
    {
        var response = await _auth.LoginAsync(dto);
        return Ok(response);
    }
    [HttpPost("CreatingClient")]
    [SwaggerOperation(Summary = "Creating a client")]
    [ProducesResponseType(typeof(ClientResponse), 200)]
    public async Task<IActionResult> CreateClient([FromBody] ClientDto clientdto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        var exists = await _clientRepository.GetOneAsync(x=>x.Name == clientdto.Name, CancellationToken.None);
        if (exists != null)
        {
            return BadRequest(new BadRequestResponse{Messages = ["Client already exists"]});
        }
        var clientResponse =await _clientService.AddClientFromClientDto(clientdto);
        return Ok(new SuccessResponse<ClientResponse>(clientResponse));
    }
    [HttpGet("get/clients")]
    [SwaggerOperation(Summary = "Getting  clients which are not soft deleted")]
    [ProducesResponseType(typeof(List<ClientResponse>), 200)]
    public async Task<IActionResult> GetClients()
    {
        var clients = await _clientService.GetClientsService();
        return Ok(new SuccessResponse<List<ClientResponse>>(clients));
    }
    [HttpGet("get/client/{Clientid}")]
    [SwaggerOperation(Summary = "Getting  client by client id which is not soft deleted")]
    [ProducesResponseType(typeof(List<ClientResponse>), 200)]
    public async Task<IActionResult> GetClientById([Required] string Clientid)
    {
        if (!ModelState.IsValid) return BadRequest(new BadRequestResponse());
        var clientresponse = await _clientService.GetClientById(Clientid);
        if(clientresponse == null) return BadRequest(new BadRequestResponse
        {
            Messages = ["Client Id does not exist"]
        });
        return Ok(new SuccessResponse<ClientResponse>(clientresponse));
    }
    [HttpGet("get/clients/includesoftdeleted")]
    [SwaggerOperation(Summary = "Getting  clients. including soft deleted ")]
    [ProducesResponseType(typeof(List<ClientResponse>), 200)]
    public async Task<IActionResult> GetAllCLientsIncludingSoftDeleted()
    {
        var clients = await _clientService.GetAllCLientsIncludingSoftDeleted();
        return Ok( new SuccessResponse<List<ClientResponse>>(clients));
    }
    [HttpPut("update/client")]
    [SwaggerOperation(Summary = "Updating the client")]
    [ProducesResponseType(typeof(List<ClientResponse>), 200)]
    public async Task<IActionResult> UpdateClient([Required]string clientid,[FromBody] ClientResponse clientdto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!ObjectId.TryParse(clientid, out var objectId))
        {
            return BadRequest(new BadRequestResponse
            {
                Messages =["Invalid client id format"]
            });
        }
        var client = await _clientRepository.GetOneAsync(x=>x.Id ==ObjectId.Parse(clientid), CancellationToken.None);
        if(client == null) return BadRequest(new BadRequestResponse{Messages = ["Client id does not exists"]});
        ClientResponse afterUpdate=await _clientService.UpdateClient(client);
        var response = new
        {
            before = new Response{Message = "Before The Update",ClientResponse = ClientToClientResponse.ClientToClientReponse(client)},
            after = new Response{Message = "after The Update",ClientResponse = clientdto}
        };
        return Ok(new SuccessResponse<object>(response,"Updated"));
    }
    [HttpDelete("softdelete/client/{Clientid}")]
    [SwaggerOperation(Summary = "Soft deleting the client by clientid")]
    [ProducesResponseType(typeof(Response), 200)]
    public async Task<IActionResult> SoftDeleteClient([Required]string Clientid)
    {
        if (!ObjectId.TryParse(Clientid, out var objectId))
        {
            return BadRequest(new BadRequestResponse
            {
                Messages =["Invalid client id format"]
            });
        }      
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var exists = await _clientRepository.GetOneAsync(x => x.Id == objectId, CancellationToken.None);
        if(exists == null) return BadRequest(new BadRequestResponse{Messages = ["Client does not exists"]});
        string messege =await _clientService.SoftDeleteClient(objectId);
        Response response = new()
        {
            Message = messege,
            ClientResponse = ClientToClientResponse.ClientToClientReponse(exists)
        };
        return Ok(new SuccessResponse<Response>(response));
    }
    [HttpDelete("delete/client/{ClientId}")]
    [SwaggerOperation(Summary = "deleting the client by clientid")]
    [ProducesResponseType(typeof(Response), 200)]
    public async Task<IActionResult> DeleteClient([Required] string Clientid)
    {
        if (!ObjectId.TryParse(Clientid, out var objectId))
        {
            return BadRequest(new BadRequestResponse
            {
                Messages =["Invalid client id format"]
            });
        }      
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var messege = await _clientService.DeleteClient(objectId);
        var exists = await _clientRepository.GetOneAsync(x=>x.Id == objectId, CancellationToken.None);
        if(exists == null) return BadRequest(new BadRequestResponse{Messages = ["Client does not exists"]});
        Response response = new()
        {
            Message = messege,
            ClientResponse = ClientToClientResponse.ClientToClientReponse(exists)
        };
        return Ok(new SuccessResponse<Response>(response));
    }
}
