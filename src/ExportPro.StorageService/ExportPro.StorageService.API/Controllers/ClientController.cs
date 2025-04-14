using System.ComponentModel.DataAnnotations;
using ExportPro.Auth.SDK.DTOs;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ExportPro.Auth.SDK.Interfaces;
using ExportPro.StorageService.DataAccess.Repositories;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.SDK.DTOs;
using MongoDB.Bson;
using ExportPro.StorageService.SDK.Responses;
using ExportPro.StorageService.SDK.Mapping;
using MediatR;
using ExportPro.StorageService.DataAccess.Services;
using ExportPro.StorageService.CQRS.Handlers.Client;



namespace ExportPro.StorageService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClientController : ControllerBase
{
    private readonly IAuth _auth;
    private readonly ClientRepository _clientRepository;
    private readonly IClientService _clientService;
    private readonly IMediator _mediator;
    public ClientController(IAuth authApi,
        IMediator mediator,ClientRepository clientRepository, IClientService clientService)
    {
        _auth = authApi;
        _clientRepository = clientRepository;
        _mediator = mediator;
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
        if (!ModelState.IsValid) return BadRequest();
        var exists = await _clientRepository.GetOneAsync(x=>x.Name == clientdto.Name && x.IsDeleted==false, CancellationToken.None);
        if (exists != null) return BadRequest(new BadRequestResponse{Messages = ["Client already exists"]});
        var clientResponse =await _mediator.Send(new AddClientFromClientDtoCommand(clientdto));
        return Ok(clientResponse);
    }
    [HttpGet("get/clients")]
    [SwaggerOperation(Summary = "Getting  clients which are not soft deleted")]
    [ProducesResponseType(typeof(List<ClientResponse>), 200)]
    public async Task<IActionResult> GetClients()
    {
        var clients = await _mediator.Send(new GetClientsQuery());
        return Ok(clients);
    }
    [HttpGet("get/client/{Clientid}")]
    [SwaggerOperation(Summary = "Getting  client by client id which is not soft deleted")]
    [ProducesResponseType(typeof(List<ClientResponse>), 200)]
    public async Task<IActionResult> GetClientById([Required] string Clientid)
    {
        if (!ModelState.IsValid) return BadRequest(new BadRequestResponse());
        var clientresponse = await _mediator.Send(new GetClientByIdQuery(Clientid));
        if(clientresponse.Data == null) return BadRequest(new BadRequestResponse{Messages = ["Client Id does not exist"]});
        return Ok(clientresponse);
    }
    [HttpGet("get/clients/includesoftdeleted")]
    [SwaggerOperation(Summary = "Getting  clients. including soft deleted ")]
    [ProducesResponseType(typeof(List<ClientResponse>), 200)]
    public async Task<IActionResult> GetAllCLientsIncludingSoftDeleted()
    {
        var clients = await _mediator.Send(new GetAllCLientsIncludingSoftDeletedQuery());
        return Ok( clients);
    }
    [HttpGet("get/clients/fullclient/{clientId}")]
    [SwaggerOperation(Summary = "Getting  client by id including invoices,customers,items")]
    [ProducesResponseType(typeof(List<ClientResponse>), 200)]
    public async Task<IActionResult> GetFullClient(string clientId)
    {
        var clients = await _clientService.GetFullClient(clientId);
        return Ok(clients);
    }
    [HttpGet("get/clients/fullclient/all")]
    [SwaggerOperation(Summary = "Getting  clients including invoices,customers,items")]
    [ProducesResponseType(typeof(List<ClientResponse>), 200)]
    public async Task<IActionResult> GetAllFullClients()
    {
        var clients = await _clientService.GetAllFullClients();
        return Ok(clients);
    }

