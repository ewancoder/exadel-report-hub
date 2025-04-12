using ExportPro.Auth.SDK.DTOs;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ExportPro.Auth.SDK;
using ExportPro.Auth.SDK.Interfaces;
using ExportPro.StorageService.DataAccess.Repositories;
using ExportPro.StorageService.Models.Models;
using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.StorageService.SDK.DTOs;
using MongoDB.Bson;
using Refit;
using Microsoft.AspNetCore.Authorization;
using ExportPro.StorageService.SDK.Responses;
using ExportPro.StorageService.SDK.Mapping;
using MediatR;
using ExportPro.StorageService.CQRS.Queries;
namespace ExportPro.StorageService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DummyController : ControllerBase
{
    private readonly IAuth _auth;
    private readonly ClientRepository _clientRepository;
    private readonly IMediator _mediator;
    public DummyController(IAuth authApi, ClientRepository clientRepository, IMediator mediator)
    {
        _auth = authApi;
        _clientRepository = clientRepository;
        _mediator = mediator;
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
    [HttpPost("createclient")]
    public async Task<IActionResult> CreateClient([FromBody] ClientDto clientdto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        var exists = await _clientRepository.GetOneAsync(x=>x.Name == clientdto.Name, CancellationToken.None);
        if (exists != null)
        {
            return BadRequest("Client already exists");
        }
        Client client = new()
        {
            Name = clientdto.Name,
            Description = clientdto.Description
        };
        await _clientRepository.AddOneAsync(client, CancellationToken.None);
        var cleinrespones = ClientToClientResponse.ClientToClientReponse(client);

        return Ok(cleinrespones);
    }
    [HttpGet("get")]
    public async Task<IActionResult> GetClients()
    {
        var clients = await _mediator.Send(new GetClientsQuery());
        return Ok(clients);
    }
    [HttpGet("GetClientById")]
    public async Task<IActionResult> GetClientById(string Clientid)
    {
        if (Clientid == null) return BadRequest();
        var objectid = ObjectId.TryParse(Clientid, out var id);
        if (!ModelState.IsValid) return BadRequest();
        try
        {
        var client = await _clientRepository.GetByIdAsync(id, CancellationToken.None);
        if (client == null) return NotFound();
            var cleinrespones = new
            {
                id = client.Id.ToString(),
                name = client.Name,
                description = client.Description,
                UpdatedAt = client.UpdatedAt,
                CreatedAt = client.CreatedAt,
                invoiceIds = client.InvoiceIds,
                customerIds = client.CustomerIds,
                itemIds = client.ItemIds
            };
            return Ok(cleinrespones);
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpGet("GetAllClientsSoftDeletedTo")]
    public async Task<IActionResult> GetAllClientsSoftDeletedTo()
    {
        try
        {
            var clients = await _clientRepository.GetClients();
            var cl = clients.Select(x => new
            {
                id = x.Id.ToString(),
                name = x.Name,
                description = x.Description,
                createdAt = x.CreatedAt,
                updatedAt = x.UpdatedAt,
                invoiceIds = x.InvoiceIds,
                customerIds = x.CustomerIds,
                IsDeleted = x.IsDeleted

            }).ToList();

            return Ok(cl);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPut("updateclient")]
    public async Task<IActionResult> UpdateClient(string clientid,[FromBody] Client clientdto) {
        if(clientid == null) return BadRequest();
        var objectid = ObjectId.TryParse(clientid, out var id);
        var client = await _clientRepository.GetOneAsync(x=>x.Id == id,CancellationToken.None);
        if (!ModelState.IsValid) return BadRequest();
        client.Name = clientdto.Name;
        client.Description = clientdto.Description;
        client.UpdatedAt = DateTime.UtcNow;
        await _clientRepository.UpdateOneAsync(client, CancellationToken.None);
        var cleinrespones = new
        {
            id = client.Id.ToString(),
            name = client.Name,
            description = client.Description,
            UpdatedAt = client.UpdatedAt,
            CreatedAt = client.CreatedAt,
            invoiceIds = client.InvoiceIds,
            customerIds = client.CustomerIds,
            itemIds = client.ItemIds
        };
        return Ok();
    }
    [HttpDelete("deleteclient")]
    public async Task<IActionResult> SoftDeleteClient(string Clientid)
    {
        if (Clientid  == null) return BadRequest();
        var objectid = ObjectId.TryParse(Clientid, out var id);
        if (!ModelState.IsValid) return BadRequest();
        try
        {
        await _clientRepository.SoftDeleteAsync(id, CancellationToken.None);
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }   
        return Ok("Successfull Soft deleted");
    }
    [HttpDelete("deleteclienthard")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> HardDeleteClient(string Clientid)
    {
        if (Clientid == null) return BadRequest();
        var objectid = ObjectId.TryParse(Clientid, out var id);
        if (!ModelState.IsValid) return BadRequest();
        try
        {
            await _clientRepository.DeleteAsync(id, CancellationToken.None);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        return Ok("Successfull Soft deleted");
    }
    //public async Task<IActionResult> AddItemsToClient(string clientId, [FromBody] List<string> itemsId)
    //{
    //    if (string.IsNullOrWhiteSpace(clientId))
    //        return BadRequest("ClientId is required.");

    //    if (!ObjectId.TryParse(clientId, out var objectId))
    //        return BadRequest("Invalid ClientId format.");

    //    if (itemsId == null || itemsId.Count == 0)
    //        return BadRequest("ItemsId list cannot be empty.");

    //    if (!ModelState.IsValid)
    //        return BadRequest(ModelState);

    //    try
    //    {
    //        var client = await _clientRepository.GetByIdAsync(objectId, CancellationToken.None);
    //        if (client == null)
    //            return NotFound("Client not found.");

    //        client.ItemIds ??= new List<string>();
    //        client.ItemIds.AddRange(itemsId);

    //        await _clientRepository.UpdateOneAsync(client, CancellationToken.None);
    //        return Ok("Items added to client.");
    //    }
    //    catch (Exception ex)
    //    {
    //        // You might want to log this exception too
    //        return StatusCode(500, $"Internal server error: {ex.Message}");
    //    }
    //}

  

}
