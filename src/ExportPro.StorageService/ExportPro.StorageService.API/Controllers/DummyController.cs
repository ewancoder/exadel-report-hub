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
namespace ExportPro.StorageService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DummyController : ControllerBase
{
    private readonly IAuth _auth;
    private readonly ClientRepository _clientRepository;
    public DummyController(IAuth authApi, ClientRepository clientRepository)
    {
        _auth = authApi;
        _clientRepository = clientRepository;
         
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
        Client client = new()
        {
            Name = clientdto.Name,
            Description = clientdto.Description
        };
        await _clientRepository.AddOneAsync(client, CancellationToken.None);
        return Ok();
    }
    [HttpGet("get")]
    public async Task<IActionResult> GetClients()
    {
        var clients = await _clientRepository.GetClients();

        return Ok(clients);
    }
    [HttpPut("updateclient")]
    public async Task<IActionResult> UpdateClient(string Name,[FromBody] Client clientdto) {
        if(Name == null) return BadRequest();
        var client = await _clientRepository.GetClientByName(Name);
        if (!ModelState.IsValid) return BadRequest();
        client.Name = clientdto.Name;
        client.Description = clientdto.Description;
        await _clientRepository.UpdateOneAsync(client, CancellationToken.None);
        return Ok();
    }
}