    [HttpPut("update/client")]
    [SwaggerOperation(Summary = "Updating the client")]
    [ProducesResponseType(typeof(List<ClientResponse>), 200)]
    public async Task<IActionResult> UpdateClient([Required]string clientid,[FromBody] ClientUpdateDto clientdto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!ObjectId.TryParse(clientid, out var objectId))
            return BadRequest(new BadRequestResponse
            {
                Messages =["Invalid client id format"]
            });
        var client = await _clientRepository.GetOneAsync(x=>x.Id ==objectId, CancellationToken.None);
        if(client == null) return BadRequest(new BadRequestResponse{Messages = ["Client id does not exists"]});
        BaseResponse<ClientResponse> afterUpdate=await _mediator.Send(new UpdateClientCommand(clientdto,clientid));
        var response = new
        {
            before =new Response{Message = "before the update",ClientResponse = new BaseResponse<ClientResponse> { Data = ClientToClientResponse.ClientToClientReponse(client)}},
            after = new Response{Message = "after The Update",ClientResponse = afterUpdate}
        };
        return Ok(new SuccessResponse<object>(response,"Updated"));
    }
    [HttpDelete("softdelete/client/{Clientid}")]
    [SwaggerOperation(Summary = "Soft deleting the client by clientid")]
    [ProducesResponseType(typeof(Response), 200)]
    public async Task<IActionResult> SoftDeleteClient([Required]string Clientid)
    {
        if (!ObjectId.TryParse(Clientid, out var objectId))
            return BadRequest(new BadRequestResponse
            {
                Messages =["Invalid client id format"]
            });
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var client_exists=await _mediator.Send(new GetClientByIdQuery(Clientid));
        if(client_exists.Data  == null) return BadRequest(new BadRequestResponse{Messages = ["Client does not exists"]});
        var message = await _mediator.Send(new SoftDeleteClientCommand(objectId));
        client_exists.Data.IsDeleted = true;
        Response response = new()
        {
            Message =message.Data,
            ClientResponse = new BaseResponse<ClientResponse>{Data = client_exists.Data}
        };
        return Ok(response);
    }
    [HttpDelete("delete/client/{ClientId}")]
    [SwaggerOperation(Summary = "deleting the client by clientid")]
    [ProducesResponseType(typeof(Response), 200)]
    public async Task<IActionResult> DeleteClient([Required]string ClientId)
    {
        if (!ObjectId.TryParse(ClientId, out var objectId))
            return BadRequest(new BadRequestResponse
            {
                Messages =["Invalid client id format"]
            });
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var client_exists=await _mediator.Send(new GetClientByIdIncludingSoftDeletedQuery(objectId));
        if(client_exists  == null) return BadRequest(new BadRequestResponse{Messages = ["Client does not exists"]});
        var messege = await  _mediator.Send(new DeleteClientCommand(objectId));
        Response response = new()
        {
            Message = messege.Data,
            ClientResponse = new BaseResponse<ClientResponse>{Data = client_exists.Data}
        };
        return Ok(new SuccessResponse<Response>(response));
    }
    [HttpPost("add/itemids/{clientId}")]
    [SwaggerOperation(Summary = "Add multiple item IDs to a client")]
    [ProducesResponseType(typeof(ClientResponse), 200)]
    public async Task<IActionResult> AddItemIds(
        [Required] string clientId,
        [FromBody, Required] List<string> itemIds)
    {
        if (!ObjectId.TryParse(clientId, out var objId))
            return BadRequest(new BadRequestResponse { Messages = ["Invalid client ID format"] });

        var client = await _clientRepository.GetOneAsync(x => x.Id == objId,CancellationToken.None);
        if (client is null)
            return BadRequest(new BadRequestResponse { Messages = ["Client ID does not exist"] });
        var responseClient = await _clientService.AddItemIds(clientId, itemIds);
        return Ok(new SuccessResponse<ClientResponse>(responseClient, "Item IDs added successfully"));
    }

    [HttpPost("add/customerids/{clientId}")]
    [SwaggerOperation(Summary = "Add multiple customer IDs to a client")]
    [ProducesResponseType(typeof(ClientResponse), 200)]
    public async Task<IActionResult> AddCustomerIds(
        [Required] string clientId,
        [FromBody, Required] List<string> customerIds,
        CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(clientId, out var objId))
            return BadRequest(new BadRequestResponse { Messages = ["Invalid client ID format"] });

        var client = await _clientRepository.GetOneAsync(x => x.Id == objId, cancellationToken);
        if (client is null)
            return BadRequest(new BadRequestResponse { Messages = ["Client ID does not exist"] });
        var responseClient =await _clientService.AddItemIds(clientId,customerIds);
        return Ok(new SuccessResponse<ClientResponse>(responseClient, "Customer IDs added successfully"));
    }
     [HttpPost("add/invoiceids/{clientId}")]
     [SwaggerOperation(Summary = "Add multiple invoice IDs to a client")]
     [ProducesResponseType(typeof(ClientResponse), 200)]
     public async Task<IActionResult> AddInvoiceIds(
         [Required] string clientId,
         [FromBody, Required] List<string> invoiceIds,
         CancellationToken cancellationToken)
     {
         if (!ObjectId.TryParse(clientId, out var objId))
             return BadRequest(new BadRequestResponse { Messages = ["Invalid client ID format"] });
         var client = await _clientRepository.GetOneAsync(x => x.Id == objId, cancellationToken);
         if (client is null)
             return BadRequest(new BadRequestResponse { Messages = ["Client ID does not exist"] });
         var responseClient = await _clientService.AddInvoiceIds(clientId, invoiceIds);
         return Ok(new SuccessResponse<ClientResponse>(responseClient, "Invoice IDs added successfully"));
    }
}
